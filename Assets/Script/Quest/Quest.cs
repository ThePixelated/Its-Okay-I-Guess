using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string QuestID;
    public string Title;
    [TextArea(3, 10)]
    public string Description;
    public List<QuestObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(QuestID))
        {
            QuestID = $"{Title}_{Guid.NewGuid().ToString()}";
        }
    }
}

public enum ObjectiveType
{
    Collectable,
    ReachLocation,
    SideActivity,
    TalkNPC,
    Custom,
}

[System.Serializable]
public class QuestObjective
{

    public string ObjectiveID;
    [TextArea(3, 10)]
    public string Description;
    public ObjectiveType Type;
    public int RequiredAmount;
    public int Current_Amount;

    public bool IsCompleted => this.Current_Amount >= this.RequiredAmount;
}

[System.Serializable]
public class QuestProgress
{
    public Quest Quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.Quest = quest;
        this.objectives = new List<QuestObjective>();

        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                ObjectiveID = obj.ObjectiveID,
                Description = obj.Description,
                Type = obj.Type,
                RequiredAmount = obj.RequiredAmount,
                Current_Amount = 0,
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(x => x.IsCompleted);
    public string QuestID => Quest.QuestID;
}