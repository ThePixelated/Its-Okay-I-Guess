using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataAction", menuName = "Scriptable Objects/DataAction")]
public class DataAction : ScriptableObject
{
    public List<NLM_Action> NLM_Action = new List<NLM_Action>();
}

[System.Serializable]
public class NLM_Action
{
    public string CaseScenario;
    public ActionChoiceData LeftChoice;
    public ActionChoiceData RightChoice;
}

[System.Serializable]
public class ActionChoiceData
{
    public string Source;
    public string Action;
    public CopingTag Coping_Tag;
    public string StatImpact;
    public bool IsSignificant;

    public void DebugLog()
    {
        Debug.LogWarning($"    Source: {this.Source}\r\n    Action: {this.Action}\r\n    Coping Tag: {this.Coping_Tag}\r\n    StatImpact: {this.StatImpact}\r\n    IsSignificant: {this.IsSignificant}");
    }
}

public enum CopingTag
{
    Adaptive,
    Maladaptive,
    Avoidance
}