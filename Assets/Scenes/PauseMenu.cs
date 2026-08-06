using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    // Keeping this static is fine, but we must explicitly reset it
    public static bool GameIsPaused = false;
    public static event Action<bool> OnPauseToggled;

    public GameObject pauseMenuUI;

    void Awake()
    {
        // FIX: Ensure the pause state is completely cleared out 
        // whenever this script loads into a fresh scene.
        GameIsPaused = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnPauseToggled?.Invoke(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnPauseToggled?.Invoke(true);
    }

    public void LoadMenu()
    {
        // FIX: Clean up the state explicitly before jumping scenes
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}