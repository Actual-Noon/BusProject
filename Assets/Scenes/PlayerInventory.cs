using System; // Required for Action
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory Items")]
    public bool hasWireCutter = false;

    // An event that other scripts (like the UI) can listen to
    public static event Action<int> OnCoinsChanged;

    [SerializeField] private int _coinCount = 0;
    public int coinCount
    {
        get => _coinCount;
        set
        {
            _coinCount = value;
            // Whenever coins change, alert the UI and pass the new total
            OnCoinsChanged?.Invoke(_coinCount);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}