using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public QuestUI m_questUI;
    public List<Quest> sideQuestDB = new List<Quest>();
    //public List<QuestProgress> currentQuest = new List<QuestProgress>();

    private Dictionary<string, Quest> questLookUp = new Dictionary<string, Quest>();
    public List<Quest> activeQuests = new List<Quest>();

    //[SerializeField] private QuestUI m_questUI;

    /// <summary>
    /// ada list main quest
    /// list side quest
    /// </summary>
    /// 

    public Dictionary<string, QuestState> sideQuestDatabase = new Dictionary<string, QuestState>();

    //private void OnValidate()
    //{
    //    questLookUp = new Dictionary<string, Quest>();
    //    foreach (var questData in questDB)
    //    {
    //        questLookUp.Add(questData.QuestID, questData);
    //        sideQuestDatabase.Add(questData.QuestID, questData.questState);
    //        Debug.LogWarning("Innit node.... - " + questData.QuestID);
    //    }
    //}

    private void Awake()
    {
        Instance = this;

        //m_QuestData.InnitQuestData();

        //questLookUp = new Dictionary<string, Quest>();
        //foreach (var quest in questDB)
        //{
        //    questLookUp.Add(quest.QuestID, quest);
        //    Debug.LogWarning("Innit quest.... - " + quest.QuestID);
        //}

        foreach (var questData in sideQuestDB)
        {
            questData.ResetValue();
        }

        //questLookUp = new Dictionary<string, Quest>();
        foreach (var questData in sideQuestDB)
        {
            questLookUp.Add(questData.QuestID, questData);
            sideQuestDatabase.Add(questData.QuestID, questData.questState);
            Debug.LogWarning("Innit node.... - " + questData.QuestID);
        }
    }


    public void OnNPCTalked(string npcID)
    {
        // Cek semua quest yang lagi aktif
        foreach (Quest quest in activeQuests)
        {
            foreach (QuestObjective obj in quest.objectives)
            {
                // Jika ada langkah quest yang menyuruh bicara ke NPC ini
                if (obj.Type == ObjectiveType.TalkNPC && npcID == obj.ObjectiveID)
                {
                    Debug.Log($"Currently talk: {npcID} - target objective: {obj.ObjectiveID}");

                    obj.Current_Amount = 1; // Tandai sudah selesai
                    Debug.Log($"Objective {obj.ObjectiveID} di quest {quest.QuestID} selesai!");

                    // Cek apakah semua langkah di quest ini sudah beres
                    if (quest.IsAllObjectivesComplete())
                    {
                        Debug.Log($"Quest {quest.QuestID} siap diselesaikan!");
                        sideQuestDatabase[quest.QuestID] = quest.questState;
                    }
                }
            }
        }
    }

    public void SuccessQuest(Quest quest)
    {
        //gapapap kalo ga pake quesy manager soS
    }

    public void ChangeToCardGame()
    {
        GameModeManager.Instance.Switch(GameModeManager.Instance.CardMode);
    }

    public void AddQuest(string questID)
    {
        activeQuests.Add(questLookUp[questID]);
        questLookUp[questID].questState = QuestState.Active;
        sideQuestDatabase[questID] = QuestState.Active;
        m_questUI.UpdateQuestUI();
    }

    public Dictionary<int, QuestState> mainQuestDatabase = new Dictionary<int, QuestState>();
    public QuestState GetMainQuestState(int sqID)
    {
        if (mainQuestDatabase.ContainsKey(sqID))
        {
            Debug.Log($"QUEST State RETRIVE: {mainQuestDatabase[sqID]}");
            return mainQuestDatabase[sqID];
        }
        return QuestState.Unassigned; // Default jika belum terdaftar
    }

    public QuestState GetSideQuestState(string sqID)
    {
        if (sideQuestDatabase.ContainsKey(sqID))
        {
            Debug.Log($"QUEST State RETRIVE: {sideQuestDatabase[sqID]}");
            return sideQuestDatabase[sqID];
        }
        return QuestState.Unassigned; // Default jika belum terdaftar
    }

    //public void GetActiveQuestsForTarget(string objectID)
    //{
    //    //foreach (var curr_activeQuest in activeQuests)
    //    //{
    //    //    curr_activeQuest
    //    //}
    //}
}

public enum QuestState
{
    Unassigned, // Belum diambil / Belum mulai
    Active,     // Sedang berjalan / Belum selesai
    Success     // Sudah selesai
}