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
    [Tooltip("Coping mechanism tag untuk firstChoice — dipakai NLM logging & fitur ensiklopedia")]
    public CopingTag firstChoiceTag;
    [Tooltip("Entry ensiklopedia yang ke-unlock kalau player pilih firstChoice (kiri)")]
    public EncyclopediaEntry firstChoiceEncyclopedia;

    [TextArea(3, 10)] public string secondChoice;  // shown when card dragged RIGHT
    [Tooltip("Coping mechanism tag untuk secondChoice — dipakai NLM logging & fitur ensiklopedia")]
    public CopingTag secondChoiceTag;
    [Tooltip("Entry ensiklopedia yang ke-unlock kalau player pilih secondChoice (kanan)")]
    public EncyclopediaEntry secondChoiceEncyclopedia;

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

/// <summary>
/// Coping mechanism classification untuk tiap choice.
/// Dipakai oleh NLM (Narrative Logic Manager / AI summary) dan fitur ensiklopedia.
/// </summary>
public enum CopingTag
{
    Adaptive,
    Maladaptive
}

// Keep for backward compatibility — no longer used in new flow
public enum SubChaptType { Null, ActionChapt, Consequences }
public enum BranchState { Stable, Trigger, Continues, Closed }