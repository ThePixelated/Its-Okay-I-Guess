using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataChapter", menuName = "Scriptable Objects/DataChapter")]
public class DataChapter : ScriptableObject
{
    public List<ChaptSection> ChapterSections = new List<ChaptSection>();
}

[System.Serializable]
public class ChaptSection
{
    public string Title;
    public int ID = 0;
    public ActionChapt actionSection = new ActionChapt();

    /// <summary>
    /// Index 0 = consequence if player chose LEFT (firstChoice).
    /// Index 1 = consequence if player chose RIGHT (secondChoice).
    /// </summary>
    public List<Consequences> consSection = new List<Consequences>(2);
}

[System.Serializable]
public class ActionChapt
{
    [TextArea(3, 10)] public string statement;
    [TextArea(3, 10)] public string firstChoice;   // shown when card dragged LEFT
    [TextArea(3, 10)] public string secondChoice;  // shown when card dragged RIGHT
    public StatVariable statEffect;
}

[System.Serializable]
public class Consequences
{
    public string Title;
    [TextArea(3, 10)] public string firstChoice;
    [TextArea(3, 10)] public string secondChoice;
}

[System.Serializable]
public class StatVariable
{
    public float Money;
    public float Health;
    public float Energy;
    public float Social;
}

// Keep enums for backward compatibility — SubChaptType no longer used in new flow
// but may still exist in old SO assets in the project
public enum SubChaptType { Null, ActionChapt, Consequences }
public enum BranchState { Stable, Trigger, Continues, Closed }