using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    //[SerializeField] private GameObject questEntryPrefab2;

    [SerializeField] private TextMeshProUGUI titleQuest;
    [SerializeField] private TextMeshProUGUI descQuest;

    //[SerializeField] private Quest targetQuest;
    public QuestManager m_questManager;
    //[SerializeField] private List<QuestProgress> quests = new List<QuestProgress>();

    //private void Start()
    //{
    //    quests.Add(new QuestProgress(targetQuest));

    //    UpdateQuestUI(null);
    //}

    public void UpdateQuestUI()
    {
        foreach (Transform item in questListContent)
        {
            if (item.gameObject.name != "")
            {
                Destroy(item.gameObject);
            }
        }

        foreach (var itemQuest in m_questManager.onHoldSQ)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TextMeshProUGUI questTitleTxt = entry.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            Debug.LogWarning(questTitleTxt.gameObject.name);
            TextMeshProUGUI questDescTxt = entry.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            Debug.LogWarning(questDescTxt.gameObject.name);

            questTitleTxt.text = itemQuest.Title;

            string tempText = "";
            foreach (var objective in itemQuest.objectives)
            {
                tempText += $"{objective.Description} ({objective.Current_Amount}/{objective.RequiredAmount})\n";
            }
            questDescTxt.text = tempText;
        }


        UIManager.Instance.m_UILoader.RefreshLayoutObj();
    }
}
