using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueInteractable : InteractableBase
{
    [Header("Developer Pre-Set Dialogue")]
    [TextArea(3, 5)]
    [SerializeField]
    private List<string> developerDialogueLines = new List<string>()
    {
        "Greetings traveler!",
        "Be careful, monsters wander these woods.",
        "Good luck out there!"
    };

    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        base.OnInteract(dialogueSystem);

        // Prevent opening if already talking
        if (DialogueUI.IsDialogueActive) return;

        // Trigger dialogue from DialogueUI singleton
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(developerDialogueLines);
        }
    }
}