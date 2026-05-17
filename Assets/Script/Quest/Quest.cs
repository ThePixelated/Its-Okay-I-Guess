using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string QuestID;
    public string Title;
    public string SubTitle;
    [TextArea(3, 10)]
    public string Description;
    public List<QuestObjective> objectives;
    public QuestState questState;

    private void OnValidate()
    {
        foreach (var item in objectives)
            item.InnitObjectiveIDs(); // ini harus masuk ke bagian resetvalue
    }

    public bool IsAllObjectivesComplete()
    {
        foreach (var obj in objectives)
        {
            if (!obj.IsReached()) return false;
        }

        questState = QuestState.Success;
        return true;
    }

    public void ResetValue()
    {
        foreach (var obj in objectives)
            obj.Current_Amount = 0;

        questState = QuestState.Unassigned;
    }
}

[System.Serializable]
public class QuestObjective
{
    public string ObjectiveID;
    [TextArea(3, 10)]
    public string Description;
    //public string TargetID; // Jika TalkTo, isi dengan ID NPC (misal: "NPC_A")
    public ObjectiveType Type;
    public int RequiredAmount;
    public int Current_Amount;

    public List<string> ObjectivesIDs = new List<string>();

    public bool IsReached() 
    {
        return Current_Amount >= this.RequiredAmount;
    }

    public void InnitObjectiveIDs()
    {
        ObjectivesIDs = ObjectiveID.Split(';')
                           .Select(id => id.Trim())
                           .Where(id => !string.IsNullOrEmpty(id))
                           .ToList();
    }
}

public enum ObjectiveType
{
    Single,
    Collectable,
    Use,
    TalkNPC,
    Interactable,
    ReachLocation,
    SideActivity,
    EndLocation,
    Custom,
    Null
}