using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class RunData
{
    public float totalTime;        // Total seconds in the run
    public float walkHoldTime;     // Total seconds spent holding movement keys
    public int coinsCollected;
    public bool isWin;
    public string dateSaved;
}

[Serializable]
public class SaveDataHolder
{
    public List<RunData> savedRuns = new List<RunData>();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveFilePath;
    public SaveDataHolder CurrentData { get; private set; } = new SaveDataHolder();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "run_history.json");
            LoadRuns();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveRun(float totalTime, float walkHoldTime, int coins, bool won)
    {
        RunData newRun = new RunData
        {
            totalTime = totalTime,
            walkHoldTime = walkHoldTime,
            coinsCollected = coins,
            isWin = won,
            dateSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };

        CurrentData.savedRuns.Add(newRun);

        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Run saved to {saveFilePath}");
    }

    public void LoadRuns()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            CurrentData = JsonUtility.FromJson<SaveDataHolder>(json);
        }
        else
        {
            CurrentData = new SaveDataHolder();
        }
    }

    public int GetBestRunIndex()
    {
        var runs = CurrentData.savedRuns;
        if (runs == null || runs.Count == 0) return -1;

        int bestIndex = -1;
        float fastestTime = float.MaxValue; // Start with a huge number to find the minimum time

        for (int i = 0; i < runs.Count; i++)
        {
            // 1. MUST be a winning run
            // 2. MUST be faster than the previously recorded fastest winning run
            if (runs[i].isWin && runs[i].totalTime < fastestTime)
            {
                fastestTime = runs[i].totalTime;
                bestIndex = i;
            }
        }

        // Returns the index of the fastest winning run, or -1 if no winning run exists yet
        return bestIndex;
    }
}