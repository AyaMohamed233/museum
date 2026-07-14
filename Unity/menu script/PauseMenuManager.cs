using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    
    [Tooltip("ضع هنا واجهة المرشد السياحي (مثل Press N to Speak) ليتم إخفاؤها أثناء التوقف")]
    public GameObject guideUI;
    
    [Tooltip("ضع هنا الـ Global Volume الأساسي الذي يحتوي على كل التأثيرات (ومن ضمنها الـ Depth Of Field)")]
    public Volume globalVolume;
    private VolumeComponent depthOfField;

    [Header("UI Controls")]
    public Slider volumeSlider;
    public Button muteButton;
    public Button settingsButton;
    public Button exitButton;
    public Button menuButton;

    [Tooltip("زرار الرجوع من الإعدادات للقائمة الرئيسية")]
    public Button backButton;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    private float savedVolume;
    private bool isMuted = false;

    private bool isPaused = false;

    /// <summary>
    /// Static flag for other scripts (e.g. PlayerController) to check pause state.
    /// Prevents input conflicts that cause cursor/movement bugs.
    /// </summary>
    public static bool IsGamePaused { get; private set; }

    private bool isSettingsOpen = false;

    // Frame guard: prevents ESC from being processed twice in one frame
    // (once by Update's Input.GetKeyDown, once by EventSystem's Cancel action)
    private int lastEscapeFrame = -1;

    // Cache state to avoid breaking TGTourGuideController
    private bool wasTimelinePlaying;
    private bool wasNarrationPlaying;
    private bool wasGuideUIActive;

    // Cinema overlay (created by CinematicPauseStyle)
    [HideInInspector] public GameObject cinemaOverlay;

    // Settings row container — holds muteButton + speaker icon + volumeSlider
    [HideInInspector] public GameObject settingsRow;

    // Cached PlayerController — used to sync cursorLocked state on resume (visitor mode only)
    private PlayerController playerController;

    // Cached reference to avoid repeated GetComponent calls
    private CinematicPauseStyle _cachedStyle;
    private CinematicPauseStyle CachedStyle
    {
        get
        {
            if (_cachedStyle == null)
                _cachedStyle = GetComponent<CinematicPauseStyle>();
            return _cachedStyle;
        }
    }

    /// <summary>
    /// Public property for mute state (used by CinematicPauseStyle for icon visuals)
    /// </summary>
    public bool IsMuted => isMuted;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        // البحث عن تأثير الـ Depth Of Field داخل الـ Global Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet(out UnityEngine.Rendering.Universal.DepthOfField dof))
            {
                depthOfField = dof;
                depthOfField.active = false;
            }
        }

        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMenu);
        if (muteButton != null) muteButton.onClick.AddListener(ToggleMute);
        
        if (settingsButton != null) 
        {
            settingsButton.interactable = true;
            settingsButton.onClick.AddListener(OpenSettings);
        }

        // ربط زرار الرجوع بدالة إغلاق الإعدادات
        if (backButton != null)
        {
            backButton.interactable = true;
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
            var img = backButton.GetComponent<Image>();
            if (img != null) img.raycastTarget = true;
        }
        else
        {
            Debug.LogError("[PAUSE] backButton is NULL! Drag it in the Inspector.");
        }

        // في البداية نضمن أن واجهة الإعدادات مخفية والأزرار الرئيسية ظاهرة
        CloseSettings();

        if (volumeSlider != null && backgroundMusic != null)
        {
            volumeSlider.value = backgroundMusic.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
            savedVolume = backgroundMusic.volume;
            
            if (backgroundMusic.volume <= 0.01f)
            {
                isMuted = true;
            }
        }

        // Cache PlayerController for cursor sync on resume (exists in visitor mode only)
        playerController = FindObjectOfType<PlayerController>();

        // Lock cursor for gameplay and reset pause state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsGamePaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Skip if HUD is open and will handle this escape press
            if (!isPaused && HUDManager.Instance != null && HUDManager.Instance.IsAnyPanelOpen)
            {
                return;
            }

            // FRAME GUARD: Skip if ESC was already processed this frame
            if (lastEscapeFrame == Time.frameCount)
            {
                return;
            }
            lastEscapeFrame = Time.frameCount;

            if (!isPaused)
            {
                PauseGame();
                return;
            }

            if (isPaused && isSettingsOpen)
            {
                CloseSettings();
                return;
            }

            if (isPaused && !isSettingsOpen)
            {
                ResumeGame();
                return;
            }
        }
    }

    public void ConsumeEscape()
    {
        lastEscapeFrame = Time.frameCount;
    }

    public void PauseGame()
    {
        isPaused = true;
        IsGamePaused = true;
        isSettingsOpen = false;
        Time.timeScale = 0f;
        ShowMainPauseButtons();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (cinemaOverlay != null) cinemaOverlay.SetActive(true);
        
        if (guideUI != null)
        {
            wasGuideUIActive = guideUI.activeSelf;
            guideUI.SetActive(false); // Hide Guide UI
        }
        
        if (depthOfField != null) depthOfField.active = true; 

        // CRITICAL: Show cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Sync PlayerController: disable Cinemachine + set cursorLocked = false
        // This is needed when PauseMenuManager.Update() runs BEFORE PlayerController.Update(),
        // because the IsGamePaused guard makes PlayerController skip its own SetCursorLock call.
        if (playerController != null) playerController.SetCursorLock(false);

        // 1. Safe Timeline pause
        if (TGTourGuideController.Instance != null && TGTourGuideController.Instance.playableDirector != null)
        {
            wasTimelinePlaying = (TGTourGuideController.Instance.playableDirector.state == PlayState.Playing);
            if (wasTimelinePlaying)
            {
                TGTourGuideController.Instance.playableDirector.Pause();
            }
        }

        // 2. Safe Narration pause
        if (TGTourGuideController.Instance != null && TGTourGuideController.Instance.narrationAudioSource != null)
        {
            wasNarrationPlaying = TGTourGuideController.Instance.narrationAudioSource.isPlaying;
            if (wasNarrationPlaying)
            {
                TGTourGuideController.Instance.narrationAudioSource.Pause();
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsGamePaused = false;
        isSettingsOpen = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (cinemaOverlay != null) cinemaOverlay.SetActive(false);
        
        if (guideUI != null && wasGuideUIActive)
        {
            guideUI.SetActive(true); // Show Guide UI again if it was active
        }
        
        if (depthOfField != null) depthOfField.active = false; 

        // CRITICAL: Re-lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Sync PlayerController internal state (cursorLocked + Cinemachine)
        // Without this, cursorLocked stays false → HandleMovement() blocks movement
        if (playerController != null) playerController.SetCursorLock(true);

        // 1. Resume Timeline safely
        if (wasTimelinePlaying && TGTourGuideController.Instance != null && TGTourGuideController.Instance.playableDirector != null)
        {
            TGTourGuideController.Instance.playableDirector.Play();
        }

        // 2. Resume Narration safely
        if (wasNarrationPlaying && TGTourGuideController.Instance != null && TGTourGuideController.Instance.narrationAudioSource != null)
        {
            TGTourGuideController.Instance.narrationAudioSource.UnPause();
        }
    }

    private void OnBackButtonClicked()
    {
        lastEscapeFrame = Time.frameCount;
        CloseSettings();
    }

    public void OpenSettings()
    {
        if (!isPaused) return;

        isSettingsOpen = true;
        
        // إخفاء الأزرار الرئيسية
        if (settingsButton != null) settingsButton.gameObject.SetActive(false);
        if (exitButton != null) exitButton.gameObject.SetActive(false);
        if (menuButton != null) menuButton.gameObject.SetActive(false);
        
        // إظهار عناصر الإعدادات (settings row container)
        if (settingsRow != null)
        {
            settingsRow.SetActive(true);
            if (muteButton != null) muteButton.gameObject.SetActive(true);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
        }
        else
        {
            if (muteButton != null) muteButton.gameObject.SetActive(true);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
        }

        // إظهار زرار الرجوع
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
            backButton.interactable = true;
            backButton.transform.SetAsLastSibling();
            
            // Ensure raycast targets are correct
            Image backImg = backButton.GetComponent<Image>();
            if (backImg != null) backImg.raycastTarget = true;
            
            TMPro.TextMeshProUGUI backText = backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (backText != null) backText.raycastTarget = false;
            
            Text legacyText = backButton.GetComponentInChildren<Text>();
            if (legacyText != null) legacyText.raycastTarget = false;
        }

        // Ensure microphone icon exists inside the correct parent (SpeakerIcon in settingsRow)
        if (settingsRow != null)
        {
            Transform speakerIcon = settingsRow.transform.Find("SpeakerIcon");
            if (speakerIcon != null && speakerIcon.Find("MicrophoneIcon") == null)
            {
                CinematicPauseStyle style = CachedStyle;
                if (style != null)
                {
                    style.ForceCreateMicrophoneIcon();
                }
            }
        }

        // Switch title to settings mode
        CinematicPauseStyle titleStyle = CachedStyle;
        if (titleStyle != null) titleStyle.ApplySettingsTitle();
    }

    public void CloseSettings()
    {
        isSettingsOpen = false;
        ShowMainPauseButtons();

        // Restore title to main menu mode
        CinematicPauseStyle style = CachedStyle;
        if (style != null) style.ApplyMainTitle();
    }

    /// <summary>
    /// إظهار الأزرار الرئيسية وإخفاء عناصر الإعدادات
    /// </summary>
    private void ShowMainPauseButtons()
    {
        if (settingsButton != null) settingsButton.gameObject.SetActive(true);
        if (exitButton != null) exitButton.gameObject.SetActive(true);
        if (menuButton != null) menuButton.gameObject.SetActive(true);
        
        if (settingsRow != null)
        {
            settingsRow.SetActive(false);
        }
        else
        {
            if (muteButton != null) muteButton.gameObject.SetActive(false);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        }

        if (backButton != null) backButton.gameObject.SetActive(false);
    }

    public void SetVolume(float vol)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = vol;
            if (vol > 0) isMuted = false;
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        
        if (backgroundMusic != null)
        {
            if (isMuted)
            {
                savedVolume = volumeSlider != null ? volumeSlider.value : backgroundMusic.volume;
                backgroundMusic.volume = 0f;
                if (volumeSlider != null) volumeSlider.value = 0f;
            }
            else
            {
                backgroundMusic.volume = savedVolume > 0.1f ? savedVolume : 0.5f;
                if (volumeSlider != null) volumeSlider.value = backgroundMusic.volume;
            }
        }
        // NOTE: CinematicPauseStyle.LateUpdate() handles microphone icon visual updates
        // based on IsMuted property — no need to update icon from here.
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (TGTourGuideController.Instance != null)
        {
            if (TGTourGuideController.Instance.playableDirector != null)
                TGTourGuideController.Instance.playableDirector.Stop();

            if (TGTourGuideController.Instance.narrationAudioSource != null)
                TGTourGuideController.Instance.narrationAudioSource.Stop();
        }
            
        SceneManager.LoadScene("menu");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void OnDestroy()
    {
        // Reset static flag on scene unload to prevent stale state
        IsGamePaused = false;
    }
}
