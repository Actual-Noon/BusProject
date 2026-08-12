using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class KillZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Type the exact name of your Game Over scene as it appears in Build Settings.")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    // Runs automatically when another Collider enters this object's Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone has the "Player" tag
        if (other.CompareTag("Player"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (RunSessionTracker.Instance != null)
        {
            RunSessionTracker.Instance.EndRun(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(gameOverSceneName);
    }
}