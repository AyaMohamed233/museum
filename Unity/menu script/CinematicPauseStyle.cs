using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cinematic Glassmorphism â€” Ancient Egyptian + Modern Tech â€” Premium AAA Game UI.
///
/// Full visual theme engine for the pause menu:
/// â€¢ Fullscreen cinematic dark overlay (brightness 75%)
/// â€¢ Frosted glass panel with multi-layer shadow system
/// â€¢ Gold-accented buttons with glass hover states
/// â€¢ Red-tinted exit button with separate glow
/// â€¢ Gold-filled volume slider with rounded track/thumb
/// â€¢ "MENU" title in Cinzel-style wide spacing
/// â€¢ Settings sub-panel with frosted glass background
///
/// Ø£Ø¶Ù Ù‡Ø°Ø§ Ø§Ù„ÙƒÙ…Ø¨ÙˆÙ†Ù†Øª Ø¹Ù„Ù‰ Ù†ÙØ³ Ø§Ù„Ù€ GameObject Ø§Ù„Ù„ÙŠ Ø¹Ù„ÙŠÙ‡ PauseMenuManager.
/// ÙƒÙ„ Ø§Ù„Ø£Ù„ÙˆØ§Ù† ÙˆØ§Ù„Ø£Ø¨Ø¹Ø§Ø¯ Ù‚Ø§Ø¨Ù„Ø© Ù„Ù„ØªØ¹Ø¯ÙŠÙ„ Ù…Ù† Ø§Ù„Ù€ Inspector.
/// </summary>
[RequireComponent(typeof(PauseMenuManager))]
[ExecuteAlways]
public class CinematicPauseStyle : MonoBehaviour
{
    // =============================================
    // â”â”â” COLOR PALETTE â”â”â”
    // =============================================

    [Header("â”â”â” GOLD & GLASS COLORS â”â”â”")]

    [Tooltip("#f0c84a â€” Solid Gold primary accent")]
    public Color primaryAccent = new Color(0.941f, 0.784f, 0.290f, 1f);

    [Tooltip("rgba(20,15,10,0.45) â€” Dark cinematic glass background")]
    public Color glassBackground = new Color(0.12f, 0.10f, 0.08f, 0.95f);

    [Tooltip("rgba(255,220,140,0.18) â€” Pale gold glass border")]
    public Color glassBorder = new Color(1f, 0.863f, 0.549f, 0.6f);

    [Tooltip("Pure white main text")]
    public Color mainTextColor = Color.white;

    [Tooltip("rgba(255,255,255,0.92) â€” Slightly soft white button text")]
    public Color buttonTextColor = new Color(1f, 1f, 1f, 0.92f);

    [Tooltip("rgba(0,0,0,0.15) â€” Cinematic overlay (brightness 75%)")]
    public Color overlayDarkness = new Color(0f, 0f, 0f, 0.5f);

    // =============================================
    // â”â”â” BUTTON COLORS â”â”â”
    // =============================================

    [Header("â”â”â” BUTTON SYSTEM â”â”â”")]

    [Tooltip("rgba(255,255,255,0.08) â€” Button normal glass background")]
    public Color buttonNormal = new Color(1f, 1f, 1f, 0.15f);

    [Tooltip("rgba(255,255,255,0.12) â€” Button normal border")]
    public Color buttonBorder = new Color(1f, 1f, 1f, 0.4f);

    [Tooltip("rgba(255,210,100,0.18) â€” Hover gold background")]
    public Color hoverBackground = new Color(1f, 0.824f, 0.392f, 0.18f);

    [Header("INNER EFFECTS")]
    [Tooltip("#f0c84a â€” Hover text becomes gold")]
    public Color hoverTextColor = new Color(0.941f, 0.784f, 0.290f, 1f);

    [Tooltip("rgba(240,200,74,0.25) â€” Pressed button state")]
    public Color buttonPressed = new Color(0.941f, 0.784f, 0.290f, 0.25f);

    // =============================================
    // â” â” â”  EXIT BUTTON â” â” â” 
    // =============================================

    [Header("â” â” â”  EXIT BUTTON â” â” â” ")]

    [Tooltip("rgba(180,40,40,0.35) â€” Exit button base")]
    public Color exitButtonBase = new Color(0.706f, 0.157f, 0.157f, 0.35f);

    [Tooltip("rgba(220,60,60,0.55) â€” Exit button hover")]
    public Color exitButtonHover = new Color(0.863f, 0.235f, 0.235f, 0.55f);

    [Tooltip("rgba(220,60,60,0.7) â€” Exit button pressed")]
    public Color exitPressed = new Color(0.863f, 0.235f, 0.235f, 0.7f);

    // =============================================
    // â” â” â”  SLIDER â” â” â” 
    // =============================================

    [Header("â” â” â”  VOLUME SLIDER â” â” â” ")]

    [Tooltip("rgba(240,200,74,0.85) â€” Strong glowing gold fill")]
    public Color sliderFill = new Color(0.941f, 0.784f, 0.290f, 0.85f);

    [Tooltip("rgba(255,255,255,0.15) â€” Transparent white empty track")]
    public Color sliderEmpty = new Color(1f, 1f, 1f, 0.15f);

    [Tooltip("Track height in pixels")]
    public float sliderTrackHeight = 6f;

    [Tooltip("Track border radius")]
    public int sliderTrackRadius = 6;

    [Tooltip("Thumb diameter in pixels")]
    public float sliderThumbSize = 20f;

    [Tooltip("Thumb glow spread in pixels (the gold ring around the thumb)")]
    public float sliderThumbGlowSpread = 3f;

    // =============================================
    // â” â” â”  PANEL DESIGN â” â” â” 
    // =============================================

    [Header("â” â” â”  PANEL DESIGN â” â” â” ")]

    [Tooltip("Panel corner radius â€” 20px")]
    public int panelCornerRadius = 20;

    [Header("Panel Layout")]
    [Range(0.2f, 0.8f)]
    [Tooltip("Panel width as a percentage of screen width")]
    public float panelWidthPercent = 0.35f; // e.g., 35% of screen
    
    [Tooltip("Panel fixed height when in Main Menu (-1 for auto size based on contents)")]
    public float panelFixedHeight = 500f;

    [Tooltip("Panel fixed height when in Settings Mode (-1 for auto size based on contents)")]
    public float panelSettingsFixedHeight = 400f;

    [Tooltip("Panel minimum height in pixels (0 for auto size based on contents)")]
    [Range(0f, 1000f)]
    public float panelMinHeight = 0f;

    [Tooltip("Panel border thickness â€” 1px")]
    public int panelBorderThickness = 1;

    [Tooltip("Panel padding top")]
    public float paddingTop = 44f;
    [Tooltip("Panel padding left/right")]
    public float paddingHorizontal = 40f;
    [Tooltip("Panel padding bottom")]
    public float paddingBottom = 48f;

    [Tooltip("Spacing between elements in the panel")]
    public float elementSpacing = 48f;

    // =============================================
    // â” â” â”  SHADOW SYSTEM â” â” â” 
    // =============================================

    [Header("â” â” â”  SHADOW SYSTEM â” â” â” ")]

    [Tooltip("rgba(255,255,255,0.08) â€” Inner highlight")]
    public Color innerHighlight = new Color(1f, 1f, 1f, 0.08f);

    [Tooltip("rgba(255,220,140,0.06) â€” Inner gold glow")]
    public Color innerGoldGlow = new Color(1f, 0.863f, 0.549f, 0.06f);

    // =============================================
    // â” â” â”  BUTTON ROUNDING â” â” â” 
    // =============================================

    [Header("â” â” â”  BUTTON ROUNDING & SIZING â” â” â” ")]

    [Tooltip("Button width in pixels â€” 0 = Full width of panel")]
    public float buttonWidth = 280f;

    [Tooltip("Button height in pixels â€” 64px")]
    public float buttonHeight = 72f;

    [Tooltip("Button corner radius â€” 12px")]
    public int buttonCornerRadius = 12;

    [Tooltip("Exit Button corner radius")]
    public int exitButtonRadius = 12;

    [Tooltip("Button border thickness â€” 1px")]
    public int buttonBorderThickness = 1;

    // =============================================
    // â” â” â”  SETTINGS PANEL â” â” â” 
    // =============================================

    [Header("â” â” â”  SETTINGS PANEL â” â” â” ")]

    [Tooltip("rgba(255,255,255,0.05) â€” Settings background")]
    public Color settingsBackground = new Color(1f, 1f, 1f, 0.05f);

    [Tooltip("rgba(255,255,255,0.1) â€” Settings border")]
    public Color settingsBorder = new Color(1f, 1f, 1f, 0.1f);

    [Tooltip("Settings corner radius â€” 14px")]
    public int settingsRadius = 14;

    [Tooltip("Settings Row Height")]
    public float settingsHeight = 70f;

    [Tooltip("Spacing between Mute, Icon, and Slider")]
    public float settingsSpacing = 12f;

    [Tooltip("Horizontal padding inside the settings row")]
    public int settingsPaddingHorizontal = 18;

    [Tooltip("Vertical padding inside the settings row")]
    public int settingsPaddingVertical = 14;

    [Tooltip("Mute button width inside settings")]
    public float settingsMuteWidth = 90f;

    [Tooltip("Mute button height inside settings")]
    public float settingsMuteHeight = 42f;

    [Header("--- MUTE BUTTON ICON ---")]
    
    [Tooltip("Microphone icon sprite when UNMUTED (normal state)")]
    public Sprite microphoneIconSprite;
    
    [Tooltip("Microphone icon sprite when MUTED (different icon)")]
    public Sprite microphoneIconMutedSprite;
    
    [Tooltip("Microphone icon size")]
    public float microphoneIconSize = 24f;
    
    [Tooltip("Speaker circle background color")]
    public Color microphoneCircleBgColor = new Color(1f, 1f, 1f, 0.08f);

    [Tooltip("Speaker circle border color")]
    public Color microphoneCircleBorderColor = new Color(1f, 1f, 1f, 0.12f);
    
    [Tooltip("Microphone icon normal color (when unmuted)")]
    public Color microphoneNormalColor = Color.white;
    
    [Tooltip("Microphone icon muted color (when muted - red)")]
    public Color microphoneMutedColor = new Color(0.863f, 0.235f, 0.235f, 1f); // Red color
    
    [Tooltip("Show red glow/shadow when muted")]
    public bool showMutedGlow = true;
    
    [Tooltip("Muted glow color (red shadow)")]
    public Color mutedGlowColor = new Color(0.863f, 0.235f, 0.235f, 0.6f); // Red with transparency
    
    [Tooltip("Muted glow size (how much bigger than icon)")]
    public float mutedGlowSize = 1.5f;

    // Track previous values to detect Inspector changes
    private float lastMicrophoneIconSize = -1f;
    private Color lastMicrophoneNormalColor;
    private Color lastMicrophoneMutedColor;
    private Sprite lastMicrophoneIconSprite;
    private Sprite lastMicrophoneIconMutedSprite;
    private bool lastShowMutedGlow;
    private Color lastMutedGlowColor;
    private float lastMutedGlowSize;

    [Tooltip("Slider height inside settings")]
    public float settingsSliderHeight = 42f;

    [Tooltip("Speaker icon size inside settings")]
    public float settingsIconSize = 40f;

    [Header("--- SETTINGS PANEL SIZE ---")]

    [Tooltip("Settings panel width in pixels -- 0 = same as button width")]
    public float settingsWidth = 0f;

