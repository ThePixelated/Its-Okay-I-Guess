using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks which EncyclopediaEntry IDs are unlocked THIS playthrough.
/// Separate from EncyclopediaEntry (which is a shared design-time asset) —
/// unlock state is per-save, not per-asset.
/// </summary>
public class EncyclopediaProgress : MonoBehaviour
{
    public static EncyclopediaProgress Instance { get; private set; }

    private const string SAVE_KEY = "Game_EncyclopediaProgress";

    [Tooltip("Assign the master EncyclopediaDatabase here so default-unlocked entries can be applied on first load.")]
    [SerializeField] private EncyclopediaDatabase database;

    private HashSet<string> _unlockedEntryIDs = new HashSet<string>();

    /// <summary>Fired whenever a new entry gets unlocked — UI can subscribe for unlock animations/notifications.</summary>
    public event Action<string> OnEntryUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    public bool IsUnlocked(string entryID)
    {
        if (string.IsNullOrEmpty(entryID)) return false;
        return _unlockedEntryIDs.Contains(entryID);
    }

    public bool IsUnlocked(EncyclopediaEntry entry)
    {
        return entry != null && IsUnlocked(entry.entryID);
    }

    /// <summary>Unlock an entry by ID. No-op if already unlocked.</summary>
    public void UnlockEntry(string entryID)
    {
        if (string.IsNullOrEmpty(entryID)) return;
        if (_unlockedEntryIDs.Contains(entryID)) return;

        _unlockedEntryIDs.Add(entryID);
        Save();

        Debug.Log($"[EncyclopediaProgress] Unlocked: {entryID}");
        OnEntryUnlocked?.Invoke(entryID);
    }

    /// <summary>Convenience overload — unlock directly from an EncyclopediaEntry reference.</summary>
    public void UnlockEntry(EncyclopediaEntry entry)
    {
        if (entry == null) return;
        UnlockEntry(entry.entryID);
    }

    // ─────────────────────────────────────────────
    //  PERSISTENCE
    // ─────────────────────────────────────────────

    [System.Serializable]
    private class SaveBlob
    {
        public List<string> unlockedIDs;
    }

    public void Save()
    {
        SaveBlob blob = new SaveBlob { unlockedIDs = new List<string>(_unlockedEntryIDs) };
        string json = JsonUtility.ToJson(blob);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        _unlockedEntryIDs.Clear();

        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveBlob blob = JsonUtility.FromJson<SaveBlob>(json);

            if (blob.unlockedIDs != null)
                _unlockedEntryIDs = new HashSet<string>(blob.unlockedIDs);
        }

        // Apply any entries flagged as unlocked-by-default that aren't saved yet
        if (database != null)
        {
            foreach (var entry in database.entries)
            {
                if (entry.isUnlockedByDefault && !_unlockedEntryIDs.Contains(entry.entryID))
                    _unlockedEntryIDs.Add(entry.entryID);
            }
        }

        Debug.Log($"[EncyclopediaProgress] Loaded {_unlockedEntryIDs.Count} unlocked entries.");
    }

    /// <summary>Wipe progress — call when starting a brand new playthrough.</summary>
    public void ResetProgress()
    {
        _unlockedEntryIDs.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Load(); // re-apply defaults
    }
}