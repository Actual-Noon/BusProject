using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Setup")]
    [Tooltip("Drag your Car Prefab here (it must have an Animator component attached)")]
    public GameObject carPrefab;

    [Tooltip("Where the car will spawn")]
    public Transform spawnPoint;

    [Header("Spawn Odds")]
    [Tooltip("How often to pick a number (in seconds)")]
    public float checkInterval = 0.5f;

    [Tooltip("Threshold required to spawn the car (> 90)")]
    public int threshold = 90;

    void Start()
    {
        // Start the repeating loop
        StartCoroutine(RandomSpawnLoop());
    }

    IEnumerator RandomSpawnLoop()
    {
        while (true)
        {
            // Wait 0.5 seconds before running the logic
            yield return new WaitForSeconds(checkInterval);

            // Pick a random integer from 0 to 100 (inclusive)
            int randomNumber = Random.Range(0, 101);

            if (randomNumber > threshold)
            {
                SpawnCar();
            }
        }
    }

    void SpawnCar()
    {
        // 1. Spawn the car at the spawnPoint position & rotation
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject spawnedCar = Instantiate(carPrefab, spawnPos, spawnRot);

        // 2. Get the Animator component from the spawned car
        Animator animator = spawnedCar.GetComponent<Animator>();

        if (animator != null)
        {
            // Get the current animation clip length (duration in seconds)
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animLength = stateInfo.length;

            // 3. Destroy the car after its animation duration finishes
            Destroy(spawnedCar, animLength);
        }
        else
        {
            Debug.LogWarning("Spawned car does not have an Animator component!");
        }
    }
}