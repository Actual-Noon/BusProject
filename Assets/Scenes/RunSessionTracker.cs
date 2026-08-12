using UnityEngine;

public class RunSessionTracker : MonoBehaviour
{
    public static RunSessionTracker Instance { get; private set; }

    public float TotalRunTime { get; private set; }
    public float WalkHoldTime { get; private set; }
    public bool PendingRunWon { get; private set; }
    public int PendingCoins { get; private set; }
    public bool HasPendingRun { get; private set; }

    private bool isTracking = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewRun()
    {
        TotalRunTime = 0f;
        WalkHoldTime = 0f;
        HasPendingRun = false;
        isTracking = true;
    }

    private void Update()
    {
        if (isTracking && !PauseMenu.GameIsPaused && !DialogueUI.IsDialogueActive)
        {
            TotalRunTime += Time.deltaTime;
        }
    }

    public void AddWalkHoldTime(float duration)
    {
        if (isTracking && !PauseMenu.GameIsPaused && !DialogueUI.IsDialogueActive)
        {
            WalkHoldTime += duration;
        }
    }

    public void EndRun(bool isWin)
    {
        if (!isTracking) return;

        isTracking = false;
        PendingRunWon = isWin;
        PendingCoins = PlayerInventory.Instance != null ? PlayerInventory.Instance.coinCount : 0;
        HasPendingRun = true;
    }

    public void ClearPendingRun()
    {
        HasPendingRun = false;
    }
}