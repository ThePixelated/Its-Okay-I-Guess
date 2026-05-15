using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DataAction", menuName = "Scriptable Objects/DataAction")]
public class DataAction : ScriptableObject
{
    public List<NLM_Action> ChapterSections = new List<NLM_Action>();

    private void OnValidate()
    {
        foreach (var action in ChapterSections)
        {
            if (action != null && action.LeftChoice != null)
            {
                action.LeftChoice.ParseStatsImpact();
            }

            if (action != null && action.RightChoice != null)
            {
                action.RightChoice.ParseStatsImpact();
            }
        }

        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
}

[System.Serializable]
public class NLM_Action
{
    [TextArea(3, 10)]
    public string TitleCaseScenario;
    public ActionChoiceData LeftChoice;
    public ActionChoiceData RightChoice;
}

[System.Serializable]
public class ActionChoiceData
{
    public string Source;
    [TextArea(3, 10)]
    public string Action;
    public CopingTag Coping_Tag;
    public string StatImpact;
    public bool IsSignificant;
    public StatVariable StatVariable;

    public void DebugLog()
    {
        Debug.LogWarning($"    Source: {this.Source}\r\n    Action: {this.Action}\r\n    Coping Tag: {this.Coping_Tag}\r\n    StatImpact: {this.StatImpact}\r\n    IsSignificant: {this.IsSignificant}");
    }

    public void ParseStatsImpact()
    {
        StatImpact = ($"Kesehatan {StatVariable.Health}; Sosial {StatVariable.Social}; Energi {StatVariable.Energy}; Keuangan {StatVariable.Money};");
    }
}

public enum CopingTag
{
    Adaptive,
    Maladaptive,
    Avoidance
}

[System.Serializable]
public class StatVariable
{
    public int Health;
    public int Social;
    public int Energy;
    public int Money;
}