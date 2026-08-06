using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    [Header("Universal Interaction Settings")]
    [TextArea(2, 3)]
    [SerializeField] private string interactionPrompt = "Press [E] to Interact";
    [SerializeField] private float promptDuration = 2.0f;
    [SerializeField] private Transform uiFloatingPoint;

    private float promptCooldownTimer = 0f;

    public string InteractionPrompt => interactionPrompt;
    public float PromptDuration => promptDuration;
    public bool CanShowPrompt => promptCooldownTimer <= 0f;

    protected virtual void Update()
    {
        if (promptCooldownTimer > 0f)
        {
            promptCooldownTimer -= Time.deltaTime;
        }
    }

    public Vector3 GetUIPosition()
    {
        if (uiFloatingPoint != null) return uiFloatingPoint.position;
        return transform.position + Vector3.up * 1.5f;
    }

    // FIX: Removed Vector2 position from the arguments so it doesn't force a move!
    public virtual void OnInteract(InteractionPromptUI dialogueSystem)
    {
        StartPromptCooldown(3.0f);
    }

    public void StartPromptCooldown(float duration)
    {
        promptCooldownTimer = duration;
    }
}