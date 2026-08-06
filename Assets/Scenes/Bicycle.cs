using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Bicycle : InteractableBase
{
    [Header("Lock System Connections")]
    [SerializeField] private GameObject bikeLockModel;
    [SerializeField] private string lockedDialogue = "The bike is locked securely with a thick cable.";
    [SerializeField] private string unlockDialogue = "You snipped the cable lock using the Wire Cutters!";

    private bool isLocked = true;
    private bool isRiding = false;

    [Header("Physics Movement Metrics")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float accelerationRate = 25f;
    [SerializeField] private float frictionDeceleration = 15f;
    [SerializeField] private float maxSteerAngle = 40f;
    [SerializeField] private float steeringSmoothTime = 0.15f;

    // MIRRORED FROM YOUR PLAYER MOVEMENT SCRIPT
    [Header("Gravity & Grounding")]
    [SerializeField] private float gravity = -30f;
    [SerializeField] private Transform groundCheck; // Place an empty object at the bottom of the wheels
    [SerializeField] private LayerMask ground;
    private float velocityY;
    private bool isGrounded;

    [Header("Camera Hijack Settings")]
    [SerializeField] private Transform bikeCameraSeat;
    [SerializeField] private float mouseSensitivity = 0.1f;

    [Header("Player Camera Restore Settings")]
    [SerializeField] private Vector3 playerCamWalkLocalPosition = new Vector3(0f, 0.8f, 0f); // Adjust this to your player's exact head height

    [Header("Dismount Settings")]
    [SerializeField] private Vector3 dismountOffset = new Vector3(-1.2f, 0f, 0f);

    private Rigidbody rb;
    private string originalTag;

    private float moveInput = 0f;
    private float turnInput = 0f;
    private float currentSpeed = 0f;
    private float targetSteerAngle = 0f;
    private float currentSteerAngle = 0f;
    private float steerVelocityCache;

    private float camXRotation = 0f;
    private float camYRotation = 0f;

    private GameObject playerRef;
    private Transform cameraRef;
    private Transform cameraOriginalParent;
    private MonoBehaviour playerCamLookScript;
    private Vector3 cameraOriginalLocalPosition; // Stores the exact head-height offset
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalTag = gameObject.tag;

        // --- ADD THESE SAFETY BALANCES ---
        isRiding = false; // Force riding state to false on load
        playerRef = null;
        cameraRef = null;

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        base.OnInteract(dialogueSystem);

        if (isLocked)
        {
            if (PlayerInventory.Instance != null && PlayerInventory.Instance.hasWireCutter)
            {
                isLocked = false;
                if (bikeLockModel != null) bikeLockModel.SetActive(false);
                if (dialogueSystem != null) dialogueSystem.Show(unlockDialogue, 3.0f);

                GetOnBike();
            }
            else
            {
                if (dialogueSystem != null) dialogueSystem.Show(lockedDialogue, 2.5f);
            }
            return;
        }

        if (!isRiding) GetOnBike();
    }

    protected override void Update()
    {
        if (!isRiding)
        {
            base.Update();
            return;
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GetOffBike();
            return;
        }

        // Gather keyboard inputs
        if (Keyboard.current != null)
        {
            moveInput = 0f;
            if (Keyboard.current.wKey.isPressed) moveInput = 1f;
            if (Keyboard.current.sKey.isPressed) moveInput = -1f;

            turnInput = 0f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;
            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
        }

        ProcessClampedCameraLook();
    }

    void FixedUpdate()
    {
        // We ALWAYS apply manual gravity calculations, even if the player isn't riding it,
        // so the bike doesn't randomly float away if pushed off a ledge!
        ApplyManualGravity();

        if (!isRiding) return;

        ProcessEnginePhysics();
        ProcessResponsiveSteering();
    }

    private void ApplyManualGravity()
    {
        // Check if the bike wheels are touching the ground layer
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, ground);
        }
        else
        {
            // Fallback safety if you haven't assigned a ground check point yet
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.0f, ground);
        }

        // Build downward acceleration velocity exactly like your player movement script
        velocityY += gravity * 2f * Time.fixedDeltaTime;

        if (isGrounded && velocityY < -1f)
        {
            velocityY = -4f; // Keeps the bike snapped tightly against downward slopes
        }
    }

    private void ProcessEnginePhysics()
    {
        if (moveInput != 0f)
        {
            currentSpeed += moveInput * accelerationRate * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, frictionDeceleration * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.4f, maxSpeed);

        // Combine horizontal driving velocity with our custom vertical gravity accumulator
        Vector3 physicsVelocity = transform.forward * currentSpeed;
        physicsVelocity.y = velocityY;

        rb.linearVelocity = physicsVelocity;
    }

    private void ProcessResponsiveSteering()
    {
        // Enforce steer rules: can only turn if actively moving forward/backward
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            float directionMultiplier = currentSpeed > 0f ? 1f : -1f;
            targetSteerAngle = turnInput * maxSteerAngle * directionMultiplier;
        }
        else
        {
            targetSteerAngle = 0f;
        }

        currentSteerAngle = Mathf.SmoothDamp(currentSteerAngle, targetSteerAngle, ref steerVelocityCache, steeringSmoothTime);

        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * currentSteerAngle * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void ProcessClampedCameraLook()
    {
        if (Mouse.current == null || cameraRef == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        camYRotation += mouseDelta.x;
        camYRotation = Mathf.Clamp(camYRotation, -45f, 45f);

        camXRotation -= mouseDelta.y;
        camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);

        cameraRef.localRotation = Quaternion.Euler(camXRotation, camYRotation, 0f);
    }

    private void GetOnBike()
    {
        playerRef = GameObject.FindWithTag("Player");
        if (playerRef == null) return;

        gameObject.tag = "Player";

        cameraRef = Camera.main != null ? Camera.main.transform : playerRef.GetComponentInChildren<Camera>().transform;

        if (cameraRef != null)
        {
            playerCamLookScript = cameraRef.GetComponent("PlayerMouseLook") as MonoBehaviour;
            if (playerCamLookScript == null) playerCamLookScript = playerRef.GetComponent("Movement") as MonoBehaviour;
            if (playerCamLookScript != null) playerCamLookScript.enabled = false;

            cameraOriginalParent = cameraRef.parent;
            cameraRef.SetParent(bikeCameraSeat);
            cameraOriginalLocalPosition = cameraRef.localPosition;
            cameraRef.localPosition = Vector3.zero;

            camXRotation = 0f;
            camYRotation = 0f;
            cameraRef.localRotation = Quaternion.identity;
        }

        playerRef.SetActive(false);
        isRiding = true;
    }

    private void GetOffBike()
    {
        isRiding = false;
        gameObject.tag = originalTag;

        if (playerRef != null)
        {
            Vector3 spawnPosition = transform.TransformPoint(dismountOffset);
            playerRef.transform.position = spawnPosition;
            playerRef.transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            playerRef.SetActive(true);

            if (cameraRef != null)
            {
                cameraRef.SetParent(cameraOriginalParent);

                // Manually force the camera back to your exact eye-level coordinates
                cameraRef.localPosition = playerCamWalkLocalPosition;
                cameraRef.localRotation = Quaternion.identity;

                if (playerCamLookScript != null) playerCamLookScript.enabled = true;
            }
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        playerRef = null;
        cameraRef = null;
        cameraOriginalParent = null;
        playerCamLookScript = null;
    }

    // Visual editor circle drawing helper to see the ground check range
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
        }
    }
}