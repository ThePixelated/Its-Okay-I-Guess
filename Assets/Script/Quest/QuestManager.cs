using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestUI m_questUI;
    public List<Quest> questDB = new List<Quest>();
    public List<QuestProgress> currentQuest = new List<QuestProgress>();

    private Dictionary<string, Quest> questLookUp = new Dictionary<string, Quest>();



    //[SerializeField] 

    //[SerializeField] private QuestUI m_questUI;

    /// <summary>
    /// ada list main quest
    /// list side quest
    /// </summary>

    private void Awake()
    {
        Instance = this;

        //m_QuestData.InnitQuestData();

        questLookUp = new Dictionary<string, Quest>();
        foreach (var quest in questDB)
        {
            questLookUp.Add(quest.QuestID, quest);
            Debug.LogWarning("Innit quest.... - " + quest.QuestID);
        }
    }

    public void AddQuest(string questID)
    {
        currentQuest.Add(new QuestProgress(questLookUp[questID]));

        m_questUI.UpdateQuestUI();
    }
}
