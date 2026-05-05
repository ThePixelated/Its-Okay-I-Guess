using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public QuestManager m_questManager;
    
    [Header("Primary Quest UI")]
    [SerializeField] private GameObject panelPrimaryQuest;
    [SerializeField] private TextMeshProUGUI PtitleText;
    [SerializeField] private TextMeshProUGUI PdesctText;

    [Header("Side Quest UI")]
    [SerializeField] private GameObject panelSideQuest;
    [SerializeField] private TextMeshProUGUI StitleText;
    [SerializeField] private TextMeshProUGUI SdesctText;


    [Header("OLD")]
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    //[SerializeField] private GameObject questEntryPrefab2;

    [SerializeField] private TextMeshProUGUI titleQuest;
    [SerializeField] private TextMeshProUGUI descQuest;

    //[SerializeField] private Quest targetQuest;
    //[SerializeField] private List<QuestProgress> quests = new List<QuestProgress>();

    //private void Start()
    //{
    //    quests.Add(new QuestProgress(targetQuest));

    //    UpdateQuestUI(null);
    //}

    private void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        if (m_questManager.currentActiveActQuest != null)
        {
            panelPrimaryQuest.SetActive(true);
            PtitleText.text = m_questManager.currentActiveActQuest.Title;
            //PdesctText.text = m_questManager.currentActiveActQuest.SubTitle;

            string tempText = "";
            foreach (var objective in m_questManager.currentActiveActQuest.objectives)
            {
                string numeratorDesc = " ";
                if (objective.Type == ObjectiveType.Collectable)
                    numeratorDesc = $" ({objective.Current_Amount}/{objective.RequiredAmount})";

                if (objective.Current_Amount >= objective.RequiredAmount)
                    tempText += $"<s>{objective.Description}{numeratorDesc}</s>\n";
                else
                    tempText += $"<b>{objective.Description}{numeratorDesc}</b>\n";
            }
            PdesctText.text = tempText;
        }
        else
        {
            PtitleText.text = "";
            PdesctText.text = "";
            panelPrimaryQuest.SetActive(false);
        }

        if (m_questManager.currentActiveSQ != null)
        {
            panelSideQuest.SetActive(true);
            StitleText.text = m_questManager.currentActiveSQ.Title;
            //SdesctText.text = m_questManager.currentActiveSQ.SubTitle;

            string tempText = "";
            foreach (var objective in m_questManager.currentActiveSQ.objectives)
            {
                string numeratorDesc = " ";
                if (objective.Type == ObjectiveType.Collectable)
                    numeratorDesc = $" ({objective.Current_Amount}/{objective.RequiredAmount})";

                if (objective.Current_Amount >= objective.RequiredAmount)
                    tempText += $"<s>{objective.Description}{numeratorDesc}</s>\n";
                else
                    tempText += $"<b>{objective.Description}{numeratorDesc}</b>\n";
            }
            SdesctText.text = tempText;
        }
        else
        {
            StitleText.text = "";
            SdesctText.text = "";
            panelSideQuest.SetActive(false);
        }

        UIManager.Instance.m_UILoader.RefreshLayoutObj();
    }

    public void DEPRICATED_UpdateQuestUI()
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
