using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the HUD UI system with persistent action buttons and dynamic context buttons.
/// 
/// Persistent buttons (always visible at bottom-left):
///   [ESC] Menu  |  [M] Map  |  [I] Information
/// 
/// Dynamic buttons (bottom-right, appear based on raycast):
///   [T] Talk to [Statue Name]
/// 
/// Panels:
///   - Information Panel: Statue name + description
///   - Map Panel: Placeholder for museum map
/// (Menu panel is handled by PauseMenuManager)
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ══════════════════════════════════════════════════════
    //  INSPECTOR FIELDS
    // ══════════════════════════════════════════════════════

    [Header("═══ Persistent Buttons (Always Visible) ═══")]
    [SerializeField] private GameObject menuButtonGroup;    // [ESC] Menu
    [SerializeField] private GameObject mapButtonGroup;     // [M] Map
    [SerializeField] private GameObject infoButtonGroup;    // [I] Information

    [Header("═══ Dynamic Buttons (Raycast Dependent) ═══")]
    [SerializeField] private GameObject talkButtonGroup;    // [T] Talk
    [SerializeField] private TextMeshProUGUI talkLabel;     // "Talk to Ramesses"

    [Header("═══ Information Panel ═══")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoTitleText;        // Statue display name
    [SerializeField] private TextMeshProUGUI infoDescriptionText;  // Statue description

    [Header("═══ Map Panel ═══")]
    [SerializeField] private GameObject mapPanel;

    // ══════════════════════════════════════════════════════
    //  EVENTS (for other scripts to subscribe to)
    // ══════════════════════════════════════════════════════

    /// <summary>Fired when the Map is opened or closed.</summary>
    public event Action<bool> OnMapToggled;

    /// <summary>Fired when the Info panel is opened or closed.</summary>
    public event Action<bool> OnInfoToggled;

    // ══════════════════════════════════════════════════════
    //  PRIVATE STATE
    // ══════════════════════════════════════════════════════

    private bool isMapOpen = false;
    private bool isInfoOpen = false;

    // Cached statue data for the Information panel
    private string cachedStatueName = "";
    private string cachedStatueDescription = "";
    private bool hasStatueInView = false;

    // Reference to Cinemachine for cursor lock management
    private MonoBehaviour cinemachineInput;

    // ══════════════════════════════════════════════════════
    //  PUBLIC PROPERTIES
    // ══════════════════════════════════════════════════════

    /// <summary>True if any HUD panel (Map, Info) is currently open.</summary>
    public bool IsAnyPanelOpen => isMapOpen || isInfoOpen;

    // ══════════════════════════════════════════════════════
    //  UNITY LIFECYCLE
    // ══════════════════════════════════════════════════════

    private void Start()
    {
        // Ensure persistent buttons are visible
        SetActive(menuButtonGroup, true);
        SetActive(mapButtonGroup, true);
        SetActive(infoButtonGroup, true);

        // Hide dynamic button initially
        SetActive(talkButtonGroup, false);

        // Hide all panels initially
        SetActive(infoPanel, false);
        SetActive(mapPanel, false);

        // Cache references for cursor management
        GameObject cmCam = GameObject.Find("CinemachineCamera");
        if (cmCam != null)
        {
            cinemachineInput = cmCam.GetComponent("CinemachineInputAxisController") as MonoBehaviour;
            if (cinemachineInput == null)
            {
                cinemachineInput = cmCam.GetComponent("CinemachineInputProvider") as MonoBehaviour;
            }
        }

        Debug.Log("✅ HUDManager initialized");
    }

    private void Update()
    {
        HandleInput();
    }

    // ══════════════════════════════════════════════════════
    //  INPUT HANDLING
    // ══════════════════════════════════════════════════════

    private void HandleInput()
    {
        // Don't process HUD input if the game is paused by PauseMenuManager
        if (PauseMenuManager.IsGamePaused) return;

        // I — Toggle Information (only if looking at a statue)
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (hasStatueInView)
            {
                ToggleInfo();
            }
            else
            {
                Debug.Log("ℹ️ No statue in view — cannot open Information panel");
            }
        }

        // M — Toggle Map
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        // ESC — Close any open HUD panel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool consumed = false;
            
            if (isInfoOpen) 
            { 
                CloseInfo(); 
                consumed = true; 
            }
            else if (isMapOpen)  
            { 
                CloseMap();  
                consumed = true; 
            }

            // If we closed a panel, tell PauseMenuManager to ignore this ESC press
            if (consumed)
            {
                PauseMenuManager pmm = FindObjectOfType<PauseMenuManager>();
                if (pmm != null)
                {
                    pmm.ConsumeEscape();
                }
            }
        }
    }

    // ══════════════════════════════════════════════════════
    //  TALK BUTTON (Dynamic — called by StatueRaycastDetector)
    // ══════════════════════════════════════════════════════

    public void ShowTalkButton(string statueName, bool isInSession, string statueDescription = "")
    {
        SetActive(talkButtonGroup, true);

        if (talkLabel != null)
        {
            talkLabel.text = isInSession
                ? $"Stop talking to {statueName}"
                : $"Talk to {statueName}";
        }

        // Cache statue data for the Information panel
        cachedStatueName = statueName;
        cachedStatueDescription = statueDescription;
        hasStatueInView = true;

        // Update info panel if it's currently open (player turned to a different statue)
        if (isInfoOpen)
        {
            UpdateInfoPanelContent();
        }
    }

    public void HideTalkButton()
    {
        SetActive(talkButtonGroup, false);
        hasStatueInView = false;

        // Close info panel if it was open and we lost sight of the statue
        if (isInfoOpen)
        {
            CloseInfo();
        }
    }

    // ══════════════════════════════════════════════════════
    //  INFORMATION PANEL
    // ══════════════════════════════════════════════════════

    public void ToggleInfo()
    {
        if (isInfoOpen) CloseInfo();
        else OpenInfo();
    }

    private void OpenInfo()
    {
        if (!hasStatueInView) return;

        // Close other panels first
        if (isMapOpen) CloseMap();

        isInfoOpen = true;
        SetActive(infoPanel, true);
        UpdateInfoPanelContent();

        SetCursorState(false);

        OnInfoToggled?.Invoke(true);
        Debug.Log($"ℹ️ Information panel opened: {cachedStatueName}");
    }

    private void CloseInfo()
    {
        isInfoOpen = false;
        SetActive(infoPanel, false);

        // Only re-lock cursor if no other panel is open
        if (!IsAnyPanelOpen) SetCursorState(true);

        OnInfoToggled?.Invoke(false);
        Debug.Log("ℹ️ Information panel closed");
    }

    private void UpdateInfoPanelContent()
    {
        if (infoTitleText != null)
        {
            infoTitleText.text = cachedStatueName;
        }

        if (infoDescriptionText != null)
        {
            infoDescriptionText.text = string.IsNullOrEmpty(cachedStatueDescription)
                ? "No description available."
                : cachedStatueDescription;
        }
    }

    // ══════════════════════════════════════════════════════
    //  MAP PANEL
    // ══════════════════════════════════════════════════════

    public void ToggleMap()
    {
        if (isMapOpen) CloseMap();
        else OpenMap();
    }

    private void OpenMap()
    {
        // Close other panels first
        if (isInfoOpen) CloseInfo();

        isMapOpen = true;
        SetActive(mapPanel, true);

        SetCursorState(false);

        OnMapToggled?.Invoke(true);
        Debug.Log("🗺️ Map opened");
    }

    private void CloseMap()
    {
        isMapOpen = false;
        SetActive(mapPanel, false);

        if (!IsAnyPanelOpen) SetCursorState(true);

        OnMapToggled?.Invoke(false);
        Debug.Log("🗺️ Map closed");
    }

    // ══════════════════════════════════════════════════════
    //  CURSOR & CAMERA MANAGEMENT
    // ══════════════════════════════════════════════════════

    private void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;

        // Disable/enable Cinemachine camera rotation
        if (cinemachineInput != null)
        {
            cinemachineInput.enabled = locked;
        }
    }

    // ══════════════════════════════════════════════════════
    //  UTILITY
    // ══════════════════════════════════════════════════════

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null && obj.activeSelf != active)
        {
            obj.SetActive(active);
        }
    }
}
