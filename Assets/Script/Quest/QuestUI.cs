using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    //[SerializeField] private GameObject questEntryPrefab2;

    [SerializeField] private Quest targetQuest;
    [SerializeField] private List<QuestProgress> quests = new List<QuestProgress>();

    //private void Start()
    //{
    //    quests.Add(new QuestProgress(targetQuest));

    //    UpdateQuestUI(null);
    //}

    public void AddQuest(Quest questData)
    {
        quests.Add(new QuestProgress(questData));

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        foreach (Transform item in questListContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var itemQuest in quests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TextMeshProUGUI questTitleTxt = entry.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            Debug.LogWarning(questTitleTxt.gameObject.name);
            TextMeshProUGUI questDescTxt = entry.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            Debug.LogWarning(questDescTxt.gameObject.name);

            questTitleTxt.text = itemQuest.Quest.Title;

            string tempText = "";
            foreach (var objective in itemQuest.objectives)
            {
                tempText += $"{objective.Description} ({objective.Current_Amount}/{objective.RequiredAmount})\n";
            }
            questDescTxt.text = tempText;
        }


        UILoader.Instance.RefreshLayoutObj();
    }
}
