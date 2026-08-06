using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // A custom data structure to pair an item with its drop probability
    [System.Serializable]
    public struct SpawnableItem
    {
        public string itemName; // Just for organization in the Inspector
        public GameObject itemPrefab; // The item prefab (Coin, WireCutter, Potion, etc.)
        [Range(0f, 100f)] public float spawnChance; // The percentage chance this item spawns
    }

    [Header("Spawner Settings")]
    [SerializeField] private List<SpawnableItem> poolOfItems;

    [Header("Spawning Behavior")]
    [Tooltip("If true, each anchor chooses randomly from the pool. If false, it loops through the pool items sequentially.")]
    [SerializeField] private bool pickRandomFromPool = true;

    private void Start()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        if (poolOfItems == null || poolOfItems.Count == 0)
        {
            Debug.LogWarning("ItemSpawner: No items added to the spawn pool!");
            return;
        }

        int itemIndex = 0;

        // Auto-detects and loops through all child anchors exactly like before!
        foreach (Transform childAnchor in transform)
        {
            // Determine which item configuration we are evaluating for this anchor
            SpawnableItem itemToTry;

            if (pickRandomFromPool)
            {
                // Grab a completely random item setup from your pool
                int randomIndex = Random.Range(0, poolOfItems.Count);
                itemToTry = poolOfItems[randomIndex];
            }
            else
            {
                // Cycle through the items in order (Anchor 1 tries Item A, Anchor 2 tries Item B...)
                itemToTry = poolOfItems[itemIndex];
                itemIndex = (itemIndex + 1) % poolOfItems.Count; // Wraps back to 0 if it hits the end
            }

            // Check if it passes the item's specific percentage check
            if (itemToTry.itemPrefab != null)
            {
                float roll = Random.Range(0f, 100f);

                if (roll <= itemToTry.spawnChance)
                {
                    // Spawn the item at the child anchor's position
                    Instantiate(itemToTry.itemPrefab, childAnchor.position, childAnchor.rotation, childAnchor);
                }
            }
        }
    }
}