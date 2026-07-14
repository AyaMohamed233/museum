using System;
using UnityEngine;

public class MicrophoneCapture : MonoBehaviour
{
    [Header("Microphone Settings")]
    [SerializeField] private int sampleRate = 16000;
    [SerializeField] private int chunkSize = 1024;  // Reduced from 2048 to 1024 for lower latency (64ms)
    [SerializeField] private string deviceName = null;
    
    [Header("Status")]
    [SerializeField] private bool isRecording = false;
    [SerializeField] private bool isMuted = false; // Muted while statue is speaking
    
    private AudioClip microphoneClip;
    private int lastSamplePosition = 0;
    private float[] audioBuffer;
    private bool isInitialized = false;
    
    public event Action<byte[]> OnAudioChunk;
    public bool IsRecording => isRecording;
    public bool IsMuted => isMuted;

    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        if (isInitialized) return;
        
        audioBuffer = new float[chunkSize];
        
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("❌ No microphone detected!");
            Debug.LogError("Please connect a microphone or check Windows microphone permissions");
            return;
        }
        
        Debug.Log($"✅ Found {Microphone.devices.Length} microphone(s):");
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log($"  [{i}] {Microphone.devices[i]}");
        }
        
        Debug.Log($"Using device: {(string.IsNullOrEmpty(deviceName) ? "Default (first available)" : deviceName)}");
        
        // Test microphone access
        try
        {
            string testDevice = string.IsNullOrEmpty(deviceName) ? Microphone.devices[0] : deviceName;
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(testDevice, out minFreq, out maxFreq);
            Debug.Log($"Microphone capabilities: {minFreq}Hz - {maxFreq}Hz");
            
            if (maxFreq > 0 && sampleRate > maxFreq)
            {
                Debug.LogWarning($"Requested sample rate {sampleRate}Hz exceeds max {maxFreq}Hz");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error testing microphone: {ex.Message}");
        }
        
        isInitialized = true;
    }

    // C2 Fix: Non-blocking flag for coroutine-based initialization
    private bool isStartingMicrophone = false;
    
    public void StartRecording()
    {
        if (isRecording || isStartingMicrophone)
        {
            Debug.LogWarning("Already recording or starting");
            return;
        }
        
        if (!isInitialized)
        {
            Initialize();
        }

        // C2 Fix: Use coroutine instead of Thread.Sleep to avoid blocking main thread
        StartCoroutine(StartRecordingCoroutine());
    }
    
    private System.Collections.IEnumerator StartRecordingCoroutine()
    {
        isStartingMicrophone = true;
        
        Debug.Log($"Starting microphone: {(string.IsNullOrEmpty(deviceName) ? "Default" : deviceName)}");
        microphoneClip = Microphone.Start(deviceName, true, 1, sampleRate);
        
        if (microphoneClip == null)
        {
            Debug.LogError("Failed to start microphone - microphoneClip is null");
            isStartingMicrophone = false;
            yield break;
        }
        
        // C2 Fix: Wait for microphone without blocking — yield each frame
        float startTime = Time.realtimeSinceStartup;
        float maxWaitTime = 5.0f; // 5 second timeout
        
        while (!(Microphone.GetPosition(deviceName) > 0))
        {
            if (Time.realtimeSinceStartup - startTime > maxWaitTime)
            {
                Debug.LogError($"Microphone start timeout after {maxWaitTime}s");
                Debug.LogError($"Available devices: {string.Join(", ", Microphone.devices)}");
                Microphone.End(deviceName);
                isStartingMicrophone = false;
                yield break;
            }
            yield return null; // Wait one frame (non-blocking!)
        }
        
        float elapsed = Time.realtimeSinceStartup - startTime;
        lastSamplePosition = 0;
        isRecording = true;
        isStartingMicrophone = false;
        
        Debug.Log($"✅ Recording started at {sampleRate}Hz after {elapsed * 1000:F0}ms");
    }

    public void StopRecording()
    {
        if (!isRecording)
            return;

        Microphone.End(deviceName);
        isRecording = false;
        
        Debug.Log("Recording stopped");
    }

    private void Update()
    {
        if (!isRecording || microphoneClip == null || isMuted)
            return;

        int currentPosition = Microphone.GetPosition(deviceName);
        
        if (currentPosition < 0 || currentPosition == lastSamplePosition)
            return;

        // Calculate samples to read
        int samplesToRead = currentPosition - lastSamplePosition;
        
        if (samplesToRead < 0)
        {
            // Wrapped around
            samplesToRead = microphoneClip.samples - lastSamplePosition + currentPosition;
        }

        // Process in chunks
        while (samplesToRead >= chunkSize)
        {
            // Get audio data
            microphoneClip.GetData(audioBuffer, lastSamplePosition);
            
            // Convert float32 to PCM16
            byte[] pcmData = ConvertToPCM16(audioBuffer);
            
            // Send chunk
            OnAudioChunk?.Invoke(pcmData);
            
            lastSamplePosition += chunkSize;
            
            if (lastSamplePosition >= microphoneClip.samples)
            {
                lastSamplePosition = 0;
            }
            
            samplesToRead -= chunkSize;
        }
    }
    
    public void Mute()
    {
        if (isMuted) return;
        
        isMuted = true;
        Debug.Log("🔇 Microphone muted (statue speaking)");
    }
    
    public void Unmute(float delay = 0.5f)
    {
        if (!isMuted) return;
        
        // Add delay to ensure statue finished completely
        if (delay > 0)
        {
            Invoke(nameof(UnmuteImmediate), delay);
        }
        else
        {
            UnmuteImmediate();
        }
    }
    
    private void UnmuteImmediate()
    {
        isMuted = false;
        Debug.Log("🔊 Microphone unmuted (ready to listen)");
    }

    private byte[] ConvertToPCM16(float[] samples)
    {
        byte[] pcmData = new byte[samples.Length * 2]; // 16-bit = 2 bytes per sample
        
        for (int i = 0; i < samples.Length; i++)
        {
            // Clamp to [-1, 1]
            float sample = Mathf.Clamp(samples[i], -1f, 1f);
            
            // Convert to 16-bit PCM
            short pcmSample = (short)(sample * short.MaxValue);
            
            // Little-endian
            pcmData[i * 2] = (byte)(pcmSample & 0xFF);
            pcmData[i * 2 + 1] = (byte)((pcmSample >> 8) & 0xFF);
        }
        
        return pcmData;
    }

    private void OnDestroy()
    {
        StopRecording();
    }

    private void OnApplicationQuit()
    {
        StopRecording();
    }
}