    [Tooltip("Settings border thickness in pixels")]
    public int settingsBorderThickness = 1;

    [Header("--- SETTINGS TITLE (separate from main) ---")]

    [Tooltip("Settings panel title text -- separate from main MENU title")]
    public string settingsTitleText = "SETTINGS";

    [Tooltip("Settings title font size")]
    public float settingsTitleFontSize = 42f;

    [Tooltip("Settings title character spacing")]
    public float settingsTitleCharSpacing = 18f;

    [Tooltip("Settings title color -- separate from main title")]
    public Color settingsTitleColor = Color.white;

    [Tooltip("Settings title height")]
    public float settingsTitleHeight = 65f;

    // =============================================
    // â” â” â”  TITLE â” â” â” 
    // =============================================

    [Header("â” â” â”  TITLE â” â” â” ")]

    [Tooltip("Title text â€” Cinzel 700 42px")]
    public string titleText = "MENU";

    [Tooltip("Title Font Asset")]
    public TMPro.TMP_FontAsset titleFont;

    [Tooltip("Title font size")]
    public float titleFontSize = 42f;

    [Tooltip("Letter spacing for Cinzel premium feel")]
    public float titleCharacterSpacing = 18f;

    [Tooltip("Title area height")]
    public float titleHeight = 65f;

    [Tooltip("Space between Title and Buttons")]
    public float spaceAfterTitle = 30f;

    // Separator removed per user request

    [Header("â” â” â”  GLOW COLORS â” â” â” ")]

    [Tooltip("Gold glow for standard buttons")]
    public Color buttonGlowColor = new Color(1f, 0.85f, 0.4f, 1f);

    [Tooltip("Red glow for exit button")]
    public Color exitGlowColor = new Color(1f, 0.35f, 0.3f, 1f);

    // =============================================
    // â” â” â”  ANIMATION (via GlowButtonEffect) â” â” â” 
    // =============================================

    [Header("â” â” â”  ANIMATION CONFIG â” â” â” ")]

    [Tooltip("Hover scale â€” 1.04 = 4% larger")]
    public float hoverScaleValue = 1.04f;

    [Tooltip("Click scale â€” 0.97 = 3% smaller")]
    public float clickScaleValue = 0.97f;

    [Tooltip("Animation duration â€” 0.35s cubic-bezier")]
    public float animDurationValue = 0.35f;

    [Header("â” â” â”  BACK BUTTON â” â” â” ")]

    [Tooltip("Back button diameter in pixels")]
    public float backButtonSize = 48f;

    [Tooltip("Back button corner radius (high = circle)")]
    public int backButtonRadius = 64;

    [Tooltip("Back button Y offset from panel top")]
    public float backButtonTopOffset = 50f;

    [Tooltip("Back button X offset from panel left")]
    public float backButtonLeftOffset = 44f;

    [Header("--- BACK BUTTON TEXT ---")]
    
    [Tooltip("Back button text font size")]
    public float backButtonTextSize = 22f;
    
    [Tooltip("Back button text X offset (horizontal adjustment)")]
    public float backButtonTextOffsetX = 0f;
    
    [Tooltip("Back button text Y offset (vertical adjustment)")]
    public float backButtonTextOffsetY = 0f;

    [Header("â” â” â”  STYLING MODE â” â” â” ")]
    [Tooltip("ØªÙ… Ø¥Ø²Ø§Ù„Ø© Ø§Ù„Ø­Ø°Ù  Ø§Ù„Ø¥Ø¬Ø¨Ø§Ø±ÙŠ Ù„Ù„ØªØ¹Ø¯ÙŠÙ„Ø§Øª Ø§Ù„ÙŠØ¯ÙˆÙŠØ©. Ø§Ù„ÙƒÙˆØ¯ Ø§Ù„Ø¢Ù† ÙŠØ­ØªØ±Ù… ØªØ¹Ø¯ÙŠÙ„Ø§ØªÙƒ Ù ÙŠ Ø§Ù„Ù€ Inspector.")]
    public bool applyColorsInEditor = true;

    // =============================================
    // INTERNALS
    // =============================================

    private PauseMenuManager pauseManager;
    private RectTransform panelRect;
    private GameObject titleObject;
    private GameObject innerHighlightObject;
    private GameObject innerGoldGlowObject;
    private bool styleApplied = false;

    // =============================================
    // LIFECYCLE
    // =============================================

    void Awake()
    {
        pauseManager = GetComponent<PauseMenuManager>();
    }

    void Start()
    {
        if (!Application.isPlaying) return;
        
        // Debug.Log("[STYLE] ========== CinematicPauseStyle.Start() ==========");
        
        if (pauseManager == null || pauseManager.pauseMenuPanel == null)
            return;

        panelRect = pauseManager.pauseMenuPanel.GetComponent<RectTransform>();
        ConfigureCanvasScaler();

        // CRITICAL: Remove old microphone icon from mute button (deprecated location)
        if (pauseManager.muteButton != null)
        {
            Transform oldIcon = pauseManager.muteButton.transform.Find("MicrophoneIcon");
            if (oldIcon != null)
            {
                // Debug.Log("[STYLE] 🗑️ Removing old MicrophoneIcon from mute button");
                Destroy(oldIcon.gameObject);
            }
        }

        // CRITICAL: Temporarily activate the panel so all components (RoundedImage, LayoutGroup, etc.) work
        bool wasActive = pauseManager.pauseMenuPanel.activeSelf;
        pauseManager.pauseMenuPanel.SetActive(true);

        // Apply non-destructive runtime logic
        CreateCinemaOverlay();
        ApplyFullStyle();
        
        // Force layout rebuild while panel is active
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        
        // Restore original state
        pauseManager.pauseMenuPanel.SetActive(wasActive);
        
        pauseManager.CloseSettings();
        styleApplied = true;
    }

    /// <summary>
    /// Detect when the panel becomes active and refresh all visual components.
    /// CinematicPauseStyle lives on the PARENT (same as PauseMenuManager),
    /// so OnEnable won't fire when the child panel opens — we use LateUpdate instead.
    /// </summary>
    private bool wasPanelActive = false;
    private bool lastSettingsMode = false;

    void LateUpdate()
    {
        if (!Application.isPlaying || !styleApplied) return;
        if (pauseManager == null || pauseManager.pauseMenuPanel == null) return;

        bool isActive = pauseManager.pauseMenuPanel.activeSelf;
        if (isActive && !wasPanelActive)
        {
            // Panel just became active — refresh rounded corners and layout
            if (panelRect != null)
            {
                RoundedImage[] allRounded = panelRect.GetComponentsInChildren<RoundedImage>(true);
                foreach (var ri in allRounded)
                {
                    ri.Refresh();
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
            }
        }
        
        // CRITICAL FIX: Forcefully apply radii continuously in Play Mode
        // to ensure they update immediately when changed in the Inspector.
        if (isActive)
        {
            if (pauseManager.exitButton != null)
            {
                RoundedImage exitRounded = pauseManager.exitButton.GetComponent<RoundedImage>();
                if (exitRounded != null)
                {
                    if (exitRounded.cornerRadius != exitButtonRadius)
                    {
                        // Debug.Log($"[STYLE] LateUpdate: Changing Exit Button Radius from {exitRounded.cornerRadius} to {exitButtonRadius}");
                        exitRounded.cornerRadius = exitButtonRadius;
                        exitRounded.Refresh();
                    }
                }
            }
            if (pauseManager.menuButton != null)
            {
                RoundedImage menuRounded = pauseManager.menuButton.GetComponent<RoundedImage>();
                if (menuRounded != null && menuRounded.cornerRadius != buttonCornerRadius)
                {
                    menuRounded.cornerRadius = buttonCornerRadius;
                    menuRounded.Refresh();
                }
            }
            if (pauseManager.settingsButton != null)
            {
                RoundedImage settingsRounded = pauseManager.settingsButton.GetComponent<RoundedImage>();
                if (settingsRounded != null && settingsRounded.cornerRadius != buttonCornerRadius)
                {
                    settingsRounded.cornerRadius = buttonCornerRadius;
                    settingsRounded.Refresh();
                }
            }
            
            // CRITICAL FIX: Update back button size, radius, position, and text in real-time
            if (pauseManager.backButton != null)
            {
                // Update corner radius
                RoundedImage backRounded = pauseManager.backButton.GetComponent<RoundedImage>();
                if (backRounded != null && backRounded.cornerRadius != backButtonRadius)
                {
                    backRounded.cornerRadius = backButtonRadius;
                    backRounded.Refresh();
                }
                
                // Update button size and position
                RectTransform backRT = pauseManager.backButton.GetComponent<RectTransform>();
                if (backRT != null)
                {
                    // Update size
                    if (backRT.rect.width != backButtonSize || backRT.rect.height != backButtonSize)
                    {
                        backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backButtonSize);
                        backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backButtonSize);
                    }
                    
                    // Update position
                    Vector2 targetPos = new Vector2(backButtonLeftOffset, -backButtonTopOffset);
                    if (backRT.anchoredPosition != targetPos)
                    {
                        backRT.anchoredPosition = targetPos;
                    }
                }
                
                // Update text size and position
                TextMeshProUGUI tmpText = pauseManager.backButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    if (tmpText.fontSize != backButtonTextSize)
                    {
                        tmpText.fontSize = backButtonTextSize;
                    }
                    
                    RectTransform textRT = tmpText.GetComponent<RectTransform>();
                    if (textRT != null)
                    {
                        Vector2 targetMin = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                        Vector2 targetMax = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                        if (textRT.offsetMin != targetMin || textRT.offsetMax != targetMax)
                        {
                            textRT.offsetMin = targetMin;
                            textRT.offsetMax = targetMax;
                        }
                    }
                }
            }
            
            // CRITICAL FIX: Update slider thumb (gold circle) size in real-time
            if (pauseManager.volumeSlider != null)
            {
                Slider slider = pauseManager.volumeSlider;
                
                // Update thumb size
                if (slider.handleRect != null)
                {
                    if (slider.handleRect.rect.width != sliderThumbSize || slider.handleRect.rect.height != sliderThumbSize)
                    {
                        slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderThumbSize);
                        slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sliderThumbSize);
                        
                        // Also update the LayoutElement
                        LayoutElement handleLE = slider.handleRect.GetComponent<LayoutElement>();
                        if (handleLE != null)
                        {
                            handleLE.preferredWidth = sliderThumbSize;
                            handleLE.preferredHeight = sliderThumbSize;
                        }
                    }
                    
