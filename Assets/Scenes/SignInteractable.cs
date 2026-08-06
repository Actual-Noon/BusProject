using UnityEngine;

public class SignInteractable : InteractableBase
{
    [Header("Sign Specific Settings")]
    [TextArea(2, 4)]
    [SerializeField] private string signReadMessage = "The sign reads: STOP.";
    [SerializeField] private float messageDisplayDuration = 1.0f;

    // FIX: Removed Vector2 positioning logic here too
    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        base.StartPromptCooldown(messageDisplayDuration + 0.5f);

        if (dialogueSystem != null)
        {
            // By only sending the message and duration, it honors your Unity Inspector placement!
            dialogueSystem.Show(signReadMessage, messageDisplayDuration);
        }
    }
}