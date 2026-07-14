using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    
    // Cinemachine handles all camera rotation (Pan Tilt + Input Axis Controller)
    // No camera fields needed here anymore
    
    private CharacterController characterController;
    private Vector3 velocity;
    private bool cursorLocked = true;
    public bool IsCursorLocked => cursorLocked;
    
    private Camera mainCamera;
    private MonoBehaviour cinemachineInput;
    private HUDManager hudManager;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        
        // Find Cinemachine input component to disable it when interacting with UI
        GameObject cmCam = GameObject.Find("CinemachineCamera");
        if (cmCam != null)
        {
            cinemachineInput = cmCam.GetComponent("CinemachineInputAxisController") as MonoBehaviour;
            if (cinemachineInput == null)
            {
                // Fallback for older versions of Cinemachine
                cinemachineInput = cmCam.GetComponent("CinemachineInputProvider") as MonoBehaviour;
            }
        }
        
        // Lock cursor
        SetCursorLock(true);
    }

    private void Start()
    {
        // Subscribe to HUDManager events to freeze/unfreeze movement
        hudManager = FindObjectOfType<HUDManager>();
        if (hudManager != null)
        {
            hudManager.OnMapToggled += OnPanelToggled;
            hudManager.OnInfoToggled += OnPanelToggled;
        }
    }

    private void Update()
    {
        // Enforce cursor lock state every frame to prevent Unity Editor ESC bugs
        if (cursorLocked)
        {
            if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
            if (Cursor.visible) Cursor.visible = false;
        }

        // ESC is now handled by HUDManager for menu — no cursor toggle here
        HandleMovement();
        ApplyGravity();
    }

    /// <summary>
    /// Called by HUDManager when any panel is opened or closed.
    /// </summary>
    private void OnPanelToggled(bool isOpen)
    {
        SetCursorLock(!isOpen);
    }

    /// <summary>
    /// Public method to lock/unlock cursor. Can be called by other scripts.
    /// </summary>
    public void SetCursorLock(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
        
        // Freeze camera rotation when unlocking the cursor
        if (cinemachineInput != null)
        {
            cinemachineInput.enabled = locked;
        }
    }

    private void HandleMovement()
    {
        // Freeze player movement when cursor is unlocked (e.g., panel is open)
        if (!cursorLocked) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Camera-relative movement (Cinemachine controls camera direction)
        if (mainCamera == null) mainCamera = Camera.main;
        
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 move = right * horizontal + forward * vertical;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (hudManager != null)
        {
            hudManager.OnMapToggled -= OnPanelToggled;
            hudManager.OnInfoToggled -= OnPanelToggled;
        }
    }
}
