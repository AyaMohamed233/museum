using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public static MuseumManager Instance { get; private set; }
    
    [Header("Server Settings")]
    [SerializeField] private string serverUrl = "ws://localhost:8080";
    [SerializeField] private string authToken = "egyptian-guide-2025"; // C5: shared secret
    
    [Header("Audio Components")]
    [SerializeField] private MicrophoneCapture microphoneCapture;
    
    [Header("Reconnection")]
    [SerializeField] private float reconnectDelay = 3f;
    [SerializeField] private int maxReconnectAttempts = 10;
    
    [Header("Heartbeat")]
    [SerializeField] private float heartbeatInterval = 15f;
    
    [Header("Echo Cancellation")]
    [SerializeField] private float muteTimeoutSeconds = 10f; // H3: timeout fallback
    
    [Header("Status")]
    [SerializeField] private string visitorId;
    [SerializeField] private string activeStatueId;
    [SerializeField] private string connectionState = "DISCONNECTED";
    
    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    
    // C1: Message queue for thread-safe main-thread dispatch
    private Queue<byte[]> audioQueue = new Queue<byte[]>();
    private Queue<string> messageQueue = new Queue<string>(); // C1: text messages queued to main thread
    private object audioQueueLock = new object();
    private object messageQueueLock = new object();
    
    // H6: Cap audio queue
    private const int MAX_AUDIO_QUEUE_SIZE = 200;
    
    // C3: Semaphore for serialized WebSocket sends
    private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1, 1);
    
    private AudioPlayer audioPlayer;
    
    // H1: Reconnection state
    private int reconnectAttempts = 0;
    private bool isReconnecting = false;
    private bool intentionalDisconnect = false;
    
    // H2: Heartbeat
    private float lastHeartbeatTime = 0f;
    private float lastPongTime = 0f;
    
    // H3: Echo cancellation timeout
    private float muteStartTime = 0f;
    private bool isMutedByEcho = false;
    
    // Events
    public event Action<string> OnVisitorRegistered;
    public event Action<string> OnStatueActivated;
    public event Action<string> OnStatueDeactivated;
    public event Action<string> OnGracePeriodStarted;
    public event Action<string, string> OnTranscriptionReceived;
    public event Action<string> OnError;
    
    public string VisitorId => visitorId;
    public string ActiveStatueId => activeStatueId;
    public bool IsConnected => connectionState == "CONNECTED";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Setup audio player
        audioPlayer = gameObject.GetComponent<AudioPlayer>();
        if (audioPlayer == null)
        {
            audioPlayer = gameObject.AddComponent<AudioPlayer>();
        }
        
        // Subscribe to AudioPlayer events for microphone muting
        audioPlayer.OnStartedSpeaking += OnStatueStartedSpeaking;
        audioPlayer.OnFinishedSpeaking += OnStatueFinishedSpeaking;
        
        // Setup microphone capture
        if (microphoneCapture == null)
        {
            microphoneCapture = gameObject.GetComponent<MicrophoneCapture>();
            if (microphoneCapture == null)
            {
                microphoneCapture = gameObject.AddComponent<MicrophoneCapture>();
            }
        }
        
        microphoneCapture.OnAudioChunk += OnMicrophoneChunk;
    }
    
    private void OnStatueStartedSpeaking()
    {
        // Mute microphone when statue starts speaking
        if (microphoneCapture != null)
        {
            microphoneCapture.Mute();
            isMutedByEcho = true;
            muteStartTime = Time.time;
        }
    }
    
    private void OnStatueFinishedSpeaking()
    {
        // NOTE: This event is no longer used for unmuting
        // We now rely on turn_complete message from Gemini API
        // This prevents premature unmuting when audio has gaps between chunks
    }

    private async void Start()
    {
        Debug.Log("MuseumManager starting...");
        await ConnectToServer();
        
        // Test ping after connection
        if (IsConnected)
        {
            await Task.Delay(1000);
            await TestPing();
        }
    }
    
    private async Task TestPing()
    {
        Debug.Log("Testing ping...");
        var pingMessage = new StatueActionMessage { action = "ping" };
        await SendJsonMessage(pingMessage);
    }

    public async Task ConnectToServer()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            Debug.Log("Already connected to server");
            return;
        }

        try
        {
            webSocket = new ClientWebSocket();
            
            // C5: Add auth token as subprotocol/header
            if (!string.IsNullOrEmpty(authToken))
            {
                webSocket.Options.SetRequestHeader("X-Auth-Token", authToken);
            }
            
            cancellationTokenSource = new CancellationTokenSource();
            
            connectionState = "CONNECTING";
            Debug.Log($"Connecting to {serverUrl}...");
            
            await webSocket.ConnectAsync(new Uri(serverUrl), cancellationTokenSource.Token);
            
            connectionState = "CONNECTED";
            reconnectAttempts = 0;
            isReconnecting = false;
            lastPongTime = Time.time;
            lastHeartbeatTime = Time.time;
            Debug.Log($"Connected to server! State: {webSocket.State}");
            
            _ = ReceiveLoop();
        }
        catch (Exception ex)
        {
            connectionState = "DISCONNECTED";
            Debug.LogError($"Connection failed: {ex.Message}");
            OnError?.Invoke($"Connection failed: {ex.Message}");
            
            // H1: Auto-reconnect on failure
            if (!intentionalDisconnect)
            {
                ScheduleReconnect();
            }
        }
    }

    // H1: Reconnection logic
    private async void ScheduleReconnect()
    {
        if (isReconnecting || intentionalDisconnect) return;
        if (reconnectAttempts >= maxReconnectAttempts)
        {
            Debug.LogError($"Max reconnect attempts ({maxReconnectAttempts}) reached. Giving up.");
            OnError?.Invoke("Connection lost. Please restart.");
            return;
        }
        
        isReconnecting = true;
        reconnectAttempts++;
        float delay = reconnectDelay * Mathf.Pow(1.5f, reconnectAttempts - 1); // exponential backoff
        Debug.Log($"Reconnecting in {delay:F1}s (attempt {reconnectAttempts}/{maxReconnectAttempts})...");
        
        await Task.Delay((int)(delay * 1000));
        
        if (!intentionalDisconnect)
        {
            isReconnecting = false;
            await ConnectToServer();
        }
    }

    public async Task DisconnectFromServer()
    {
        if (webSocket == null)
            return;

        try
        {
            intentionalDisconnect = true;
            cancellationTokenSource?.Cancel();
            
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }
            
            webSocket?.Dispose();
            webSocket = null;
            connectionState = "DISCONNECTED";
            visitorId = null;
            activeStatueId = null;
            
            Debug.Log("Disconnected from server");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during disconnect: {ex.Message}");
        }
    }

    // H5: Changed from async void to proper method with try-catch wrapper
    public async void EnterStatueZone(string statueId, string voiceName = null)
    {
        try
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Cannot enter statue zone: not connected");
                return;
            }
            
            Debug.Log($"Entering statue zone: {statueId} with voice: {voiceName ?? "default"}");
            
            var message = new StatueActionMessage
            {
                action = "ENTER",
                statueId = statueId,
                voiceName = voiceName
            };
            
            await SendJsonMessage(message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error entering statue zone: {ex.Message}");
        }
    }

    // H5: Changed from async void to proper method with try-catch wrapper
    public async void ExitStatueZone(string statueId)
    {
        try
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Cannot exit statue zone: not connected");
                return;
            }
            
            Debug.Log($"Exiting statue zone: {statueId}");
            
            // NEW: Clear audio immediately on client side
            if (audioPlayer != null)
            {
                audioPlayer.ClearBuffer();
            }
            lock (audioQueueLock)
            {
                audioQueue.Clear();
            }
            Debug.Log("🗑️ Immediate audio queue clear on Exit");
            
            var message = new StatueActionMessage
            {
                action = "EXIT",
                statueId = statueId
            };
            
            await SendJsonMessage(message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error exiting statue zone: {ex.Message}");
        }
    }

    // C3: Serialized WebSocket sends via SemaphoreSlim
    private async Task SendJsonMessage(object message)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError($"Cannot send message: webSocket is {(webSocket == null ? "null" : webSocket.State.ToString())}");
            return;
        }

        await sendSemaphore.WaitAsync();
        try
        {
            string json = JsonUtility.ToJson(message);
            Debug.Log($"Sending JSON: {json}");
            byte[] data = Encoding.UTF8.GetBytes(json);
            
            await webSocket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Text,
                true,
                cancellationTokenSource.Token
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message: {ex.Message}");
        }
        finally
        {
            sendSemaphore.Release();
        }
    }

    // C3: Serialized audio sends via same SemaphoreSlim
    // H5: Wrapped in try-catch for async void safety
    private float lastAudioBlockLogTime = 0f;  // Throttle debug logs
    
    private async void OnMicrophoneChunk(byte[] audioData)
    {
        try
        {
            if (!IsConnected || string.IsNullOrEmpty(activeStatueId))
            {
                if (Time.time - lastAudioBlockLogTime > 3f)
                {
                    Debug.LogWarning($"🔇 Audio blocked: IsConnected={IsConnected}, activeStatueId={activeStatueId ?? "null"}");
                    lastAudioBlockLogTime = Time.time;
                }
                return;
            }

            // Don't send audio if statue is speaking (prevent echo)
            if (audioPlayer != null && audioPlayer.IsSpeaking)
                return;

            // Check if microphone is muted
            if (microphoneCapture != null && microphoneCapture.IsMuted)
            {
                if (Time.time - lastAudioBlockLogTime > 3f)
                {
                    Debug.LogWarning($"🔇 Audio blocked: microphone is muted (isMuted=true)");
                    lastAudioBlockLogTime = Time.time;
                }
                return;
            }

            await sendSemaphore.WaitAsync();
            try
            {
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(audioData),
                        WebSocketMessageType.Binary,
                        true,
                        cancellationTokenSource.Token
                    );
                }
            }
            finally
            {
                sendSemaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            // Normal during shutdown
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending audio: {ex.Message}");
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024 * 64];
        
        try
        {
            Debug.Log("ReceiveLoop started");
            
            while (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationTokenSource.Token
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("Server closed connection");
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    var audioData = new byte[result.Count];
                    Array.Copy(buffer, audioData, result.Count);
                    
                    // H6: Cap audio queue size
                    lock (audioQueueLock)
                    {
                        if (audioQueue.Count < MAX_AUDIO_QUEUE_SIZE)
                        {
                            audioQueue.Enqueue(audioData);
                        }
                        // else: drop chunk to prevent memory growth
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    // C1: Queue text messages for main-thread processing
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    lock (messageQueueLock)
                    {
                        messageQueue.Enqueue(message);
                    }
                }
            }
            
            Debug.Log("ReceiveLoop ended");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Receive loop cancelled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Receive error: {ex.Message}");
        }
        
        // Connection lost — clean up and reconnect
        connectionState = "DISCONNECTED";
        if (!intentionalDisconnect)
        {
            ScheduleReconnect();
        }
    }

    // C1: Now safely called from main thread only (via Update)
    private void ProcessServerMessage(string json)
    {
        try
        {
            var msg = JsonUtility.FromJson<ServerMessage>(json);
            
            switch (msg.type)
            {
                case "visitor_registered":
                    visitorId = msg.visitorId;
                    Debug.Log($"Registered as visitor: {visitorId}");
                    OnVisitorRegistered?.Invoke(visitorId);
                    break;
                    
                case "statue_activated":
                    activeStatueId = msg.statueId;
                    Debug.Log($"Statue activated: {activeStatueId}");
                    
                    // Reset speaking state for new session
                    if (audioPlayer != null)
                    {
                        audioPlayer.ResetSpeakingState();
                    }
                    
                    // Reset echo cancellation state AND unmute microphone
                    // Critical: isMuted can be stuck from previous session's audio during grace period
                    isMutedByEcho = false;
                    if (microphoneCapture != null)
                    {
                        microphoneCapture.Unmute(0f);  // Ensure clean unmuted state before recording
                    }
                    
                    microphoneCapture.StartRecording();
                    OnStatueActivated?.Invoke(activeStatueId);
                    break;
                    
                case "statue_deactivated":
                    Debug.Log($"Statue deactivated: {msg.statueId}");
                    microphoneCapture.StopRecording();
                    // Reset muted state to prevent isMuted from being stuck across sessions
                    if (microphoneCapture != null)
                    {
                        microphoneCapture.Unmute(0f);
                    }
                    audioPlayer.ClearBuffer();
                    if (activeStatueId == msg.statueId)
                    {
                        activeStatueId = null;
                    }
                    isMutedByEcho = false;
                    OnStatueDeactivated?.Invoke(msg.statueId);
                    break;
                    
                case "statue_reactivated":
                    Debug.Log($"Statue reactivated: {msg.statueId}");
                    
                    // Reset speaking state for reactivated session
                    if (audioPlayer != null)
                    {
                        audioPlayer.ResetSpeakingState();
                    }
                    
                    // Reset echo cancellation AND unmute microphone
                    isMutedByEcho = false;
                    if (microphoneCapture != null)
                    {
                        microphoneCapture.Unmute(0f);  // Critical: clear stuck isMuted from previous session
                    }
                    
                    // Ensure microphone is recording after reactivation
                    if (!microphoneCapture.IsRecording)
                    {
                        microphoneCapture.StartRecording();
                    }
                    break;
                    
                case "grace_period_started":
                    Debug.Log($"Grace period started for: {msg.statueId}");
                    OnGracePeriodStarted?.Invoke(msg.statueId);
                    break;
                    
                case "status":
                    Debug.Log($"Server status: {msg.status}");
                    break;
                    
                case "transcription":
                    OnTranscriptionReceived?.Invoke(msg.sender, msg.text);
                    break;
                    
                case "clear_audio":
                    Debug.Log($"🗑️ clear_audio received");
                    audioPlayer.ClearBuffer();
                    break;
                    
                case "turn_complete":
                    Debug.Log($"✅ turn_complete received - Statue finished speaking");
                    // Reset speaking state in audio player
                    if (audioPlayer != null)
                    {
                        audioPlayer.ResetSpeakingState();
                    }
                    // Unmute microphone immediately after statue finished
                    if (microphoneCapture != null)
                    {
                        microphoneCapture.Unmute(0.2f);
                    }
                    isMutedByEcho = false;
                    break;
                    
                case "error":
                    Debug.LogError($"Server error: {msg.message}");
                    OnError?.Invoke(msg.message);
                    break;
                    
                case "pong":
                    lastPongTime = Time.time;
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to parse message: {ex.Message}");
        }
    }

    private void Update()
    {
        // C1: Process text message queue on main thread (thread-safe)
        lock (messageQueueLock)
        {
            while (messageQueue.Count > 0)
            {
                var message = messageQueue.Dequeue();
                ProcessServerMessage(message);
            }
        }
        
        // Process audio queue on main thread
        lock (audioQueueLock)
        {
            while (audioQueue.Count > 0)
            {
                var audioData = audioQueue.Dequeue();
                audioPlayer.AddAudioData(audioData);
            }
        }
        
        // H2: Periodic heartbeat
        if (IsConnected && Time.time - lastHeartbeatTime > heartbeatInterval)
        {
            lastHeartbeatTime = Time.time;
            var pingMessage = new StatueActionMessage { action = "ping" };
            _ = SendJsonMessage(pingMessage);
        }
        
        // H3: Echo cancellation timeout fallback
        if (isMutedByEcho && Time.time - muteStartTime > muteTimeoutSeconds)
        {
            Debug.LogWarning($"⚠️ Echo cancellation timeout ({muteTimeoutSeconds}s) — force unmuting microphone");
            if (microphoneCapture != null)
            {
                microphoneCapture.Unmute(0f);
            }
            if (audioPlayer != null)
            {
                audioPlayer.ResetSpeakingState();
            }
            isMutedByEcho = false;
        }
    }

    private void OnDestroy()
    {
        if (microphoneCapture != null)
        {
            microphoneCapture.OnAudioChunk -= OnMicrophoneChunk;
        }
        
        if (audioPlayer != null)
        {
            audioPlayer.OnStartedSpeaking -= OnStatueStartedSpeaking;
            audioPlayer.OnFinishedSpeaking -= OnStatueFinishedSpeaking;
        }
        
        intentionalDisconnect = true;
        _ = DisconnectFromServer();
        
        sendSemaphore?.Dispose();
    }

    private void OnApplicationQuit()
    {
        intentionalDisconnect = true;
        _ = DisconnectFromServer();
    }

    [Serializable]
    private class StatueActionMessage
    {
        public string action;
        public string statueId;
        public string voiceName;
    }

    [Serializable]
    private class ServerMessage
    {
        public string type;
        public string visitorId;
        public string statueId;
        public string status;
        public string sender;
        public string text;
        public string message;
        public int duration;
    }
}
