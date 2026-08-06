using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class WinZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string winSceneName = "WinScene"; // Name of your Win scene

    // This function automatically runs when something enters the invisible trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is actually the Player
        // (Make sure your Player GameObject has the "Player" Tag in the Inspector!)
        if (other.CompareTag("Player"))
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        // 1. Unlock the cursor for the Win Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Load the Win Scene
        SceneManager.LoadScene(winSceneName);
    }
}