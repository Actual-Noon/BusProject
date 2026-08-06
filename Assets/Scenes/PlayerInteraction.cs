using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Range")]
    [SerializeField] private float interactDistance = 4.0f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI System References")]
    [SerializeField] private InteractionPromptUI interactionPromptSystem; // Assign "InteractBg" here
    [SerializeField] private InteractionPromptUI dialogueBubbleSystem;    // Assign "Text Background" here

    private Camera playerCam;
    private InteractableBase currentTarget;

    void Awake()
    {
        playerCam = GetComponent<Camera>();

        if (playerCam == null)
        {
            Debug.LogError("CRITICAL: PlayerInteraction script must be placed directly onto your Main Camera object!");
        }
    }

    void Update()
    {
        // 1. If game is paused OR dialogue is open, hide prompts and block new E presses
        if (PauseMenu.GameIsPaused || DialogueUI.IsDialogueActive)
        {
            if (interactionPromptSystem != null)
            {
                interactionPromptSystem.Hide();
            }
            currentTarget = null;
            return;
        }

        // 2. Otherwise run normal raycast detection
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        if (playerCam == null || interactionPromptSystem == null || dialogueBubbleSystem == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();

            if (interactable != null)
            {
                currentTarget = interactable;

                Vector3 screenPos = playerCam.WorldToScreenPoint(currentTarget.GetUIPosition());
                Vector2 localPoint;

                Canvas parentCanvas = interactionPromptSystem.GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    Debug.LogError("Missing Canvas! Ensure your UI element is a child of a Canvas.");
                    return;
                }

                RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);

                if (currentTarget.CanShowPrompt)
                {
                    interactionPromptSystem.Show(currentTarget.InteractionPrompt, localPoint, currentTarget.PromptDuration);
                }

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    // FIX: Removed 'localPoint' from this function call. 
                    // This stops the dialogue text from being moved by the Raycast position!
                    currentTarget.OnInteract(dialogueBubbleSystem);

                    interactionPromptSystem.Hide();
                }

                return;
            }
        }

        if (currentTarget != null)
        {
            interactionPromptSystem.Hide();
        }

        currentTarget = null;
    }
}