                    // Update thumb glow size
                    Transform glowTrans = slider.handleRect.Find("ThumbGlow");
                    if (glowTrans != null)
                    {
                        RectTransform glowRT = glowTrans.GetComponent<RectTransform>();
                        if (glowRT != null)
                        {
                            float glowPadding = sliderThumbGlowSpread * 2f;
                            Vector2 targetMin = new Vector2(-glowPadding, -glowPadding);
                            Vector2 targetMax = new Vector2(glowPadding, glowPadding);
                            if (glowRT.offsetMin != targetMin || glowRT.offsetMax != targetMax)
                            {
                                glowRT.offsetMin = targetMin;
                                glowRT.offsetMax = targetMax;
                            }
                        }
                    }
                }
                
                // Update track height
                if (slider.fillRect != null)
                {
                    LayoutElement fillLE = slider.fillRect.GetComponent<LayoutElement>();
                    if (fillLE != null && fillLE.preferredHeight != sliderTrackHeight)
                    {
                        fillLE.preferredHeight = sliderTrackHeight;
                    }
                }
            }
            
            // CRITICAL FIX: Update speaker icon size in real-time
            if (pauseManager.settingsRow != null)
            {
                Transform iconTrans = pauseManager.settingsRow.transform.Find("SpeakerIcon");
                if (iconTrans != null)
                {
                    LayoutElement iconLE = iconTrans.GetComponent<LayoutElement>();
                    if (iconLE != null)
                    {
                        if (iconLE.preferredWidth != settingsIconSize || iconLE.preferredHeight != settingsIconSize)
                        {
                            iconLE.preferredWidth = settingsIconSize;
                            iconLE.preferredHeight = settingsIconSize;
                        }
                    }
                }
            }
            
        }
        
        // CRITICAL FIX: Recreate microphone icon if Inspector values changed
        // This ensures all style changes apply immediately
        if (pauseManager.settingsRow != null)
        {
            Transform speakerIconTrans = pauseManager.settingsRow.transform.Find("SpeakerIcon");
            if (speakerIconTrans != null)
            {
                bool needsRecreate = false;
                
                // Check if any microphone icon property changed
                if (lastMicrophoneIconSize != microphoneIconSize ||
                    lastMicrophoneNormalColor != microphoneNormalColor ||
                    lastMicrophoneMutedColor != microphoneMutedColor ||
                    lastMicrophoneIconSprite != microphoneIconSprite ||
                    lastMicrophoneIconMutedSprite != microphoneIconMutedSprite ||
                    lastShowMutedGlow != showMutedGlow ||
                    lastMutedGlowColor != mutedGlowColor ||
                    lastMutedGlowSize != mutedGlowSize)
                {
                    needsRecreate = true;
                    lastMicrophoneIconSize = microphoneIconSize;
                    lastMicrophoneNormalColor = microphoneNormalColor;
                    lastMicrophoneMutedColor = microphoneMutedColor;
                    lastMicrophoneIconSprite = microphoneIconSprite;
                    lastMicrophoneIconMutedSprite = microphoneIconMutedSprite;
                    lastShowMutedGlow = showMutedGlow;
                    lastMutedGlowColor = mutedGlowColor;
                    lastMutedGlowSize = mutedGlowSize;
                }
                
                // Recreate icon if needed
                if (needsRecreate)
                {
                    // Debug.Log("[STYLE] 🔄 Inspector values changed - Recreating microphone icon...");
                    CreateMicrophoneIconInSpeakerCircle(speakerIconTrans.gameObject);
                }
                
                
                // Always update color based on mute state
                // CinematicPauseStyle is the sole owner of icon visuals
                {
                    Transform micIconTrans = speakerIconTrans.Find("MicrophoneIcon");
                    if (micIconTrans != null)
                    {
                        bool isMuted = pauseManager.IsMuted;
                        Color targetColor = isMuted ? microphoneMutedColor : microphoneNormalColor;
                        
                        // Update Image component (sprite-based icon)
                        Image micImg = micIconTrans.GetComponent<Image>();
                        if (micImg != null)
                        {
                            micImg.color = targetColor;
                            
                            // Update sprite based on mute state
                            Sprite targetSprite = (isMuted && microphoneIconMutedSprite != null) ? microphoneIconMutedSprite : microphoneIconSprite;
                            if (micImg.sprite != targetSprite)
                            {
                                micImg.sprite = targetSprite;
                            }
                        }
                        
                        // Update glow visibility and color
                        Transform glowTrans = speakerIconTrans.Find("MicrophoneGlow");
                        if (glowTrans != null)
                        {
                            bool shouldShowGlow = showMutedGlow && isMuted;
                            if (glowTrans.gameObject.activeSelf != shouldShowGlow)
                            {
                                glowTrans.gameObject.SetActive(shouldShowGlow);
                            }
                            
                            if (shouldShowGlow)
                            {
                                Image glowImg = glowTrans.GetComponent<Image>();
                                if (glowImg != null)
                                {
                                    glowImg.color = mutedGlowColor;
                                    
                                    // Update glow sprite to match icon
                                    Sprite glowSprite = (isMuted && microphoneIconMutedSprite != null) ? microphoneIconMutedSprite : microphoneIconSprite;
                                    if (glowImg.sprite != glowSprite)
                                    {
                                        glowImg.sprite = glowSprite;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        bool currentSettingsMode = isSettingsMode();
        if (isActive && currentSettingsMode != lastSettingsMode)
        {
            if (panelRect != null)
            {
                LayoutElement panelLE = panelRect.GetComponent<LayoutElement>();
                if (panelLE != null)
                {
                    panelLE.preferredHeight = currentSettingsMode ? panelSettingsFixedHeight : panelFixedHeight;
                }
            }
        }
        lastSettingsMode = currentSettingsMode;
        
        wasPanelActive = isActive;
    }

    void OnValidate()
    {
        if (applyColorsInEditor)
        {
            pauseManager = GetComponent<PauseMenuManager>();
            if (pauseManager != null && pauseManager.pauseMenuPanel != null)
            {
                panelRect = pauseManager.pauseMenuPanel.GetComponent<RectTransform>();
                UpdateExistingElementsStyle();
            }
        }
    }

    private void UpdateExistingElementsStyle()
    {
        if (pauseManager == null || panelRect == null) return;

        // =============================================
        // 1. PANEL — لون + بوردر + زوايا
        // =============================================
        Image panelImage = panelRect.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = Color.white; // RoundedImage.fillColor handles the actual color
        }

        RoundedImage panelRounded = panelRect.GetComponent<RoundedImage>();
        if (panelRounded != null)
        {
            panelRounded.cornerRadius = panelCornerRadius;
            panelRounded.borderThickness = panelBorderThickness;
            panelRounded.borderColor = glassBorder;
            panelRounded.fillColor = glassBackground;
            panelRounded.Refresh();
        }

        // =============================================
        // 2. PANEL SIZE — عرض وارتفاع
        float refWidth = 1920f;
        float desiredWidth = Mathf.Clamp(refWidth * panelWidthPercent, 300f, 680f);
        
        LayoutElement panelLE = panelRect.GetComponent<LayoutElement>();
        if (panelLE == null) return; // Don't AddComponent in OnValidate — it's forbidden by Unity
        panelLE.preferredWidth = desiredWidth;

        float currentHeight = isSettingsMode() ? panelSettingsFixedHeight : panelFixedHeight;
        if (currentHeight > 0f)
        {
            ContentSizeFitter csf = panelRect.GetComponent<ContentSizeFitter>();
            if (csf != null) csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            panelLE.preferredHeight = currentHeight;
        }
        else
        {
            panelLE.preferredHeight = -1f;
        }

        // =============================================
        // 3. PADDING + SPACING
        // =============================================
        VerticalLayoutGroup layoutGroup = panelRect.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.padding = new RectOffset(
                (int)paddingHorizontal, (int)paddingHorizontal,
                (int)paddingTop, (int)paddingBottom
            );
            layoutGroup.spacing = elementSpacing;
        }

        // =============================================
        // 4. SHADOW (Removed per user request)
        // =============================================
        Transform shadowTrans = panelRect.Find("PanelShadow");
        if (shadowTrans != null)
        {
            shadowTrans.gameObject.SetActive(false);
            // NOTE: Do NOT call Destroy() here — this method is called from OnValidate()
            // and Unity forbids Destroy/AddComponent/new GameObject inside OnValidate.
        }

        // =============================================
        // 4.5 OVERLAY
        // =============================================
        if (pauseManager.cinemaOverlay != null)
        {
            Image overlayImg = pauseManager.cinemaOverlay.GetComponent<Image>();
            if (overlayImg != null) overlayImg.color = overlayDarkness;
        }
        else
        {
            Transform overlayTrans = panelRect.parent != null ? panelRect.parent.Find("CinemaOverlay") : null;
            if (overlayTrans != null)
            {
                Image overlayImg = overlayTrans.GetComponent<Image>();
                if (overlayImg != null) overlayImg.color = overlayDarkness;
            }
        }

        // =============================================
        // 4.6 INNER HIGHLIGHT
        // =============================================
        Transform hlTrans = panelRect.Find("InnerHighlight");
        if (hlTrans != null)
        {
            RoundedImage hlRounded = hlTrans.GetComponent<RoundedImage>();
            if (hlRounded != null)
            {
                hlRounded.cornerRadius = Mathf.Max(0, panelCornerRadius - 1);
                hlRounded.borderColor = innerHighlight;
                hlRounded.Refresh();
            }
        }

        // =============================================
        // 4.7 INNER GOLD GLOW
        // =============================================
        Transform glTrans = panelRect.Find("InnerGoldGlow");
        if (glTrans != null)
        {
            RoundedImage glRounded = glTrans.GetComponent<RoundedImage>();
            if (glRounded != null)
            {
                glRounded.cornerRadius = panelCornerRadius;
                glRounded.borderColor = innerGoldGlow;
                glRounded.Refresh();
            }
        }

        // =============================================
        // 5. TITLE
        // =============================================
        Transform titleTrans = panelRect.Find("CinemaTitle");
        if (titleTrans != null)
        {
            TMPro.TextMeshProUGUI titleTMP = titleTrans.GetComponent<TMPro.TextMeshProUGUI>();
            if (titleTMP != null)
            {
                titleTMP.text = titleText;
                titleTMP.color = mainTextColor;
                titleTMP.fontSizeMax = titleFontSize; // CRITICAL FIX: Update fontSizeMax since auto-sizing is on
                titleTMP.fontSize = titleFontSize;
                titleTMP.characterSpacing = titleCharacterSpacing;
                if (titleFont != null) titleTMP.font = titleFont;
            }
            LayoutElement titleLE = titleTrans.GetComponent<LayoutElement>();
            if (titleLE != null) titleLE.preferredHeight = titleHeight;
        }

        // =============================================
        // 6. SEPARATOR (Removed per user request) & SPACER
        // =============================================
        Transform sepTrans = panelRect.Find("CinemaSeparator");
        if (sepTrans != null)
        {
            sepTrans.gameObject.SetActive(false);
            // NOTE: Do NOT call Destroy() here — OnValidate restriction.
        }

        Transform spacerTrans = panelRect.Find("CinemaTitleSpacer");
        // NOTE: Do NOT create new GameObjects here — OnValidate restriction.
        // The spacer is created in Start() → ApplyFullStyle(). Here we only update it if it exists.
        
        if (spacerTrans != null)
        {
            LayoutElement spacerLE = spacerTrans.GetComponent<LayoutElement>();
            if (spacerLE != null)
            {
                spacerLE.preferredHeight = spaceAfterTitle;
                spacerLE.flexibleHeight = 0f;
                spacerLE.flexibleWidth = 1f;
            }
        }

        // =============================================
        // 7. BUTTONS & SETTINGS ROW
        // =============================================
        UpdateButtonStyle(pauseManager.menuButton, false);
        UpdateButtonStyle(pauseManager.settingsButton, false);
        UpdateButtonStyle(pauseManager.exitButton, true);
        if (pauseManager.backButton != null) UpdateButtonStyle(pauseManager.backButton, false);

        // Fix Mute Button: It shouldn't look like a standard button
        if (pauseManager.muteButton != null)
        {
            Image muteImg = pauseManager.muteButton.GetComponent<Image>();
            if (muteImg != null) muteImg.color = Color.clear;
            
            RoundedImage muteRounded = pauseManager.muteButton.GetComponent<RoundedImage>();
            if (muteRounded != null)
            {
                muteRounded.fillColor = Color.clear;
                muteRounded.borderColor = Color.clear;
                muteRounded.Refresh();
            }
        }

        // Update Settings Row Layout
        Transform settingsRowTrans = panelRect.Find("SettingsRow");
        GameObject sRow = settingsRowTrans != null ? settingsRowTrans.gameObject : (pauseManager.settingsRow != null ? pauseManager.settingsRow : null);
        
        if (sRow != null)
        {
            LayoutElement rowLE = sRow.GetComponent<LayoutElement>();
            if (rowLE != null)
            {
                rowLE.preferredWidth = settingsWidth > 0f ? settingsWidth : buttonWidth;
                rowLE.preferredHeight = settingsHeight;
                rowLE.flexibleWidth = 0f;
            }

            HorizontalLayoutGroup hlg = sRow.GetComponent<HorizontalLayoutGroup>();
            if (hlg != null)
            {
                hlg.padding = new RectOffset(settingsPaddingHorizontal, settingsPaddingHorizontal, settingsPaddingVertical, settingsPaddingVertical);
                hlg.spacing = settingsSpacing;
            }
            
            RectTransform rowRT = sRow.GetComponent<RectTransform>();
            if (rowRT != null) LayoutRebuilder.MarkLayoutForRebuild(rowRT);
        }

        // Update Mute Button
        if (pauseManager.muteButton != null)
        {
            LayoutElement muteLE = pauseManager.muteButton.GetComponent<LayoutElement>();
            if (muteLE != null)
            {
                muteLE.preferredWidth = settingsMuteWidth;
                muteLE.preferredHeight = settingsMuteHeight;
            }
        }
        
        // Update Slider Height
        if (pauseManager.volumeSlider != null)
        {
            LayoutElement sliderLE = pauseManager.volumeSlider.GetComponent<LayoutElement>();
            if (sliderLE != null) sliderLE.preferredHeight = settingsSliderHeight;
        }
        
        // Update Icon Size
        if (sRow != null)
        {
            Transform existingIcon = sRow.transform.Find("SpeakerIcon");
            if (existingIcon != null)
            {
                LayoutElement iconLE = existingIcon.GetComponent<LayoutElement>();
                if (iconLE != null)
                {
                    iconLE.preferredWidth = settingsIconSize;
                    iconLE.preferredHeight = settingsIconSize;
                }

                RoundedImage iconRounded = existingIcon.GetComponent<RoundedImage>();
                if (iconRounded != null)
                {
                    iconRounded.fillColor = microphoneCircleBgColor;
                    iconRounded.borderColor = microphoneCircleBorderColor;
                    iconRounded.Refresh();
                }
            }
        }

        // =============================================
        // 7.5 SLIDER COLORS & SIZING
        // =============================================
        if (pauseManager.volumeSlider != null)
        {
            Slider slider = pauseManager.volumeSlider;

            // Fill
            if (slider.fillRect != null)
            {
                RoundedImage fillRounded = slider.fillRect.GetComponent<RoundedImage>();
                if (fillRounded != null)
                {
                    fillRounded.cornerRadius = sliderTrackRadius;
                    fillRounded.fillColor = sliderFill;
                    fillRounded.Refresh();
                }
                LayoutElement fillLE = slider.fillRect.GetComponent<LayoutElement>();
                if (fillLE != null) fillLE.preferredHeight = sliderTrackHeight;
            }

            // Background (empty track)
            Transform sliderBgTransform = slider.transform.Find("Background");
            if (sliderBgTransform != null)
            {
                RoundedImage bgRounded = sliderBgTransform.GetComponent<RoundedImage>();
                if (bgRounded != null)
                {
                    bgRounded.cornerRadius = sliderTrackRadius;
                    bgRounded.fillColor = sliderEmpty;
                    bgRounded.Refresh();
                }
            }

            // Handle (thumb)
            if (slider.handleRect != null)
            {
                RoundedImage handleRounded = slider.handleRect.GetComponent<RoundedImage>();
                if (handleRounded != null)
                {
                    handleRounded.fillColor = primaryAccent;
                    handleRounded.Refresh();
                }
                
                LayoutElement handleLE = slider.handleRect.GetComponent<LayoutElement>();
                if (handleLE == null) return; // Don't AddComponent in OnValidate
                handleLE.preferredWidth = sliderThumbSize;
                handleLE.preferredHeight = sliderThumbSize;
                
                // Update thumb glow size
                Transform glowTrans = slider.handleRect.Find("ThumbGlow");
                if (glowTrans != null)
                {
                    RectTransform glowRT = glowTrans.GetComponent<RectTransform>();
                    if (glowRT != null)
                    {
                        float glowPadding = sliderThumbGlowSpread * 2f;
                        glowRT.offsetMin = new Vector2(-glowPadding, -glowPadding);
                        glowRT.offsetMax = new Vector2(glowPadding, glowPadding);
                    }
                }
            }
        }

        // =============================================
        // 7.6 SETTINGS ROW STYLING
        // =============================================
        if (sRow != null)
        {
            RoundedImage rowRounded = sRow.GetComponent<RoundedImage>();
            if (rowRounded != null)
            {
                rowRounded.cornerRadius = settingsRadius;
                rowRounded.borderThickness = settingsBorderThickness;
                rowRounded.borderColor = settingsBorder;
                rowRounded.fillColor = settingsBackground;
                rowRounded.Refresh();
            }

            LayoutElement rowLE = sRow.GetComponent<LayoutElement>();
            if (rowLE != null)
            {
                rowLE.preferredHeight = settingsHeight;
                if (settingsWidth > 0f)
                {
                    rowLE.preferredWidth = settingsWidth;
                    rowLE.flexibleWidth = 0f;
                }
                else
                {
                    rowLE.preferredWidth = -1f;
                    rowLE.flexibleWidth = 1f;
                }
            }
        }

        // =============================================
        // 7.6.1 SETTINGS TITLE (independent from main title)
        // =============================================
        Transform settingsTitleTrans = panelRect.Find("CinemaTitle");
        if (settingsTitleTrans != null && isSettingsMode())
        {
            TMPro.TextMeshProUGUI settingsTitleTMP = settingsTitleTrans.GetComponent<TMPro.TextMeshProUGUI>();
            if (settingsTitleTMP != null)
            {
                settingsTitleTMP.text = settingsTitleText;
                settingsTitleTMP.color = settingsTitleColor;
                settingsTitleTMP.fontSize = settingsTitleFontSize;
                settingsTitleTMP.fontSizeMax = settingsTitleFontSize;
                settingsTitleTMP.characterSpacing = settingsTitleCharSpacing;
            }
            LayoutElement settingsTitleLE = settingsTitleTrans.GetComponent<LayoutElement>();
            if (settingsTitleLE != null) settingsTitleLE.preferredHeight = settingsTitleHeight;
        }

        // =============================================
        // 7.7 BACK BUTTON
        // =============================================
        if (pauseManager.backButton != null)
        {
            RoundedImage backRounded = pauseManager.backButton.GetComponent<RoundedImage>();
            if (backRounded != null)
            {
                backRounded.cornerRadius = backButtonRadius;
                backRounded.fillColor = buttonNormal;
                backRounded.borderColor = buttonBorder;
                backRounded.borderThickness = buttonBorderThickness;
                backRounded.Refresh();
            }

            RectTransform backRT = pauseManager.backButton.GetComponent<RectTransform>();
            if (backRT != null)
            {
                // Set anchors and pivot in Edit Mode too so it moves to the top-left corner immediately
                backRT.anchorMin = new Vector2(0f, 1f);
                backRT.anchorMax = new Vector2(0f, 1f);
                backRT.pivot = new Vector2(0.5f, 0.5f);
                backRT.anchoredPosition = new Vector2(backButtonLeftOffset, -backButtonTopOffset);
                
                // CRITICAL FIX: Update button size in Edit Mode
                backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backButtonSize);
                backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backButtonSize);
                
                LayoutElement backLE = backRT.GetComponent<LayoutElement>();
                if (backLE == null) return; // Don't AddComponent in OnValidate
                backLE.ignoreLayout = true; // VERY IMPORTANT! Prevents it from breaking or being broken by the VerticalLayoutGroup
                backLE.preferredWidth = backButtonSize;
                backLE.preferredHeight = backButtonSize;
            }
            
            // CRITICAL FIX: Update text size and position in Edit Mode
            TextMeshProUGUI tmpText = pauseManager.backButton.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.fontSize = backButtonTextSize;
                tmpText.raycastTarget = false;
                
                RectTransform textRT = tmpText.GetComponent<RectTransform>();
                if (textRT != null)
                {
                    textRT.anchorMin = Vector2.zero;
                    textRT.anchorMax = Vector2.one;
                    textRT.offsetMin = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                    textRT.offsetMax = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                    textRT.pivot = new Vector2(0.5f, 0.5f);
                }
            }
        }

        // =============================================
        // 7.8 ANIMATION CONFIG → GlowButtonEffect
        // =============================================
        UpdateGlowEffect(pauseManager.menuButton, false);
        UpdateGlowEffect(pauseManager.settingsButton, false);
        UpdateGlowEffect(pauseManager.exitButton, true);
        UpdateGlowEffect(pauseManager.backButton, false);

        // =============================================
        // 8. REFRESH ALL ROUNDED IMAGES
        // =============================================
        RoundedImage[] allRounded = panelRect.GetComponentsInChildren<RoundedImage>(true);
        foreach (var ri in allRounded) ri.Refresh();

        // =============================================
        // 9. FORCE IMMEDIATE VISUAL UPDATE
        // =============================================
        // NOTE: Canvas.ForceUpdateCanvases() and ForceRebuildLayoutImmediate() are FORBIDDEN inside OnValidate.
        // They throw exceptions that completely break the Inspector update loop.
        LayoutRebuilder.MarkLayoutForRebuild(panelRect);
    }

    /// <summary>
    /// تحديث لون الزرار + بوردر من الـ Inspector بدون AddComponent
    /// </summary>
    private void UpdateButtonStyle(Button button, bool isExitButton)
    {
        if (button == null) return;

        // Only get existing LayoutElement — do NOT AddComponent here (OnValidate restriction)
        LayoutElement le = button.GetComponent<LayoutElement>();
        
        if (le != null)
        {
            le.preferredHeight = buttonHeight;
            if (buttonWidth > 0f)
            {
                le.preferredWidth = buttonWidth;
                le.flexibleWidth = 0f;
            }
            else
            {
                le.preferredWidth = -1f;
                le.flexibleWidth = 1f;
            }
            le.flexibleHeight = 0f;
        }

        // ColorBlock
        ColorBlock cb = button.colors;
        cb.fadeDuration = 0.15f;
        if (isExitButton)
        {
            cb.normalColor = exitButtonBase;
            cb.highlightedColor = exitButtonHover;
            cb.pressedColor = exitPressed;
            cb.selectedColor = exitButtonBase;
        }
        else
        {
            cb.normalColor = buttonNormal;
            cb.highlightedColor = hoverBackground;
            cb.pressedColor = buttonPressed;
            cb.selectedColor = buttonNormal;
        }
        cb.colorMultiplier = 1f;
        button.colors = cb;

        // RoundedImage border (if exists)
        RoundedImage btnRounded = button.GetComponent<RoundedImage>();
        if (btnRounded != null)
        {
            btnRounded.cornerRadius = isExitButton ? exitButtonRadius : buttonCornerRadius;
            btnRounded.fillColor = Color.white;
            if (isExitButton)
            {
                btnRounded.borderThickness = buttonBorderThickness;
                btnRounded.borderColor = new Color(1f, 0.3f, 0.3f, 0.2f);
            }
            else
            {
                btnRounded.borderThickness = buttonBorderThickness;
                btnRounded.borderColor = buttonBorder;
            }
            btnRounded.Refresh();
        }

        // Text color
        TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>(false);
        if (tmpText != null) tmpText.color = buttonTextColor;
    }

    /// <summary>
    /// Update GlowButtonEffect properties from Inspector values
    /// </summary>
    private void UpdateGlowEffect(Button button, bool isExitButton)
    {
        if (button == null) return;
        GlowButtonEffect effect = button.GetComponent<GlowButtonEffect>();
        if (effect == null) return;

        effect.hoverScale = hoverScaleValue;
        effect.clickScale = clickScaleValue;
        effect.animDuration = animDurationValue;
        effect.normalTextColor = buttonTextColor;
        effect.hoverTextColor = isExitButton ? Color.white : hoverTextColor;
        effect.glowColor = isExitButton ? exitGlowColor : buttonGlowColor;
        effect.buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>(false);
    }

    /// <summary>
    /// Check if the settings panel is currently active
    /// </summary>
    private bool isSettingsMode()
    {
        if (pauseManager == null) return false;
        if (pauseManager.settingsRow != null && pauseManager.settingsRow.activeSelf) return true;
        return false;
    }

    /// <summary>
    /// Switch the title to settings mode (called by PauseMenuManager.OpenSettings)
    /// </summary>
    public void ApplySettingsTitle()
    {
        if (panelRect == null) return;
        Transform titleTrans = panelRect.Find("CinemaTitle");
        if (titleTrans == null) return;

        TMPro.TextMeshProUGUI titleTMP = titleTrans.GetComponent<TMPro.TextMeshProUGUI>();
        if (titleTMP != null)
        {
            titleTMP.text = settingsTitleText;
            titleTMP.color = settingsTitleColor;
            titleTMP.fontSize = settingsTitleFontSize;
            titleTMP.fontSizeMax = settingsTitleFontSize;
            titleTMP.characterSpacing = settingsTitleCharSpacing;
        }
        LayoutElement titleLE = titleTrans.GetComponent<LayoutElement>();
        if (titleLE != null) titleLE.preferredHeight = settingsTitleHeight;
    }

    /// <summary>
    /// Restore the title to main menu mode (called by PauseMenuManager.CloseSettings)
    /// </summary>
    public void ApplyMainTitle()
    {
        if (panelRect == null) return;
        Transform titleTrans = panelRect.Find("CinemaTitle");
        if (titleTrans == null) return;

        TMPro.TextMeshProUGUI titleTMP = titleTrans.GetComponent<TMPro.TextMeshProUGUI>();
        if (titleTMP != null)
        {
            titleTMP.text = titleText;
            titleTMP.color = mainTextColor;
            titleTMP.fontSize = titleFontSize;
            titleTMP.fontSizeMax = titleFontSize;
            titleTMP.characterSpacing = titleCharacterSpacing;
            if (titleFont != null) titleTMP.font = titleFont;
        }
        LayoutElement titleLE = titleTrans.GetComponent<LayoutElement>();
        if (titleLE != null) titleLE.preferredHeight = titleHeight;
    }

    void OnDestroy()
    {
        // No runtime-generated textures to clean up
    }

    // =============================================
    // PUBLIC API
    // =============================================

    private void ConfigureCanvasScaler()
    {
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null) return;

        CanvasScaler canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = parentCanvas.gameObject.AddComponent<CanvasScaler>();
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
    }

    public void ApplyFullStyle()
    {
        // Debug.Log("[STYLE] ========== ApplyFullStyle() START ==========");
        
        // STEP 1: Layout group FIRST — must exist before adding children
        // Debug.Log("[STYLE] Step 1: ConfigurePanelPadding()");
        ConfigurePanelPadding();
        
        // STEP 2: Panel visual style (glass background + rounded corners)
        // Debug.Log("[STYLE] Step 2: StylePanel()");
        StylePanel();
        
        // STEP 3: Decorative layers (shadow, highlights)
        // Panel Shadow (Removed per user request to remove duplicate panel effect)
        // CreatePanelShadow();
        
        // Debug.Log("[STYLE] Step 4: CreateInnerHighlight()");
        CreateInnerHighlight();
        
        // Debug.Log("[STYLE] Step 5: CreateInnerGoldGlow()");
        CreateInnerGoldGlow();
        
        // STEP 4: Content elements
        // Debug.Log("[STYLE] Step 6: CreateTitleIfNeeded()");
        CreateTitleIfNeeded();
        
        // Debug.Log("[STYLE] Step 8: CreateSettingsRow()");
        CreateSettingsRow();

        // STEP 5: Sibling ordering for correct visual stacking
        // Debug.Log("[STYLE] Step 9: Setting sibling indices...");
        if (titleObject != null) titleObject.transform.SetAsLastSibling();
        
        Transform spacerObj = panelRect.Find("CinemaTitleSpacer");
        if (spacerObj != null) spacerObj.SetAsLastSibling();
        

        if (pauseManager.menuButton != null) pauseManager.menuButton.transform.SetAsLastSibling();
        if (pauseManager.settingsButton != null) pauseManager.settingsButton.transform.SetAsLastSibling();
        if (pauseManager.settingsRow != null) pauseManager.settingsRow.transform.SetAsLastSibling();
        if (pauseManager.exitButton != null) pauseManager.exitButton.transform.SetAsLastSibling();
        if (pauseManager.backButton != null) pauseManager.backButton.transform.SetAsLastSibling();

        // STEP 6: Style buttons
        // Debug.Log("[STYLE] Step 10: Styling buttons...");
        StyleButton(pauseManager.menuButton, false, null);
        StyleButton(pauseManager.settingsButton, false, null);
        
        // Fix Mute Button: It shouldn't look like a standard button
        if (pauseManager.muteButton != null)
        {
            // Strip generic button scripts (glass background, glow)
            RoundedImage ri = pauseManager.muteButton.GetComponent<RoundedImage>();
            if (ri != null) DestroyImmediate(ri);
            GlowButtonEffect gbe = pauseManager.muteButton.GetComponent<GlowButtonEffect>();
            if (gbe != null) DestroyImmediate(gbe);
            UnityEngine.UI.Image img = pauseManager.muteButton.GetComponent<UnityEngine.UI.Image>();
            if (img != null) img.enabled = false;

            // Restyle text for Mute
            StyleButtonText(pauseManager.muteButton, false, "MUTE");
        }

        // Back button â€” circular, positioned absolutely
        StyleBackButton(pauseManager.backButton);

        // Exit button â€” red accent
        StyleButton(pauseManager.exitButton, true, null);

        // Debug.Log("[STYLE] Step 11: StyleSlider()");
        StyleSlider();

        // STEP 7: Panel sizing (anchors, width, height)
        // Debug.Log("[STYLE] Step 12: ConfigurePanelAnchors()");
        ConfigurePanelAnchors();
        
        // Debug.Log("[STYLE] Full style applied successfully.");
    }

    // =============================================
    // CINEMA OVERLAY â€” brightness 75%
    // =============================================

    private void CreateCinemaOverlay()
    {
        if (pauseManager.pauseMenuPanel == null) return;

        Transform existingOverlay = pauseManager.pauseMenuPanel.transform.parent?.Find("CinemaOverlay");
        if (existingOverlay != null)
        {
            pauseManager.cinemaOverlay = existingOverlay.gameObject;
            Image existingImg = existingOverlay.GetComponent<Image>();
            if (existingImg != null) existingImg.color = overlayDarkness;
            return;
        }

        GameObject overlay = new GameObject("CinemaOverlay");
        overlay.transform.SetParent(pauseManager.pauseMenuPanel.transform.parent, false);

        int panelIndex = pauseManager.pauseMenuPanel.transform.GetSiblingIndex();
        overlay.transform.SetSiblingIndex(panelIndex);

        RectTransform overlayRT = overlay.AddComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;

        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = overlayDarkness;
        overlayImg.raycastTarget = true;

        overlay.SetActive(false);
        pauseManager.cinemaOverlay = overlay;
    }

    // =============================================
    // PANEL GLASS STYLING
    // =============================================

    private void StylePanel()
    {
        if (panelRect == null) return;

        RoundedImage roundedImg = panelRect.GetComponent<RoundedImage>();
        if (roundedImg == null)
        {
            roundedImg = panelRect.gameObject.AddComponent<RoundedImage>();
            // Debug.Log("[STYLE] âž• Added RoundedImage component to panel");
        }

        roundedImg.cornerRadius = panelCornerRadius;
        roundedImg.borderThickness = panelBorderThickness;
        roundedImg.borderColor = glassBorder;
        roundedImg.fillColor = glassBackground;

        Image panelImage = panelRect.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.sprite = null;
            panelImage.type = Image.Type.Simple;
            // Use Color.white here — RoundedImage.fillColor already holds glassBackground.
            // RoundedImage multiplies fillColor * Image.color, so using glassBackground 
            // in both would result in glassBackground squared (darker than intended).
            panelImage.color = Color.white;
        }

        // If the user used a Button for the panel (to block raycasts or close menu),
        // we must disable its transition so the panel doesn't change color when hovered/clicked.
        Selectable panelSelectable = panelRect.GetComponent<Selectable>();
        if (panelSelectable != null)
        {
            panelSelectable.transition = Selectable.Transition.None;
        }

        // If GlowButtonEffect was manually added to the panel, destroy it.
        GlowButtonEffect panelGlow = panelRect.GetComponent<GlowButtonEffect>();
        if (panelGlow != null)
        {
            if (Application.isPlaying) Destroy(panelGlow);
            else DestroyImmediate(panelGlow);
        }

        // Force refresh to ensure sprite is generated
        roundedImg.Refresh();
    }

    // =============================================
    // INNER HIGHLIGHT â€” 0 1px 0 rgba(255,255,255,0.08) inset
    // =============================================

    private void CreateInnerHighlight()
    {
        if (panelRect == null) return;

        if (innerHighlightObject == null)
        {
            Transform existing = panelRect.Find("InnerHighlight");
            if (existing != null) innerHighlightObject = existing.gameObject;
        }

        if (innerHighlightObject == null)
        {
            innerHighlightObject = new GameObject("InnerHighlight");
            innerHighlightObject.transform.SetParent(panelRect, false);
            // Put it after shadow, before content
            innerHighlightObject.transform.SetSiblingIndex(1);

            innerHighlightObject.AddComponent<RectTransform>();

            LayoutElement hlLE = innerHighlightObject.AddComponent<LayoutElement>();
            hlLE.ignoreLayout = true;
        }

        // Inset â€” slightly smaller than panel
        RectTransform hlRT = innerHighlightObject.GetComponent<RectTransform>();
        hlRT.anchorMin = Vector2.zero;
        hlRT.anchorMax = Vector2.one;
        hlRT.offsetMin = new Vector2(1f, 1f);
        hlRT.offsetMax = new Vector2(-1f, -1f);

        // Use RoundedImage for matching corner radius
        RoundedImage hlRounded = innerHighlightObject.GetComponent<RoundedImage>();
        if (hlRounded == null)
        {
            hlRounded = innerHighlightObject.AddComponent<RoundedImage>();
        }
        hlRounded.cornerRadius = Mathf.Max(0, panelCornerRadius - 1);
        hlRounded.borderThickness = 1;
        hlRounded.borderColor = innerHighlight;
        hlRounded.fillColor = Color.clear;

        Image hlImg = innerHighlightObject.GetComponent<Image>();
        if (hlImg != null)
        {
            hlImg.color = Color.white;
            hlImg.raycastTarget = false;
        }
    }

    // =============================================
    // INNER GOLD GLOW â€” 0 0 0 1px rgba(255,220,140,0.06) inset
    // =============================================

    private void CreateInnerGoldGlow()
    {
        if (panelRect == null) return;

        if (innerGoldGlowObject == null)
        {
            Transform existing = panelRect.Find("InnerGoldGlow");
            if (existing != null) innerGoldGlowObject = existing.gameObject;
        }

        if (innerGoldGlowObject == null)
        {
            innerGoldGlowObject = new GameObject("InnerGoldGlow");
            innerGoldGlowObject.transform.SetParent(panelRect, false);
            // Put it after inner highlight (index 2)
            innerGoldGlowObject.transform.SetSiblingIndex(2);

            innerGoldGlowObject.AddComponent<RectTransform>();

            LayoutElement glLE = innerGoldGlowObject.AddComponent<LayoutElement>();
            glLE.ignoreLayout = true;
        }

        // Alias for the rest of the method
        GameObject goldGlowObj = innerGoldGlowObject;

        // Inset â€” identical to panel size
        RectTransform glRT = goldGlowObj.GetComponent<RectTransform>();
        glRT.anchorMin = Vector2.zero;
        glRT.anchorMax = Vector2.one;
        glRT.offsetMin = Vector2.zero;
        glRT.offsetMax = Vector2.zero;

        RoundedImage glRounded = goldGlowObj.GetComponent<RoundedImage>();
        if (glRounded == null) glRounded = goldGlowObj.AddComponent<RoundedImage>();
        
        glRounded.cornerRadius = panelCornerRadius;
        glRounded.borderThickness = 1; // 1px inset
        glRounded.borderColor = innerGoldGlow;
        glRounded.fillColor = Color.clear;

        Image glImg = goldGlowObj.GetComponent<Image>();
        if (glImg != null)
        {
            glImg.color = Color.white;
            glImg.raycastTarget = false;
        }
    }

    // =============================================
    // TITLE â€” "MENU" â€” Cinzel 700 42px
    // =============================================

    private void CreateTitleIfNeeded()
    {
        if (panelRect == null) return;

        if (titleObject == null)
        {
            Transform existing = panelRect.Find("CinemaTitle");
            if (existing != null) titleObject = existing.gameObject;
        }

        if (titleObject == null)
        {
            titleObject = new GameObject("CinemaTitle");
            titleObject.transform.SetParent(panelRect, false); // MUST be direct child of panel
            
            // Debug.Log($"[STYLE] âœ… Created CinemaTitle as direct child of {panelRect.name}");

            titleObject.AddComponent<RectTransform>();

            LayoutElement titleLE = titleObject.AddComponent<LayoutElement>();
            titleLE.preferredHeight = titleHeight;
            titleLE.flexibleWidth = 1f;
            titleLE.flexibleHeight = 0f;
        }
        else
        {
            // CRITICAL FIX: If title exists but is wrongly parented (inside a button), reparent it
            if (titleObject.transform.parent != panelRect)
            {
                // Debug.LogWarning($"[STYLE] ⚠️ CinemaTitle was child of {titleObject.transform.parent.name}, moving to {panelRect.name}");
                titleObject.transform.SetParent(panelRect, false);
            }
        }

        // Ensure title is positioned after inner highlight (index 2: shadow=0, highlight=1, title=2)
        titleObject.transform.SetSiblingIndex(2);

        TextMeshProUGUI titleTMP = titleObject.GetComponent<TextMeshProUGUI>();
        if (titleTMP == null) titleTMP = titleObject.AddComponent<TextMeshProUGUI>();

        titleTMP.text = titleText;
        if (titleFont != null) titleTMP.font = titleFont;
        titleTMP.color = mainTextColor; // White — matching reference images
        titleTMP.fontSize = titleFontSize;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.characterSpacing = titleCharacterSpacing;
        titleTMP.enableAutoSizing = true;
        titleTMP.fontSizeMin = 20f;
        titleTMP.fontSizeMax = titleFontSize;
        titleTMP.raycastTarget = false;
    }

    // =============================================
    // SETTINGS ROW â€” Horizontal: MUTE + Speaker Icon + Slider
    // Matches reference image: frosted glass row
    // =============================================

    private void CreateSettingsRow()
    {
        if (pauseManager == null || panelRect == null) return;

        // --- Find or create the settings row container ---
        GameObject settingsRow = null;

        if (pauseManager.settingsRow != null)
        {
            settingsRow = pauseManager.settingsRow;
        }
        else
        {
            Transform existing = panelRect.Find("SettingsRow");
            if (existing != null)
            {
                settingsRow = existing.gameObject;
            }
        }

        if (settingsRow == null)
        {
            settingsRow = new GameObject("SettingsRow");
            settingsRow.transform.SetParent(panelRect, false);
            settingsRow.SetActive(false); // Default hidden
        }

        // Ensure RectTransform exists
        RectTransform rowRT = settingsRow.GetComponent<RectTransform>();
        if (rowRT == null) rowRT = settingsRow.AddComponent<RectTransform>();

        // Ensure LayoutElement exists and is configured
        LayoutElement rowLE = settingsRow.GetComponent<LayoutElement>();
        if (rowLE == null) rowLE = settingsRow.AddComponent<LayoutElement>();
        rowLE.preferredHeight = settingsHeight;
        rowLE.preferredWidth = settingsWidth > 0f ? settingsWidth : buttonWidth; // Use independent width if set
        rowLE.flexibleWidth = 0f;
        rowLE.flexibleHeight = 0f;

        // Ensure HorizontalLayoutGroup exists and is configured
        HorizontalLayoutGroup hlg = settingsRow.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null) hlg = settingsRow.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(
            settingsPaddingHorizontal, // left
            settingsPaddingHorizontal, // right
            settingsPaddingVertical,   // top
            settingsPaddingVertical    // bottom
        );
        hlg.spacing = settingsSpacing;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false; // CRITICAL FIX: Stop components from stretching to fill the background

        // --- Reparent muteButton into this row ---
        if (pauseManager.muteButton != null)
        {
            pauseManager.muteButton.transform.SetParent(settingsRow.transform, false);

            // Resize mute button to act as a text label (not full-width)
            RectTransform muteRT = pauseManager.muteButton.GetComponent<RectTransform>();
            if (muteRT != null)
            {
                LayoutElement muteLE = pauseManager.muteButton.GetComponent<LayoutElement>();
                if (muteLE == null) muteLE = pauseManager.muteButton.gameObject.AddComponent<LayoutElement>();
                muteLE.preferredWidth = settingsMuteWidth;
                muteLE.preferredHeight = settingsMuteHeight;
                muteLE.flexibleWidth = 0f;
                muteLE.ignoreLayout = false;
            }
            
            // Create microphone icon on the mute button
            // NOTE: Microphone icon is now created inside SpeakerIcon circle below
        }

        // --- Create/Find speaker icon ---
        Transform existingIcon = settingsRow.transform.Find("SpeakerIcon");
        GameObject iconObj = null;
        
        if (existingIcon == null)
        {
            iconObj = new GameObject("SpeakerIcon");
            iconObj.transform.SetParent(settingsRow.transform, false);

            RectTransform iconRT = iconObj.AddComponent<RectTransform>();

            LayoutElement iconLE = iconObj.AddComponent<LayoutElement>();
            iconLE.preferredWidth = settingsIconSize;
            iconLE.preferredHeight = settingsIconSize;
            iconLE.flexibleWidth = 0f;

            // Circular background
            RoundedImage iconRounded = iconObj.AddComponent<RoundedImage>();
            iconRounded.cornerRadius = 64;
            iconRounded.fillColor = microphoneCircleBgColor;
            iconRounded.borderColor = microphoneCircleBorderColor;
            iconRounded.borderThickness = 1;

            Image iconImg = iconObj.GetComponent<Image>();
            if (iconImg != null)
            {
                iconImg.color = Color.white;
                iconImg.raycastTarget = false;
            }
        }
        else
        {
            iconObj = existingIcon.gameObject;
        }
        
        // CRITICAL FIX: Create microphone icon INSIDE the speaker icon circle
        CreateMicrophoneIconInSpeakerCircle(iconObj);

        // --- Reparent volumeSlider into this row ---
        if (pauseManager.volumeSlider != null)
        {
            pauseManager.volumeSlider.transform.SetParent(settingsRow.transform, false);

            LayoutElement sliderLE = pauseManager.volumeSlider.GetComponent<LayoutElement>();
            if (sliderLE == null) sliderLE = pauseManager.volumeSlider.gameObject.AddComponent<LayoutElement>();
            sliderLE.preferredHeight = settingsSliderHeight;
            sliderLE.flexibleWidth = 1f; // Slider takes remaining space
            sliderLE.ignoreLayout = false;
        }

        // --- Apply frosted glass styling ---
        StyleSettingsRow(settingsRow);

        pauseManager.settingsRow = settingsRow;


    }

    private void StyleSettingsRow(GameObject settingsRow)
    {
        RoundedImage bgRounded = settingsRow.GetComponent<RoundedImage>();
        if (bgRounded == null) bgRounded = settingsRow.AddComponent<RoundedImage>();

        bgRounded.cornerRadius = settingsRadius;
        bgRounded.borderThickness = settingsBorderThickness;
        bgRounded.borderColor = settingsBorder;
        bgRounded.fillColor = settingsBackground;

        Image bgImg = settingsRow.GetComponent<Image>();
        if (bgImg != null)
        {
            bgImg.sprite = null;
            bgImg.type = Image.Type.Simple;
            bgImg.color = Color.white;
            bgImg.raycastTarget = false;
        }
    }

    // =============================================
    // BUTTON STYLING
    // =============================================


    private void StyleButton(Button button, bool isExitButton, string labelOverride = null)
    {
        if (button == null) return;
        
        // Debug.Log($"[STYLE] Inspecting Button '{button.gameObject.name}': Has {button.transform.childCount} children. targetGraphic is {(button.targetGraphic != null ? button.targetGraphic.name : "NULL")}");

        // --- RoundedImage for glass corners + border ---
        RoundedImage btnRounded = button.GetComponent<RoundedImage>();
        if (btnRounded == null) btnRounded = button.gameObject.AddComponent<RoundedImage>();

        btnRounded.cornerRadius = isExitButton ? exitButtonRadius : buttonCornerRadius;
        btnRounded.fillColor = Color.white; // Neutral â€” ColorBlock handles states
        btnRounded.borderColor = Color.clear; // Glow handles the border on hover
        btnRounded.borderThickness = 0;

        Image btnImage = button.GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.sprite = null;
            btnImage.type = Image.Type.Simple;
            btnImage.color = Color.white;
        }

        // CRITICAL FIX: Because we remove the sprite, the Image's preferred height becomes 0.
        // VerticalLayoutGroup with childControlHeight=true will squish it. 
        // We must add a LayoutElement to preserve the button's intended height.
        LayoutElement le = button.GetComponent<LayoutElement>();
        if (le == null) le = button.gameObject.AddComponent<LayoutElement>();
        
        // Force the preferred height to the inspector setting to prevent squishing
        le.preferredHeight = buttonHeight;
        
        // Ensure width is controlled correctly (prevent full-width stretching if user set a width)
        if (buttonWidth > 0f)
        {
            le.preferredWidth = buttonWidth;
            le.flexibleWidth = 0f;
        }
        else
        {
            le.preferredWidth = -1f;
            le.flexibleWidth = 1f; // Full width
        }
        
        le.flexibleHeight = 0f;

        // --- ColorBlock â€” state transitions ---
        ColorBlock cb = button.colors;
        cb.fadeDuration = 0.15f;

        if (isExitButton)
        {
            cb.normalColor = exitButtonBase;
            cb.highlightedColor = exitButtonHover;
            cb.pressedColor = exitPressed;
            cb.selectedColor = exitButtonBase;
            cb.disabledColor = new Color(0.3f, 0.15f, 0.15f, 0.2f);

            // Exit button gets visible border
            btnRounded.borderThickness = buttonBorderThickness;
            btnRounded.borderColor = new Color(1f, 0.3f, 0.3f, 0.2f);
        }
        else
        {
            cb.normalColor = buttonNormal;
            cb.highlightedColor = hoverBackground;
            cb.pressedColor = buttonPressed;
            cb.selectedColor = buttonNormal;
            cb.disabledColor = new Color(1f, 1f, 1f, 0.03f);

            // Normal button border
            btnRounded.borderThickness = buttonBorderThickness;
            btnRounded.borderColor = buttonBorder;
        }

        cb.colorMultiplier = 1f;
        button.colors = cb;
        button.transition = Selectable.Transition.ColorTint;

        // Disable keyboard/gamepad navigation â€” prevents white selection highlight border
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;

        // --- Text ---
        StyleButtonText(button, isExitButton, labelOverride);

        // --- Animations & Hover Text Color ---
        GlowButtonEffect effect = button.GetComponent<GlowButtonEffect>();
        if (effect == null) effect = button.gameObject.AddComponent<GlowButtonEffect>();
        effect.normalTextColor = buttonTextColor;
        effect.hoverTextColor = isExitButton ? Color.white : hoverTextColor;
        effect.buttonText = button.GetComponentInChildren<TextMeshProUGUI>(false);
    }

    private void StyleButtonText(Button button, bool isExitButton, string labelOverride)
    {
        if (button == null) return;

        // Get all TMP components inside the button (including inactive)
        TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>(true);
        // Get all legacy Text components inside the button (including inactive)
        Text[] legacyTexts = button.GetComponentsInChildren<Text>(true);

        bool hasTMP = tmpTexts != null && tmpTexts.Length > 0;

        if (hasTMP)
        {
            int validTextIndex = 0;
            for (int i = 0; i < tmpTexts.Length; i++)
            {
                var tmpText = tmpTexts[i];
                if (tmpText == null) continue;
                
                // CRITICAL FIX: Skip if this text belongs to CinemaTitle (not a button child)
                if (tmpText.gameObject.name == "CinemaTitle" || 
                    tmpText.transform.parent?.name == "CinemaTitle")
                {
                    // Debug.Log($"[STYLE] Skipping CinemaTitle text in button {button.name}");
                    continue;
                }
                
                if (validTextIndex == 0)
                {
                    tmpText.gameObject.SetActive(true);
                    tmpText.color = buttonTextColor;

                    // DO NOT override text - let Unity Inspector control it
                    // if (!string.IsNullOrEmpty(labelOverride))
                    //     tmpText.text = labelOverride;
                    
                    // Debug.Log($"[STYLE] âœ… Styled text for {button.name}: '{tmpText.text}' (controlled from Inspector)");
                    
                    validTextIndex++;
                }
                else
                {
                    // Disable extra text components
                    tmpText.gameObject.SetActive(false);
                }
            }

            if (legacyTexts != null)
            {
                foreach (var legacyText in legacyTexts)
                {
                    if (legacyText != null)
                    {
                        legacyText.gameObject.SetActive(false);
                    }
                }
            }
        }
        else if (legacyTexts != null && legacyTexts.Length > 0)
        {
            for (int i = 0; i < legacyTexts.Length; i++)
            {
                var legacyText = legacyTexts[i];
                if (legacyText == null) continue;

                if (i == 0)
                {
                    legacyText.gameObject.SetActive(true);
                    legacyText.color = buttonTextColor;
                    legacyText.fontStyle = FontStyle.Bold;
                    // DO NOT override text - let Unity Inspector control it
                    // if (!string.IsNullOrEmpty(labelOverride))
                    //     legacyText.text = labelOverride;
                }
                else
                {
                    legacyText.gameObject.SetActive(false);
                }
            }
        }
    }

    // =============================================
    // BACK BUTTON â€” Circular, positioned absolutely
    // =============================================

    private void StyleBackButton(Button backButton)
    {
        if (backButton == null) return;

        // --- Make it ignore the VerticalLayoutGroup ---
        LayoutElement backLE = backButton.GetComponent<LayoutElement>();
        if (backLE == null) backLE = backButton.gameObject.AddComponent<LayoutElement>();
        backLE.ignoreLayout = true;

        // --- RoundedImage â€” full circle ---
        RoundedImage btnRounded = backButton.GetComponent<RoundedImage>();
        if (btnRounded == null) btnRounded = backButton.gameObject.AddComponent<RoundedImage>();

        btnRounded.cornerRadius = backButtonRadius;
        btnRounded.fillColor = buttonNormal;
        btnRounded.borderColor = buttonBorder;
        btnRounded.borderThickness = buttonBorderThickness;

        Image btnImage = backButton.GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.sprite = null;
            btnImage.type = Image.Type.Simple;
            btnImage.color = Color.white;
        }

        // --- Position: top-left corner of panel ---
        RectTransform backRT = backButton.GetComponent<RectTransform>();
        if (backRT != null)
        {
            // Anchor to top-left of parent panel
            backRT.anchorMin = new Vector2(0f, 1f);
            backRT.anchorMax = new Vector2(0f, 1f);
            backRT.pivot = new Vector2(0.5f, 0.5f);
            backRT.anchoredPosition = new Vector2(backButtonLeftOffset, -backButtonTopOffset);
            
            // backLE is already handled at the top of the method
            
            // Remove the conflicting ContentSizeFitter that was shrinking it to 0
            ContentSizeFitter backCSF = backRT.GetComponent<ContentSizeFitter>();
            if (backCSF != null) Destroy(backCSF);

            // Directly set the size at runtime!
            backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backButtonSize);
            backRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backButtonSize);

            // Debug.Log($"[STYLE] Back Button Setup: Size=({backRT.rect.width}, {backRT.rect.height}), Pos={backRT.anchoredPosition}, Active={backButton.gameObject.activeInHierarchy}");
        }

        // --- ColorBlock for hover states ---
        ColorBlock cb = backButton.colors;
        cb.fadeDuration = 0.15f;
        cb.normalColor = buttonNormal;
        cb.highlightedColor = hoverBackground;
        cb.pressedColor = buttonPressed;
        cb.selectedColor = buttonNormal;
        cb.colorMultiplier = 1f;
        backButton.colors = cb;
        backButton.transition = Selectable.Transition.ColorTint;

        // Disable navigation to prevent white selection highlight border
        Navigation nav = backButton.navigation;
        nav.mode = Navigation.Mode.None;
        backButton.navigation = nav;

        // --- Text: style only (text controlled from Unity Inspector) ---
        TextMeshProUGUI tmpText = backButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpText != null)
        {
            // DO NOT override text - let Unity Inspector control it
            // tmpText.text = "← ";
            tmpText.color = buttonTextColor;
            tmpText.fontSize = backButtonTextSize; // Use Inspector value
            tmpText.fontStyle = FontStyles.Normal;
            tmpText.characterSpacing = 0f;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.raycastTarget = false; // CRITICAL FIX: Text must not block button clicks!
            
            // CRITICAL FIX: Configure text RectTransform to fill the button and respect offsets
            RectTransform textRT = tmpText.GetComponent<RectTransform>();
            if (textRT != null)
            {
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                textRT.offsetMax = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                textRT.pivot = new Vector2(0.5f, 0.5f);
            }
            
            // Debug.Log($"[STYLE] ✅ Back button text: '{tmpText.text}' (controlled from Inspector), Size={backButtonTextSize}, Offset=({backButtonTextOffsetX}, {backButtonTextOffsetY})");
        }
        else
        {
            Text legacyText = backButton.GetComponentInChildren<Text>(true);
            if (legacyText != null)
            {
                // DO NOT override text - let Unity Inspector control it
                // legacyText.text = "← ";
                legacyText.color = buttonTextColor;
                legacyText.fontSize = (int)backButtonTextSize; // Use Inspector value
                legacyText.alignment = TextAnchor.MiddleCenter;
                legacyText.raycastTarget = false; // CRITICAL FIX: Text must not block button clicks!
                
                // Configure text RectTransform
                RectTransform textRT = legacyText.GetComponent<RectTransform>();
                if (textRT != null)
                {
                    textRT.anchorMin = Vector2.zero;
                    textRT.anchorMax = Vector2.one;
                    textRT.offsetMin = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                    textRT.offsetMax = new Vector2(backButtonTextOffsetX, backButtonTextOffsetY);
                    textRT.pivot = new Vector2(0.5f, 0.5f);
                }
            }
        }

        // --- Animations & Hover Text Color ---
        GlowButtonEffect effect = backButton.GetComponent<GlowButtonEffect>();
        if (effect == null) effect = backButton.gameObject.AddComponent<GlowButtonEffect>();
        effect.normalTextColor = buttonTextColor;
        effect.hoverTextColor = hoverTextColor;
        effect.buttonText = backButton.GetComponentInChildren<TextMeshProUGUI>(false);
    }

    // =============================================
    // SLIDER STYLING â€” Gold precision
    // =============================================

    private void StyleSlider()
    {
        Slider slider = pauseManager.volumeSlider;
        if (slider == null) return;

        // --- Fill (الجزء الممتلئ) — Strong glowing gold ---
        if (slider.fillRect != null)
        {
            Image fillImg = slider.fillRect.GetComponent<Image>();
            if (fillImg != null) { fillImg.sprite = null; fillImg.type = Image.Type.Simple; fillImg.color = Color.white; }

            RoundedImage fillRounded = slider.fillRect.GetComponent<RoundedImage>();
            if (fillRounded == null) fillRounded = slider.fillRect.gameObject.AddComponent<RoundedImage>();
            fillRounded.cornerRadius = sliderTrackRadius;
            fillRounded.borderThickness = 0;
            fillRounded.fillColor = sliderFill;
            fillRounded.borderColor = Color.clear;

            // Set track height via LayoutElement
            LayoutElement fillLE = slider.fillRect.GetComponent<LayoutElement>();
            if (fillLE == null) fillLE = slider.fillRect.gameObject.AddComponent<LayoutElement>();
            fillLE.preferredHeight = sliderTrackHeight;
        }

        // --- Background (Empty track) ---
        Transform bgTransform = slider.transform.Find("Background");
        if (bgTransform != null)
        {
            Image bgImg = bgTransform.GetComponent<Image>();
            if (bgImg != null) { bgImg.sprite = null; bgImg.type = Image.Type.Simple; bgImg.color = Color.white; }

            RoundedImage bgRounded = bgTransform.GetComponent<RoundedImage>();
            if (bgRounded == null) bgRounded = bgTransform.gameObject.AddComponent<RoundedImage>();
            bgRounded.cornerRadius = sliderTrackRadius;
            bgRounded.borderThickness = 0;
            bgRounded.fillColor = sliderEmpty;
            bgRounded.borderColor = Color.clear;
        }

        // --- Handle (Thumb) â€” solid gold circle ---
        if (slider.handleRect != null)
        {
            Image handleImg = slider.handleRect.GetComponent<Image>();
            if (handleImg != null) { handleImg.sprite = null; handleImg.type = Image.Type.Simple; handleImg.color = Color.white; }

            RoundedImage handleRounded = slider.handleRect.GetComponent<RoundedImage>();
            if (handleRounded == null) handleRounded = slider.handleRect.gameObject.AddComponent<RoundedImage>();
            handleRounded.cornerRadius = 64; // Full circle
            handleRounded.borderThickness = 0;
            handleRounded.fillColor = primaryAccent;
            handleRounded.borderColor = Color.clear;

            // Thumb size
            slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderThumbSize);
            slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sliderThumbSize);

            // Thumb glow â€” 0 0 0 3px rgba(240,200,74,0.25)
            // Create a glow child behind the handle
            CreateThumbGlow(slider.handleRect);
        }

        // Slider color transitions
        ColorBlock scb = slider.colors;
        scb.normalColor = Color.white;
        scb.highlightedColor = new Color(1f, 1f, 1f, 0.95f);
        scb.pressedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        scb.fadeDuration = 0.1f;
        slider.colors = scb;
    }

    /// <summary>
    /// Creates a soft gold glow ring around the slider thumb
    /// 0 0 0 3px rgba(240,200,74,0.25)
    /// </summary>
    private void CreateThumbGlow(RectTransform handleRect)
    {
        Transform existing = handleRect.Find("ThumbGlow");
        if (existing != null) return; // Already exists

        GameObject glowObj = new GameObject("ThumbGlow");
        glowObj.transform.SetParent(handleRect, false);
        glowObj.transform.SetAsFirstSibling(); // Behind thumb

        RectTransform glowRT = glowObj.AddComponent<RectTransform>();
        float glowPadding = sliderThumbGlowSpread * 2f; // Convert spread to padding (both sides)
        glowRT.anchorMin = Vector2.zero;
        glowRT.anchorMax = Vector2.one;
        glowRT.offsetMin = new Vector2(-glowPadding, -glowPadding);
        glowRT.offsetMax = new Vector2(glowPadding, glowPadding);

        RoundedImage glowRounded = glowObj.AddComponent<RoundedImage>();
        glowRounded.cornerRadius = 64;
        glowRounded.borderThickness = 0;
        glowRounded.fillColor = new Color(0.941f, 0.784f, 0.290f, 0.25f);
        glowRounded.borderColor = Color.clear;

        Image glowImg = glowObj.GetComponent<Image>();
        if (glowImg != null)
        {
            glowImg.color = Color.white;
            glowImg.raycastTarget = false;
        }
    }

    // =============================================
    // MICROPHONE ICON – Mute Button Visual Indicator
    // =============================================

    /// <summary>
    /// Creates a microphone icon INSIDE the speaker icon circle that changes color when muted
    /// </summary>
    private void CreateMicrophoneIconInSpeakerCircle(GameObject speakerIconObj)
    {
        if (speakerIconObj == null) return;

        // CRITICAL FIX: Always destroy old icon AND glow to recreate them
        Transform existingIcon = speakerIconObj.transform.Find("MicrophoneIcon");
        if (existingIcon != null)
        {
            // Debug.Log("[STYLE] 🗑️ Destroying old MicrophoneIcon to recreate with new settings...");
            if (Application.isPlaying)
                Destroy(existingIcon.gameObject);
            else
                DestroyImmediate(existingIcon.gameObject);
        }
        
        Transform existingGlow = speakerIconObj.transform.Find("MicrophoneGlow");
        if (existingGlow != null)
        {
            // Debug.Log("[STYLE] 🗑️ Destroying old MicrophoneGlow to recreate with new settings...");
            if (Application.isPlaying)
                Destroy(existingGlow.gameObject);
            else
                DestroyImmediate(existingGlow.gameObject);
        }

        bool isMuted = pauseManager.IsMuted;
        
        // Create RED GLOW (shadow) - shown when muted
        GameObject glowObj = null;
        if (showMutedGlow)
        {
            glowObj = new GameObject("MicrophoneGlow");
            glowObj.transform.SetParent(speakerIconObj.transform, false);

            RectTransform glowRT = glowObj.AddComponent<RectTransform>();
            glowRT.anchorMin = new Vector2(0.5f, 0.5f);
            glowRT.anchorMax = new Vector2(0.5f, 0.5f);
            glowRT.pivot = new Vector2(0.5f, 0.5f);
            glowRT.anchoredPosition = Vector2.zero;
            glowRT.sizeDelta = Vector2.one * microphoneIconSize * mutedGlowSize;

            Image glowImg = glowObj.AddComponent<Image>();
            glowImg.sprite = isMuted && microphoneIconMutedSprite != null ? microphoneIconMutedSprite : microphoneIconSprite;
            glowImg.color = mutedGlowColor;
            glowImg.raycastTarget = false;
            glowImg.preserveAspect = true;
            
            // Hide glow if not muted
            glowObj.SetActive(isMuted);
        }

        // Create icon container INSIDE the speaker circle
        GameObject iconObj = new GameObject("MicrophoneIcon");
        iconObj.transform.SetParent(speakerIconObj.transform, false);

        RectTransform iconRT = iconObj.AddComponent<RectTransform>();
        iconRT.anchorMin = Vector2.zero;
        iconRT.anchorMax = Vector2.one;
        iconRT.offsetMin = Vector2.zero;
        iconRT.offsetMax = Vector2.zero;

        // ALWAYS use Image component with sprite
        Image iconImg = iconObj.AddComponent<Image>();
        
        // Use different sprite based on mute state
        if (isMuted && microphoneIconMutedSprite != null)
        {
            iconImg.sprite = microphoneIconMutedSprite; // Muted sprite
        }
        else
        {
            iconImg.sprite = microphoneIconSprite; // Normal sprite
        }
        
        iconImg.color = isMuted ? microphoneMutedColor : microphoneNormalColor;
        iconImg.raycastTarget = false;
        iconImg.preserveAspect = true;
        iconImg.type = Image.Type.Simple;

        // If no sprite assigned, log warning
        if (microphoneIconSprite == null)
        {
            // Debug.LogWarning("[STYLE] ⚠️ Microphone Icon Sprite is NULL! Please assign a sprite in the Inspector.");
        }

        // Debug.Log($"[STYLE] ✅ Created microphone icon inside speaker circle (Sprite: {iconImg.sprite?.name}, Color: {iconImg.color}, Glow: {(glowObj != null && glowObj.activeSelf)})");
    }

    
    /// <summary>
    /// Public method to force create microphone icon (called from PauseMenuManager if icon is missing)
    /// </summary>
    public void ForceCreateMicrophoneIcon()
    {
        if (pauseManager == null || pauseManager.settingsRow == null) return;
        
        // Find the speaker icon
        Transform speakerIconTrans = pauseManager.settingsRow.transform.Find("SpeakerIcon");
        if (speakerIconTrans == null)
        {
            // Debug.LogWarning("[STYLE] ⚠️ SpeakerIcon not found in settingsRow!");
            return;
        }
        
        // Check if icon already exists
        Transform existingIcon = speakerIconTrans.Find("MicrophoneIcon");
        if (existingIcon != null)
        {
            // Debug.Log("[STYLE] MicrophoneIcon already exists, skipping creation.");
            return;
        }
        
        // Debug.Log("[STYLE] 🔧 Force creating MicrophoneIcon inside SpeakerIcon...");
        CreateMicrophoneIconInSpeakerCircle(speakerIconTrans.gameObject);
    }


    // =============================================
    // PANEL PADDING â€” 44px 40px 48px
    // =============================================

    private void ConfigurePanelPadding()
    {
        if (panelRect == null) return;

        VerticalLayoutGroup layoutGroup = panelRect.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = panelRect.gameObject.AddComponent<VerticalLayoutGroup>();
            // Debug.Log("[STYLE] ➕ Added VerticalLayoutGroup to panel");
        }

        layoutGroup.padding = new RectOffset(
            Mathf.RoundToInt(paddingHorizontal),  // left
            Mathf.RoundToInt(paddingHorizontal),  // right
            Mathf.RoundToInt(paddingTop),          // top
            Mathf.RoundToInt(paddingBottom)         // bottom
        );
        layoutGroup.spacing = elementSpacing;

        // Enforce layout flags — childControlHeight must be true
        // so LayoutElement.preferredHeight works for separator, title, settingsRow
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false; // Fixed: Prevents buttons from stretching full width
    }

    // =============================================
    // PANEL ANCHORS â€” Responsive screen-relative sizing
    // =============================================

    /// <summary>
    /// Makes the pause menu panel responsive:
    /// â€” Anchored to screen center (no fixed pixel offsets)
    /// â€” Width = % of screen width (clamped to min/max)
    /// â€” Height = auto via VerticalLayoutGroup + ContentSizeFitter (or manual min height)
    /// This replaces any fixed pixel dimensions set in the Editor.
    /// </summary>
    private void ConfigurePanelAnchors()
    {
        if (panelRect == null) return;

        // Anchor dead-center â€” pivot at (0.5, 0.5)
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot     = new Vector2(0.5f, 0.5f);

        // Zero out position offsets so it sits exactly at screen center
        panelRect.anchoredPosition = Vector2.zero;

        // Width: clamp between 300 and 680px (works for 720pâ€“4K)
        // Height: driven by ContentSizeFitter so buttons always fit
        float refWidth = 1920f; // matches CanvasScaler referenceResolution
        float desiredWidth = refWidth * panelWidthPercent;        // Inspector field
        desiredWidth = Mathf.Clamp(desiredWidth, 300f, 680f);

        panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);

        // Let height be driven automatically OR set minimum height
        ContentSizeFitter csf = panelRect.GetComponent<ContentSizeFitter>();
        if (csf == null) csf = panelRect.gameObject.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // We control width
        
        if (panelFixedHeight > 0f)
        {
            // Fixed height from Inspector
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            LayoutElement panelLE = panelRect.GetComponent<LayoutElement>();
            if (panelLE == null) panelLE = panelRect.gameObject.AddComponent<LayoutElement>();
            panelLE.preferredHeight = isSettingsMode() ? panelSettingsFixedHeight : panelFixedHeight;
            // Debug.Log($"[STYLE] Panel fixedHeight={panelLE.preferredHeight}px");
        }
        else if (panelMinHeight > 0f)
        {
            // Manual minimum height
            csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
            LayoutElement le = panelRect.GetComponent<LayoutElement>();
            if (le == null) le = panelRect.gameObject.AddComponent<LayoutElement>();
            le.minHeight = panelMinHeight;
            // Debug.Log($"[STYLE] Panel anchors set â€” width={desiredWidth}px, minHeight={panelMinHeight}px, centered");
        }
        else
        {
            // Auto height based on content
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            // Debug.Log($"[STYLE] Panel anchors set â€” width={desiredWidth}px, height=auto, centered");
        }
    }

}

