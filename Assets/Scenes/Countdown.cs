using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = 180f;
    private bool timerIsRunning = false;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Scene Settings")]
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Pause Transition Settings")]
    [SerializeField] private Vector2 gameplayPosition;
    [SerializeField] private Vector2 pausedPosition;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        timerIsRunning = true;
        DisplayTime(timeRemaining);
    }

    private void OnEnable()
    {
        PauseMenu.OnPauseToggled += HandlePauseToggled;
    }

    private void OnDisable()
    {
        PauseMenu.OnPauseToggled -= HandlePauseToggled;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Reverted back to the standard frame calculation line
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                GameOver();
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
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

    private void HandlePauseToggled(bool isPaused)
    {
        rectTransform.anchoredPosition = isPaused ? pausedPosition : gameplayPosition;
    }
    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}