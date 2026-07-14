using UnityEngine;

/// <summary>
/// Detects statues using raycast from camera center
/// Uses FIXED DISTANCE (interactionDistance) for all statues via hit.distance
/// Player manually activates/deactivates session by pressing T key
/// Auto-closes session if player moves too far away (maxSessionDistance)
/// </summary>
public class StatueRaycastDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDetectionDistance = 100f;  // Maximum detection range
    [SerializeField] private float interactionDistance = 4f;  // Fixed interaction distance for all statues (changed from 3m to 4m)
    [SerializeField] private KeyCode interactionKey = KeyCode.T;  // Key to toggle session
    [SerializeField] private LayerMask statueLayer;
    [SerializeField] private bool enableRaycastDetection = true;
    [SerializeField] private float raycastInterval = 0.0f;  // Check every frame for instant detection (set to 0.05 if performance is an issue)
    
    [Header("Audio Reference")]
    [SerializeField] private AudioPlayer audioPlayer;  // Reference to audio player for immediate stop
    
    [Header("HUD")]
    [SerializeField] private HUDManager hudManager;  // Reference to the HUD Manager

    
    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private bool showDebugInfo = false;  // Show distance debug info
    
    private Camera playerCamera;
    private StatueInfo currentStatue;  // Statue currently being looked at
    private StatueInfo activeSessionStatue;  // Statue with active session
    private bool isSessionActive = false;
    private float lastRaycastTime = 0f;  // For optimization
    private float lastKnownDistance = 0f;  // Cache last known distance to avoid recalculation
    private MicrophoneCapture cachedMicrophone;  // Cache microphone reference
    private float lastSeenActiveStatueTime = 0f;  // Track when we last saw the active statue
    private const float MAX_TIME_WITHOUT_SEEING_STATUE = 1.0f;  // Close session after 1 second of not seeing statue
    private float lastToggleTime = 0f;  // Track last T key press time
    private const float TOGGLE_COOLDOWN = 0.3f;  // Minimum 0.3 second between toggles (reduced from 1s)
    private int sessionVersion = 0;  // Incremented each time a session opens, used to ignore stale deactivation events
    
    private void Start()
    {
        playerCamera = GetComponent<Camera>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        if (playerCamera == null)
        {
            Debug.LogError("⚠️ StatueRaycastDetector: No camera found!");
            enabled = false;
            return;
        }
        
        // Auto-find AudioPlayer if not assigned
        if (audioPlayer == null)
        {
            audioPlayer = FindObjectOfType<AudioPlayer>();
            if (audioPlayer == null)
            {
                Debug.LogWarning("⚠️ AudioPlayer not found - immediate audio stop won't work");
            }
        }
        
        // Cache microphone reference
        if (MuseumManager.Instance != null)
        {
            cachedMicrophone = MuseumManager.Instance.GetComponent<MicrophoneCapture>();
        }
        
        // Auto-find HUDManager if not assigned
        if (hudManager == null)
        {
            hudManager = FindObjectOfType<HUDManager>();
            if (hudManager == null)
            {
                Debug.LogWarning("⚠️ HUDManager not found in scene!");
            }
        }
        
        // Subscribe to MuseumManager events (safe here: all Awake() have run before any Start())
        if (MuseumManager.Instance != null)
        {
            MuseumManager.Instance.OnStatueDeactivated += HandleStatueDeactivated;
        }
        
        Debug.Log("✅ StatueRaycastDetector initialized");
    }
    
    private void Update()
    {
        if (!enableRaycastDetection || (hudManager != null && hudManager.IsAnyPanelOpen))
            return;
        
        // Check for T key press with cooldown to prevent accidental double-press
        if (Input.GetKeyDown(interactionKey))
        {
            float timeSinceLastToggle = Time.time - lastToggleTime;
            
            if (timeSinceLastToggle >= TOGGLE_COOLDOWN)
            {
                if (currentStatue != null)
                {
                    // Looking at a statue — toggle session on/off
                    Debug.Log($"🔑 T pressed: currentStatue={currentStatue.statueId}, isSessionActive={isSessionActive}, activeSession={activeSessionStatue?.statueId ?? "null"}");
                    ToggleSession(currentStatue);
                }
                else if (isSessionActive && activeSessionStatue != null)
                {
                    // Not looking at any statue but have active session — exit it
                    Debug.Log($"🔴 Exiting session with {activeSessionStatue.statueId} (pressed T while looking away)");
                    ExitSession(activeSessionStatue.statueId);
                }
                else
                {
                    Debug.Log($"🔑 T pressed but no valid action: currentStatue={currentStatue?.statueId ?? "null"}, isSessionActive={isSessionActive}");
                }
                lastToggleTime = Time.time;
            }
            else
            {
                Debug.Log($"⏱️ T key pressed too soon ({timeSinceLastToggle:F2}s) - ignoring (cooldown: {TOGGLE_COOLDOWN}s)");
            }
        }
        
        // Optimized raycast - only check every raycastInterval seconds
        if (Time.time - lastRaycastTime < raycastInterval)
            return;
        
        lastRaycastTime = Time.time;
        
        // Cast ray from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Debug visualization
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDetectionDistance, Color.yellow);
        }
        
        // --- SINGLE DECISION POINT for UI visibility ---
        bool foundValidTarget = false;
        StatueInfo detectedStatue = null;
        float detectedDistance = 0f;
        
        // Check if ray hits something on the statue layer
        if (Physics.Raycast(ray, out hit, maxDetectionDistance, statueLayer))
        {
            // Try to find StatueInfo on the hit object or its parent (handles child colliders)
            StatueInfo statue = hit.collider.GetComponent<StatueInfo>();
            if (statue == null)
            {
                statue = hit.collider.GetComponentInParent<StatueInfo>();
            }
            
            if (statue != null && hit.distance <= interactionDistance)
            {
                foundValidTarget = true;
                detectedStatue = statue;
                detectedDistance = hit.distance;
                
                if (showDebugInfo)
                {
                    Debug.Log($"✅ Statue {statue.statueId} detected at {detectedDistance:F1}m (within {interactionDistance}m)");
                }
            }
            else if (showDebugInfo && statue != null)
            {
                Debug.Log($"⚠️ Statue {statue.statueId} detected but too far: {hit.distance:F1}m (max: {interactionDistance}m)");
            }
        }
        
        // --- APPLY RESULT ---
        if (foundValidTarget)
        {
            // Looking at a valid statue within range → show UI
            HandleStatueLook(detectedStatue, detectedDistance);
        }
        else
        {
            // NOT looking at any valid statue → ALWAYS hide UI
            if (currentStatue != null)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"👁️ Lost sight of {currentStatue.statueId} - Hiding UI");
                }
                currentStatue.SetHighlight(false);
                currentStatue = null;
            }
            HideScreenUI();  // Always hide, even if currentStatue was already null
        }
        
        // Auto-close session if not seeing active statue for too long
        if (isSessionActive && activeSessionStatue != null)
        {
            // Check if we're currently looking at the active statue
            bool lookingAtActiveStatue = (currentStatue != null && currentStatue == activeSessionStatue);
            
            if (!lookingAtActiveStatue)
            {
                // Not looking at active statue - check timeout
                float timeSinceLastSeen = Time.time - lastSeenActiveStatueTime;
                
                if (timeSinceLastSeen > MAX_TIME_WITHOUT_SEEING_STATUE)
                {
                    Debug.Log($"⏱️ Haven't seen {activeSessionStatue.statueId} for {timeSinceLastSeen:F1}s - Auto-closing session immediately");
                    ExitSession(activeSessionStatue.statueId);
                }
            }
        }
    }
    
    private void HandleStatueLook(StatueInfo statue, float distance)
    {
        // New statue detected
        if (currentStatue != statue)
        {
            // Unhighlight previous statue
            if (currentStatue != null)
            {
                currentStatue.SetHighlight(false);
            }
            
            currentStatue = statue;
            statue.SetHighlight(true);
        }
        
        // Update cached distance
        lastKnownDistance = distance;
        
        // If this is the active session statue, update last seen time
        if (isSessionActive && activeSessionStatue == statue)
        {
            lastSeenActiveStatueTime = Time.time;
        }
        
        // Update screen UI
        UpdateScreenUI(statue, distance);
    }
    
    private void UpdateScreenUI(StatueInfo statue, float distance)
    {
        if (hudManager == null) return;
        
        bool hasActiveSession = isSessionActive && activeSessionStatue == statue;
        hudManager.ShowTalkButton(statue.displayName, hasActiveSession, statue.description);
    }
    
    private void HideScreenUI()
    {
        if (hudManager != null)
        {
            hudManager.HideTalkButton();
        }
    }
    
    private void ToggleSession(StatueInfo statue)
    {
        // Check if this statue already has an active session
        bool hasActiveSession = isSessionActive && activeSessionStatue == statue;
        
        if (hasActiveSession)
        {
            // Close session
            Debug.Log($"🔴 Closing session with: {statue.statueId}");
            ExitSession(statue.statueId);
        }
        else
        {
            // Close any existing session first
            if (isSessionActive && activeSessionStatue != null)
            {
                Debug.Log($"🔄 Switching from {activeSessionStatue.statueId} to {statue.statueId}");
                ExitSession(activeSessionStatue.statueId);
            }
            
            // Open new session
            Debug.Log($"🟢 Opening session with: {statue.statueId}");
            ActivateStatue(statue);
        }
    }
    
    private void ActivateStatue(StatueInfo statue)
    {
        if (MuseumManager.Instance != null)
        {
            // Get voice name from statue (defaults to null if not set)
            string voiceName = string.IsNullOrEmpty(statue.voiceName) ? null : statue.voiceName;
            
            MuseumManager.Instance.EnterStatueZone(statue.statueId, voiceName);
            isSessionActive = true;
            activeSessionStatue = statue;
            sessionVersion++;  // Increment session version to invalidate any pending stale deactivation events
            lastSeenActiveStatueTime = Time.time;  // Initialize timer
            Debug.Log($"🟢 Session v{sessionVersion} opened with: {statue.statueId}");
            
            // Update screen UI
            if (currentStatue != null)
            {
                UpdateScreenUI(statue, lastKnownDistance);
            }
        }
        else
        {
            Debug.LogError("❌ MuseumManager.Instance is null!");
        }
    }
    
    private void ExitSession(string statueId)
    {
        // Immediate local stops (don't wait for server response)
        if (audioPlayer != null)
        {
            audioPlayer.StopPlayback();
            Debug.Log("🔇 Audio stopped immediately (local)");
        }
        
        // Stop microphone immediately using cached reference
        if (cachedMicrophone != null)
        {
            cachedMicrophone.StopRecording();
            cachedMicrophone.Unmute(0f);  // Reset isMuted to prevent stuck mute across sessions
            Debug.Log("🎤 Microphone stopped + unmuted (local)");
        }
        
        // Send EXIT message to server
        if (MuseumManager.Instance != null)
        {
            MuseumManager.Instance.ExitStatueZone(statueId);
        }
        
        // Clear session state
        isSessionActive = false;
        activeSessionStatue = null;
        Debug.Log($"🔴 Session closed for: {statueId} (v{sessionVersion})");
        
        // Update screen UI if still looking at the statue
        if (currentStatue != null)
        {
            UpdateScreenUI(currentStatue, lastKnownDistance);
        }
    }
    
    // Called by MuseumManager when session ends
    public void OnSessionEnded()
    {
        isSessionActive = false;
        activeSessionStatue = null;
        Debug.Log("✅ Session ended, detection re-enabled");
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (MuseumManager.Instance != null)
        {
            MuseumManager.Instance.OnStatueDeactivated -= HandleStatueDeactivated;
        }
    }
    
    private void HandleStatueDeactivated(string statueId)
    {
        // Only clear session if this deactivation matches the CURRENT active session
        // This prevents stale deactivation events (from old grace periods) from killing new sessions
        if (activeSessionStatue != null && activeSessionStatue.statueId == statueId)
        {
            Debug.Log($"📥 statue_deactivated received for {statueId} (current session v{sessionVersion})");
            isSessionActive = false;
            activeSessionStatue = null;
            
            // Update screen UI if still looking at the statue
            if (currentStatue != null && currentStatue.statueId == statueId)
            {
                UpdateScreenUI(currentStatue, lastKnownDistance);
            }
            
            Debug.Log("✅ Session ended via server deactivation");
        }
        else
        {
            // Stale deactivation event — ignore it
            Debug.Log($"⚠️ Ignoring stale statue_deactivated for {statueId} (activeSession={activeSessionStatue?.statueId ?? "null"}, v{sessionVersion})");
        }
    }
}


