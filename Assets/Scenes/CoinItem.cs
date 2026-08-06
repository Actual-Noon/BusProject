using UnityEngine;

public class CoinItem : InteractableBase
{
    [Header("Pickup UI Settings")]
    [SerializeField] private string pickupMessage = "Collected coins!";
    [SerializeField] private float displayDuration = 2.0f;

    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        base.OnInteract(dialogueSystem);

        if (PlayerInventory.Instance != null)
        {
            // 1. Calculate the random coin value based on weights
            int coinsToAdd = DetermineRandomCoinValue();

            // 2. Add coins to the inventory total
            PlayerInventory.Instance.coinCount += coinsToAdd;

            // 3. Output the notification dialogue message
            if (dialogueSystem != null)
            {
                string customizedMessage = $"{pickupMessage} (+{coinsToAdd})";
                dialogueSystem.Show(customizedMessage, displayDuration);
            }

            // 4. Make the object disappear cleanly
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ERROR: PlayerInventory manager instance was not found in this scene!");
        }
    }

    private int DetermineRandomCoinValue()
    {
        // Generate a random number between 0.0 and 100.0
        float roll = Random.Range(0f, 100f);

        // Define your probability brackets:
        // 10% chance for 10 coins (e.g., if roll is less than 10)
        if (roll < 10f)
        {
            return 10;
        }
        // 15% chance for 5 coins (e.g., if roll is between 10 and 25)
        else if (roll < 25f)
        {
            return 5;
        }
        // 25% chance for 2 coins (e.g., if roll is between 25 and 50)
        else if (roll < 50f)
        {
            return 2;
        }
        // 50% chance for 1 coin (e.g., if roll is between 50 and 100)
        else
        {
            return 1;
        }
    }
}