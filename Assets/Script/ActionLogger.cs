using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Records every player choice made during Phase Action across the WHOLE
/// playthrough (Day 1 → Day 3, ~7 actions total). Logs persist via PlayerPrefs
/// so they survive scene transitions between days.
///
/// AIManager reads the full accumulated log list once, at the end of Day 3,
/// to generate the single final summary.
/// </summary>
public class ActionLogger : MonoBehaviour
{
    public static ActionLogger Instance;

    [Tooltip("All logs from the entire playthrough (Day 1 to final day)")]
    public List<LogEntry> allLogs = new List<LogEntry>();

    private const string SAVE_KEY = "Game_Action_Logs";

    public List<LogEntry> AllLogs => allLogs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLogs();
    }

    /// <summary>
    /// Logs a single Phase Action choice. Called from CardMode when the
    /// player confirms their pick ("Pilih Aksi") — NOT during Consequence phase.
    /// </summary>
    public void AddLog(string title, string source, string actionName, CopingTag tag, StatVariable statVariable)
    {
        LogEntry newEntry = new LogEntry(title, source, actionName, tag, statVariable);
        allLogs.Add(newEntry);

        SaveLogs();
        Debug.Log($"[ActionLogger] Logged: [{tag}] {title} → {actionName} (source: {source})");
    }

    /// <summary>Returns logs filtered by Act ID prefix, useful for a per-day debug view.</summary>
    public List<LogEntry> GetLogsBySourcePrefix(string actIdPrefix)
    {
        return allLogs.FindAll(l => !string.IsNullOrEmpty(l.source) && l.source.StartsWith(actIdPrefix));
    }

    // ─────────────────────────────────────────────
    //  PERSISTENCE
    // ─────────────────────────────────────────────

    public void SaveLogs()
    {
        LogWrapper wrapper = new LogWrapper { logs = allLogs };
        string json = JsonUtility.ToJson(wrapper);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[ActionLogger] Saved: " + json);
    }

    public void LoadLogs()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[ActionLogger] No saved logs found.");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        LogWrapper wrapper = JsonUtility.FromJson<LogWrapper>(json);

        allLogs = wrapper.logs ?? new List<LogEntry>();
        Debug.Log($"[ActionLogger] Loaded {allLogs.Count} logs.");
    }

    /// <summary>Wipe all logs — call when starting a brand new playthrough.</summary>
    public void ResetLogs()
    {
        allLogs.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}