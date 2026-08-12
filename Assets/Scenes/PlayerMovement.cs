using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;

    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float sprintSpeed = 10.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;

    [Header("Stamina Settings")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaDrain = 20f;
    [SerializeField] float staminaRegen = 15f;
    [SerializeField] float regenDelay = 1.5f;
    [SerializeField] float currentStamina;

    [Header("Exhaustion UI Effect")]
    [SerializeField] CanvasGroup exhaustionOverlay;
    [SerializeField] float startDarkeningAt = 0.4f;
    [SerializeField] float maxOverlayOpacity = 0.75f;

    [Header("Gravity & Grounding")]
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    float regenDelayTimer;
    bool isExhausted;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        if (RunSessionTracker.Instance != null)
        {
            RunSessionTracker.Instance.StartNewRun();
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (exhaustionOverlay == null)
        {
            Debug.LogWarning("Please assign the ExhaustionOverlay CanvasGroup in the Inspector!");
        }
    }

    void Update()
    {
        // FIX 1: Run stamina recovery and visual overlays constantly, even in the menu
        UpdateStaminaAndLogic();
        UpdateVisualEffects();

        // Check if game is paused OR if dialogue is active!
        if (PauseMenu.GameIsPaused || DialogueUI.IsDialogueActive)
        {
            // Keep applying gravity so you don't float, but stop user input/camera rotation
            currentDir = Vector2.SmoothDamp(currentDir, Vector2.zero, ref currentDirVelocity, moveSmoothTime);
            ApplyGravityAndMove(currentDir, walkSpeed);
            return;
        }

        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = Vector2.zero;

        if (Mouse.current != null)
        {
            targetMouseDelta = Mouse.current.delta.ReadValue() * 0.05f;
        }

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    // Process input data and decide speed
    // Inside Movement.cs -> UpdateMove()
    void UpdateMove()
    {
        Vector2 targetDir = Vector2.zero;
        bool isSprintKeyPressed = false;

        if (Keyboard.current != null)
        {
            float moveX = 0f;
            float moveY = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;

            targetDir = new Vector2(moveX, moveY);
            isSprintKeyPressed = Keyboard.current.leftShiftKey.isPressed;
        }

        targetDir.Normalize();

        bool isMoving = targetDir.magnitude > 0;

        // --- ADD THIS LINE TO TRACK HOLD TIME ---
        if (isMoving && RunSessionTracker.Instance != null)
        {
            RunSessionTracker.Instance.AddWalkHoldTime(Time.deltaTime);
        }
        // ----------------------------------------

        float currentSpeed = walkSpeed;

        if (isSprintKeyPressed && isMoving && currentStamina > 0 && !isExhausted)
        {
            currentSpeed = sprintSpeed;
        }

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
        ApplyGravityAndMove(currentDir, currentSpeed);
    }

    // New helper to handle the engine-level character controller movement translations
    void ApplyGravityAndMove(Vector2 moveDirection, float speed)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);
        velocityY += gravity * 2f * Time.deltaTime;

        Vector3 moveVelocity = (transform.forward * moveDirection.y + transform.right * moveDirection.x) * speed + Vector3.up * velocityY;
        controller.Move(moveVelocity * Time.deltaTime);

        if (isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }

    // Separated math logic so stamina updates independent of user inputs
    void UpdateStaminaAndLogic()
    {
        Vector2 inputDir = Vector2.zero;
        bool isSprintKeyPressed = false;

        // Read real-time key states directly (works even when menu halts standard UpdateMove loops)
        if (Keyboard.current != null && !PauseMenu.GameIsPaused)
        {
            float moveX = 0f;
            float moveY = 0f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;

            inputDir = new Vector2(moveX, moveY);
            isSprintKeyPressed = Keyboard.current.leftShiftKey.isPressed;
        }

        bool isMoving = inputDir.magnitude > 0;

        if (isSprintKeyPressed && isMoving && currentStamina > 0 && !isExhausted)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            regenDelayTimer = regenDelay;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
        }
        else
        {
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegen * Time.deltaTime;

                if (isExhausted && currentStamina >= (maxStamina * 0.15f))
                {
                    isExhausted = false;
                }
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    void UpdateVisualEffects()
    {
        if (exhaustionOverlay == null) return;

        float staminaPercent = currentStamina / maxStamina;
        float targetAlpha = 0f;

        if (staminaPercent < startDarkeningAt)
        {
            float darknessFactor = 1f - (staminaPercent / startDarkeningAt);
            targetAlpha = darknessFactor * maxOverlayOpacity;
        }

        if (isExhausted)
        {
            targetAlpha = maxOverlayOpacity;
        }

        exhaustionOverlay.alpha = Mathf.MoveTowards(exhaustionOverlay.alpha, targetAlpha, Time.deltaTime * 2f);
    }
    // Add these at the bottom of the Movement class
    public bool IsExhausted()
    {
        return isExhausted;
    }

    public void ForceStaminaUpdate()
    {
        UpdateStaminaAndLogic();
        UpdateVisualEffects();
    }
}