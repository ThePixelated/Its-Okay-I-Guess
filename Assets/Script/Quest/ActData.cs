using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents one Act in the Primary Quest flow.
/// An Act is an ordered chain of steps (Dialogue and/or Quest).
/// Acts are linked via nextAct to form the full story chain.
/// </summary>
[CreateAssetMenu(fileName = "ActData", menuName = "Scriptable Objects/ActData")]
public class ActData : ScriptableObject
{
    [Header("Act Identity")]
    public string actID; // e.g. "CH1_Act1" — for debugging & logging

    [Header("Steps")]
    public List<PQStep> steps = new List<PQStep>();

    [Header("Next Act")]
    public ActData nextAct; // null = end of story / chapter

    [Header("Transition")]
    [Tooltip("If true, PQM will wait for onTransitionStop before running nextAct. Logic TBD.")]
    public bool hasTransitionBeforeNextAct = false;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(actID))
        {
            actID = name;
        }
    }
#endif

}

[System.Serializable]
public class PQStep
{
    public PQStepType type;

    [Tooltip("Required if type = Dialogue")]
    public DialogueData dialogueData;

    [Tooltip("Required if type = Quest. Must match QuestID naming convention: QuestType_CurrChapter_Location_Time_Day_ContextName")]
    public string questID;

    [Tooltip("Seconds to wait before executing this step")]
    public float waitTransitionTime = 0f;
}

public enum PQStepType
{
    Dialogue,
    Quest
}
