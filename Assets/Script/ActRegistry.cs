using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds an in-memory lookup of every ActData asset under Resources/Acts/...
/// (any subfolder depth — Resources.LoadAll recurses automatically) so other
/// systems (SaveManager) can resolve an ActData purely from its string actID,
/// without needing a manual scene reference to every single Act asset.
///
/// Attach to: a persistent bootstrap GameObject, singleton, DontDestroyOnLoad.
/// Must run BEFORE SaveManager.Load() needs to resolve an actID.
/// </summary>
public class ActRegistry : MonoBehaviour
{
    public static ActRegistry Instance { get; private set; }

    [Tooltip("Resources-relative path (no leading/trailing slash) where all ActData assets live, e.g. 'Acts'.")]
    [SerializeField] private string resourcesRootPath = "Acts";

    private Dictionary<string, ActData> _registry = new Dictionary<string, ActData>();

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildRegistry();
    }

    private void BuildRegistry()
    {
        _registry.Clear();

        ActData[] allActs = Resources.LoadAll<ActData>(resourcesRootPath);

        foreach (var act in allActs)
        {
            if (string.IsNullOrWhiteSpace(act.actID))
            {
                Debug.LogWarning($"[ActRegistry] ActData '{act.name}' has no actID, skipping.");
                continue;
            }

            if (_registry.ContainsKey(act.actID))
            {
                Debug.LogError($"[ActRegistry] Duplicate actID '{act.actID}' found on '{act.name}' " +
                                $"and '{_registry[act.actID].name}'. actID must be unique!");
                continue;
            }

            _registry.Add(act.actID, act);
        }

        IsReady = true;
        Debug.Log($"[ActRegistry] Loaded {_registry.Count} ActData assets from Resources/{resourcesRootPath}.");
    }

    /// <summary>Look up an ActData by its actID. Returns null if not found.</summary>
    public ActData GetByID(string actID)
    {
        if (string.IsNullOrEmpty(actID)) return null;

        if (!_registry.TryGetValue(actID, out ActData act))
        {
            Debug.LogError($"[ActRegistry] No ActData found with actID '{actID}'.");
            return null;
        }

        return act;
    }

    /// <summary>Force a re-scan — useful in editor tooling, not normally needed at runtime.</summary>
    public void Rebuild() => BuildRegistry();
}