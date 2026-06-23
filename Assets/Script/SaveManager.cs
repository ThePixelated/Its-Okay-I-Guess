using System;
using UnityEngine;

/// <summary>
/// Tracks and persists the player's story progress: which Act/step they're on
/// and their last world position. Saved granularly — every AdvanceStep() call —
/// so a crash/close loses at most one step of progress.
///
/// Does NOT own PlayerData stats, ActionLogger logs, or EncyclopediaProgress —
/// those are separate systems with their own save keys. SaveManager only owns
/// "where is the player in the story right now".
///
/// Designed to be modular for New Game vs Continue, even though only Continue
/// is wired into UI right now.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_KEY = "Game_Progress";

    public bool HasActiveSave => PlayerPrefs.HasKey(SAVE_KEY);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    private class ProgressBlob
    {
        public string currentActID;
        public int stepIndex;
        public float posX, posY, posZ;
        public string sceneName;
    }

    // ─────────────────────────────────────────────
    //  SAVE
    // ─────────────────────────────────────────────

    /// <summary>Call after every AdvanceStep() in PrimaryQuestManager.</summary>
    public void SaveProgress(string currentActID, int stepIndex, Vector3 playerPosition, string sceneName)
    {
        ProgressBlob blob = new ProgressBlob
        {
            currentActID = currentActID,
            stepIndex = stepIndex,
            posX = playerPosition.x,
            posY = playerPosition.y,
            posZ = playerPosition.z,
            sceneName = sceneName
        };

        string json = JsonUtility.ToJson(blob);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[SaveManager] Progress saved → Act '{currentActID}' step {stepIndex}.");
    }

    // ─────────────────────────────────────────────
    //  LOAD
    // ─────────────────────────────────────────────

    public struct ProgressData
    {
        public ActData act;
        public int stepIndex;
        public Vector3 playerPosition;
        public string sceneName;
        public bool isValid;
    }

    /// <summary>
    /// Reads saved progress and resolves the actID into an actual ActData via
    /// ActRegistry. Returns isValid=false if there's no save or the registry
    /// can't resolve the stored actID (e.g. asset renamed/deleted).
    /// </summary>
    public ProgressData LoadProgress()
    {
        if (!HasActiveSave)
            return new ProgressData { isValid = false };

        string json = PlayerPrefs.GetString(SAVE_KEY);
        ProgressBlob blob = JsonUtility.FromJson<ProgressBlob>(json);

        if (ActRegistry.Instance == null)
        {
            Debug.LogError("[SaveManager] ActRegistry.Instance is null — cannot resolve actID.");
            return new ProgressData { isValid = false };
        }

        ActData resolvedAct = ActRegistry.Instance.GetByID(blob.currentActID);
        if (resolvedAct == null)
        {
            Debug.LogError($"[SaveManager] Could not resolve actID '{blob.currentActID}' from save data.");
            return new ProgressData { isValid = false };
        }

        return new ProgressData
        {
            act = resolvedAct,
            stepIndex = blob.stepIndex,
            playerPosition = new Vector3(blob.posX, blob.posY, blob.posZ),
            sceneName = blob.sceneName,
            isValid = true
        };
    }

    // ─────────────────────────────────────────────
    //  NEW GAME / CONTINUE — modular entry points
    // ─────────────────────────────────────────────

    /// <summary>
    /// Wipes ALL save data across every system (progress, stats, logs,
    /// encyclopedia) — call this when the player picks "New Game".
    /// Not wired to UI yet, but ready when that flow exists.
    /// </summary>
    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);

        if (PlayerData_StaticAccessor.Current != null)
            PlayerData_StaticAccessor.Current.DeleteSave();

        if (ActionLogger.Instance != null)
            ActionLogger.Instance.ResetLogs();

        if (EncyclopediaProgress.Instance != null)
            EncyclopediaProgress.Instance.ResetProgress();

        Debug.Log("[SaveManager] New game started — all save data wiped.");
    }

    /// <summary>
    /// Returns the data PrimaryQuestManager needs to resume exactly where the
    /// player left off. Call this instead of using `startingAct` when
    /// HasActiveSave is true.
    /// </summary>
    public ProgressData ContinueGame()
    {
        return LoadProgress();
    }
}

/// <summary>
/// Tiny helper so SaveManager can reach the active PlayerData asset without
/// a direct SerializeField wiring requirement. Set this once from
/// GameModeManager.Awake() (it already holds a PlayerData reference).
/// </summary>
public static class PlayerData_StaticAccessor
{
    public static PlayerData Current;
}