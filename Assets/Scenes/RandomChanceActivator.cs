using UnityEngine;

public class RandomChanceActivator : MonoBehaviour
{
    [Header("Target Object")]
    [Tooltip("Drag the disabled object from your Hierarchy here.")]
    [SerializeField] private GameObject targetObject;

    [Header("Chance Settings")]
    [Range(0f, 100f)]
    [Tooltip("Percentage chance to activate (e.g., 25 = 25% chance)")]
    [SerializeField] private float successChance = 25f;

    [SerializeField] private bool rollOnStart = true;

    private void Start()
    {
        if (rollOnStart)
        {
            EvaluateSpawnChance();
        }
    }

    /// <summary>
    /// Rolls a random number between 0 and 100.
    /// Activates targetObject if the roll is <= successChance.
    /// </summary>
    public void EvaluateSpawnChance()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target Object is not assigned on " + gameObject.name);
            return;
        }

        // Roll a random float between 0.0 and 100.0
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= successChance)
        {
            targetObject.SetActive(true);
            Debug.Log($"Roll: {randomRoll:F1} <= {successChance} — Object Enabled!");
        }
        else
        {
            targetObject.SetActive(false);
            Debug.Log($"Roll: {randomRoll:F1} > {successChance} — Object Kept Disabled.");
        }
    }
}