using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RunViewerUI : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Save Prompt Panel (Game Over / Win Scenes)")]
    public GameObject savePromptPanel;
    public Button saveYesButton;
    public Button saveNoButton;

    [Header("Stats Viewer Panel (Main Menu)")]
    public GameObject statsPanel;
    public TextMeshProUGUI indexText;        // Displays "1 / 10"
    public TextMeshProUGUI timeText;         // Total Run Duration
    public TextMeshProUGUI walkHoldText;     // Time spent moving
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI dateText;

    [Header("Navigation Controls")]
    public Button nextButton;
    public Button prevButton;
    public Button bestRunButton;

    private int currentIndex = 0;

    private void Start()
    {
        // Make sure the stats panel stays hidden by default on scene start
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }

        // CASE 1: In Game Over or Win Scene with an unsaved run pending
        if (RunSessionTracker.Instance != null && RunSessionTracker.Instance.HasPendingRun)
        {
            if (savePromptPanel != null) savePromptPanel.SetActive(true);
        }
        // CASE 2: Viewing directly in Main Menu
        else
        {
            if (savePromptPanel != null) savePromptPanel.SetActive(false);
            // We only pre-load data in the background without forcing statsPanel active
            PrepareDataForViewer();
        }
    }

    // Separate data loading from UI activation
    private void PrepareDataForViewer()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.CurrentData == null)
        {
            ShowEmptyState();
            return;
        }

        var runs = SaveManager.Instance.CurrentData.savedRuns;
        if (runs != null && runs.Count > 0)
        {
            currentIndex = runs.Count - 1; // Default to latest run
            DisplayRun(currentIndex);
        }
        else
        {
            ShowEmptyState();
        }
    }

    // Call THIS function on your Main Menu's "Data" Button OnClick event!
    public void OpenDataMenu()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(true);
        }

        PrepareDataForViewer();
    }

    public void ShowSavedRunsList()
    {
        if (statsPanel != null) statsPanel.SetActive(true);

        // Check if SaveManager.Instance exists before accessing it
        if (SaveManager.Instance == null || SaveManager.Instance.CurrentData == null)
        {
            Debug.LogWarning("SaveManager instance was not found! Make sure a SaveManager GameObject is present in the scene.");
            ShowEmptyState();
            return;
        }

        var runs = SaveManager.Instance.CurrentData.savedRuns;
        if (runs != null && runs.Count > 0)
        {
            currentIndex = runs.Count - 1; // Default view to the latest run
            DisplayRun(currentIndex);
        }
        else
        {
            ShowEmptyState();
        }
    }

    // --- GAME OVER / WIN PROMPT BUTTON ACTIONS ---

    public void OnClickSaveYes()
    {
        var run = RunSessionTracker.Instance;
        if (run != null && run.HasPendingRun)
        {
            Debug.Log($"[Save Test] Saving Run -> Time: {run.TotalRunTime}, Coins: {run.PendingCoins}");

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveRun(run.TotalRunTime, run.WalkHoldTime, run.PendingCoins, run.PendingRunWon);
            }

            run.ClearPendingRun();
        }
        else
        {
            Debug.LogError("[Save Test] No pending run found in RunSessionTracker!");
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnClickSaveNo()
    {
        if (RunSessionTracker.Instance != null)
        {
            RunSessionTracker.Instance.ClearPendingRun();
        }

        // Return immediately to the Main Menu
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- DISPLAY & NAVIGATION ---

    public void DisplayRun(int index)
    {
        var runs = SaveManager.Instance.CurrentData.savedRuns;
        if (runs == null || runs.Count == 0)
        {
            ShowEmptyState();
            return;
        }

        currentIndex = Mathf.Clamp(index, 0, runs.Count - 1);
        RunData data = runs[currentIndex];

        // Format run duration
        int runMinutes = Mathf.FloorToInt(data.totalTime / 60F);
        int runSeconds = Mathf.FloorToInt(data.totalTime % 60F);

        // Format movement hold duration
        int walkMinutes = Mathf.FloorToInt(data.walkHoldTime / 60F);
        int walkSeconds = Mathf.FloorToInt(data.walkHoldTime % 60F);

        if (timeText != null) timeText.text = $"Time: {runMinutes:00}:{runSeconds:00}";
        if (walkHoldText != null) walkHoldText.text = $"Steps: {walkMinutes:00}:{walkSeconds:00}";
        if (coinsText != null) coinsText.text = $"Coin: {data.coinsCollected}";
        if (resultText != null) resultText.text = data.isWin ? "Result: Win" : "Result: Game Over";
        if (dateText != null) dateText.text = $"Date: {data.dateSaved}";

        if (indexText != null) indexText.text = $"{currentIndex + 1} / {runs.Count}";

        if (prevButton != null) prevButton.interactable = currentIndex > 0;
        if (nextButton != null) nextButton.interactable = currentIndex < runs.Count - 1;
    }

    public void OnNextButtonClicked() => DisplayRun(currentIndex + 1);
    public void OnPrevButtonClicked() => DisplayRun(currentIndex - 1);

    public void OnBestRunClicked()
    {
        if (SaveManager.Instance == null) return;

        int bestIndex = SaveManager.Instance.GetBestRunIndex();
        if (bestIndex != -1)
        {
            DisplayRun(bestIndex);
        }
    }

    private void ShowEmptyState()
    {
        if (indexText != null) indexText.text = "0 / 0";
        if (timeText != null) timeText.text = "Total Time: --";
        if (walkHoldText != null) walkHoldText.text = "Walk Duration: --";
        if (coinsText != null) coinsText.text = "Coins: --";
        if (resultText != null) resultText.text = "No saved runs found.";
        if (prevButton != null) prevButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
    }
}