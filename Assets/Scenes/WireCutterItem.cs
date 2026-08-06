using UnityEngine;

public class WireCutterItem : InteractableBase
{
    [Header("Pickup Settings")]
    [SerializeField] private string pickupMessage = "Picked up the Wire Cutter!";
    [SerializeField] private float displayDuration = 2.5f;

    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        // Start cooldown protection from the base script
        base.OnInteract(dialogueSystem);

        if (PlayerInventory.Instance != null)
        {
            // 1. Grant the item to the inventory
            PlayerInventory.Instance.hasWireCutter = true;

            // 2. Output the notification dialogue message
            if (dialogueSystem != null)
            {
                dialogueSystem.Show(pickupMessage, displayDuration);
            }

            // 3. Make the object disappear cleanly from the world map(No saving)
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ERROR: PlayerInventory manager instance was not found in this scene!");
        }
    }
}