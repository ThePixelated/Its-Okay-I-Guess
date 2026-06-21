using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents one Act in the Primary Quest flow.
/// An Act is an ordered chain of steps (Dialogue, Quest, or CardAction).
/// Acts are linked via nextAct to form the full story chain.
/// </summary>
[CreateAssetMenu(fileName = "ActData", menuName = "Scriptable Objects/ActData")]
public class ActData : ScriptableObject
{
    [Header("Act Identity")]
    public string actID;

    [Tooltip("Required if type = CardAction — assign the DataChapter SO here")]
    public DataChapter dataChapter;

    [Header("Steps")]
    public List<PQStep> steps = new List<PQStep>();

    [Header("Next Act")]
    public ActData nextAct;

    [Header("Transition")]
    [Tooltip("If true, PQM will wait for onTransitionStop before running nextAct.")]
    public bool hasTransitionBeforeNextAct = false;
    public string targetSceneName;
    public Transform targetSpawnPoint;
    public bool isSoftTransition;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(actID))
            actID = name;
    }
#endif
}

[System.Serializable]
public class PQStep
{
    public PQStepType type;

    [Tooltip("Required if type = Dialogue")]
    public DialogueData dialogueData;

    [Tooltip("Required if type = Quest")]
    public string questID;

    [Tooltip("Which ChaptSection index inside DataChapter to use for this step")]
    public int chaptSectionIndex = 0;

    [Tooltip("Seconds to wait before executing this step")]
    public float waitTransitionTime = 0f;
}

public enum PQStepType
{
    Dialogue,
    Quest,
    CardAction   // <-- new
}