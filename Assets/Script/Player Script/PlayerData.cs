using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    private const string SAVE_KEY = "Game_PlayerData";

    // --- Movement ---
    [SerializeField] private Direction playerDirection;
    [SerializeField] private float playerSpeed = 4.5f;

    public Direction PlayerDirection { get { return playerDirection; } set { playerDirection = value; } }
    public float PlayerSpeed { get { return playerSpeed; } }

    // --- Stats ---
    [Header("Stats (0–100)")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float energy = 100f;
    [SerializeField] private float money = 100f;
    [SerializeField] private float social = 100f;

    public float Health => health;
    public float Energy => energy;
    public float Money => money;
    public float Social => social;

    /// <summary>
    /// Fired whenever any stat changes — UI (sliders/HUD) subscribes to this
    /// to refresh without PlayerData needing to know about UI directly.
    /// </summary>
    public event Action OnStatsChanged;

    // ─────────────────────────────────────────────
    //  STAT MUTATION
    // ─────────────────────────────────────────────

    /// <summary>Apply a StatVariable delta to this player's stats.</summary>
    public void ApplyStatEffect(StatVariable effect)
    {
        health = Mathf.Clamp(health + effect.Health, 0f, 100f);
        energy = Mathf.Clamp(energy + effect.Energy, 0f, 100f);
        money = Mathf.Clamp(money + effect.Money, 0f, 100f);
        social = Mathf.Clamp(social + effect.Social, 0f, 100f);

        Debug.Log($"[PlayerData] Stats updated → HP:{health} EN:{energy} MO:{money} SO:{social}");

        OnStatsChanged?.Invoke();
        Save();
    }

    /// <summary>Reset all stats to full (call on new game).</summary>
    public void ResetStats()
    {
        health = energy = money = social = 100f;
        OnStatsChanged?.Invoke();
        Save();
    }

    // ─────────────────────────────────────────────
    //  PERSISTENCE (PlayerPrefs, same pattern as ActionLogger)
    // ─────────────────────────────────────────────

    [System.Serializable]
    private class SaveBlob
    {
        public float health;
        public float energy;
        public float money;
        public float social;
        public Direction playerDirection;
    }

    /// <summary>Serialize current stats to PlayerPrefs as JSON.</summary>
    public void Save()
    {
        SaveBlob blob = new SaveBlob
        {
            health = health,
            energy = energy,
            money = money,
            social = social,
            playerDirection = playerDirection
        };

        string json = JsonUtility.ToJson(blob);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[PlayerData] Saved: " + json);
    }

    /// <summary>
    /// Load stats from PlayerPrefs if present. Call this once on game start
    /// (e.g. from a bootstrap/GameManager Awake) BEFORE any UI reads PlayerData.
    /// If no save exists, stats keep their ScriptableObject default/inspector values.
    /// </summary>
    public void Load()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[PlayerData] No save found, using defaults.");
            OnStatsChanged?.Invoke();
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveBlob blob = JsonUtility.FromJson<SaveBlob>(json);

        health = blob.health;
        energy = blob.energy;
        money = blob.money;
        social = blob.social;
        playerDirection = blob.playerDirection;

        Debug.Log("[PlayerData] Loaded: " + json);
        OnStatsChanged?.Invoke();
    }

    /// <summary>Wipe save data (e.g. for a "New Game" button).</summary>
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        ResetStats();
    }
}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right
}