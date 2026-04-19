using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public QuestUI m_questUI;
    public List<Quest> mainQuestDB = new List<Quest>();
    public List<Quest> subMainQuestDB = new List<Quest>();
    public List<Quest> sideQuestDB = new List<Quest>();

    private Dictionary<string, Quest> questLookUp = new Dictionary<string, Quest>();
    public List<Quest> actActiveQuest = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();

    public Dictionary<string, QuestState> sideQuestDatabase = new Dictionary<string, QuestState>();

    private void Awake()
    {
        Instance = this;

        foreach (var questData in mainQuestDB) 
            questData.ResetValue();

        foreach (var questData in subMainQuestDB)
            questData.ResetValue();

        foreach (var questData in sideQuestDB)
            questData.ResetValue();

        // to lookup dict

        foreach (var questData in mainQuestDB)
        {
            questLookUp.Add(questData.QuestID, questData);
            //sideQuestDatabase.Add(questData.QuestID, questData.questState);
            Debug.LogWarning("Innit MQ DB.... - " + questData.QuestID);
        }
        
        foreach (var questData in subMainQuestDB)
        {
            questLookUp.Add(questData.QuestID, questData);
            //sideQuestDatabase.Add(questData.QuestID, questData.questState);
            Debug.LogWarning("Innit SubMQ DB.... - " + questData.QuestID);
        }

        foreach (var questData in sideQuestDB)
        {
            questLookUp.Add(questData.QuestID, questData);
            sideQuestDatabase.Add(questData.QuestID, questData.questState);
            Debug.LogWarning("Innit SQ DB.... - " + questData.QuestID);
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