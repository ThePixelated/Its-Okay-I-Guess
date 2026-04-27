using System;
using System.Collections.Generic;
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

    //private void OnValidate()
    //{
    //    if (string.IsNullOrEmpty(QuestID))
    //    {
    //        QuestID = $"{Title}_{Guid.NewGuid().ToString()}";
    //    }
    //}

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

    public bool IsReached() 
    {
        return Current_Amount >= this.RequiredAmount;
    }
}

//public class QuestData
//{
//    public string questID; // Misal: "SQ_B_01"
//    public string questName;
//    public List<QuestObjective> objectives; // Daftar langkah misinya

//    public bool IsAllObjectivesComplete()
//    {
//        foreach (var obj in objectives)
//        {
//            if (!obj.IsReached()) return false;
//        }
//        return true;
//    }
//}

public enum ObjectiveType
{
    Collectable,
    ReachLocation,
    SideActivity,
    TalkNPC,
    Custom,
    SubMainQuest,
    MainQuest
}

//[System.Serializable]
//public class QuestProgress
//{
//    public Quest Quest;
//    public List<QuestObjective> objectives;

//    public QuestProgress(Quest quest)
//    {
//        this.Quest = quest;
//        this.objectives = new List<QuestObjective>();

//        foreach (var obj in quest.objectives)
//        {
//            objectives.Add(new QuestObjective
//            {
//                ObjectiveID = obj.ObjectiveID,
//                Description = obj.Description,
//                Type = obj.Type,
//                RequiredAmount = obj.RequiredAmount,
//                Current_Amount = 0,
//            });
//        }
//    }

//    //public bool IsCompleted => objectives.TrueForAll(x => x.IsCompleted);
//    public string QuestID => Quest.QuestID;
//}