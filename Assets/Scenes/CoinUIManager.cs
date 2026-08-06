using UnityEngine;
using TMPro; // Required for TextMeshPro

public class CoinUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private string prefix = "Coins: ";

    private void OnEnable()
    {
        // Subscribe to the inventory event
        PlayerInventory.OnCoinsChanged += UpdateCoinDisplay;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks when the scene changes
        PlayerInventory.OnCoinsChanged -= UpdateCoinDisplay;
    }

    private void Start()
    {
        // Initialize the text correctly at the start of the scene
        if (PlayerInventory.Instance != null)
        {
            UpdateCoinDisplay(PlayerInventory.Instance.coinCount);
        }
    }

    private void UpdateCoinDisplay(int currentCoins)
    {
        if (coinText != null)
        {
            coinText.text = prefix + currentCoins.ToString();
        }
    }
}