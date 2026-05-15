using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Settings")]
    [SerializeField] private Slider sliderKesehatan;
    [SerializeField] private Slider sliderSosial;
    [SerializeField] private Slider sliderEnergi;
    [SerializeField] private Slider sliderUang;
    [SerializeField] private GameObject endPanel;
    //[SerializeField] private TextMeshProUGUI summaryTxt;


    [Header("Config Settings")]
    [SerializeField] private AIManager m_AIManager;
    public DataAction m_dataAction;
    public int lookUpIndex = 0;

    [SerializeField] private PlayerData m_playerData;
    //[SerializeField] private DataAction m_dataChapter;
    public List<NLM_Action> m_NLM_Actions;

    //private List<ChaptSection> m_ChaptSection;
    private bool endFlag = false;
    [HideInInspector]
    public bool buttonPressedFlag = false;

    private void Awake()
    {
        Instance = this;
        m_NLM_Actions = m_dataAction.ChapterSections;
        //m_ChaptSection = m_dataChapter.ChapterSections;
    }

    private void Update()
    {
        SliderUpdate();

        if (lookUpIndex > m_NLM_Actions.Count-1 && !endFlag)
        {
            endPanel.SetActive(true);
            m_AIManager.RequestSummary();
            endFlag = true;

            if (lookUpIndex >= m_NLM_Actions.Count)
                lookUpIndex = m_NLM_Actions.Count - 1;
        }
    }

    public void SliderUpdate()
    {
        sliderKesehatan.value = m_playerData.Kesehatan;
        sliderSosial.value = m_playerData.Sosial;
        sliderEnergi.value = m_playerData.Energi;
        sliderUang.value = m_playerData.Uang;
    }

    //public void EntryDataButton(string gObj_name)
    //{
    //    ActionChoiceData chosenAct = null;
    //    if (gObj_name == "KiriBtn")
    //        chosenAct = m_NLM_Actions[lookUpIndex].LeftChoice;
    //    else if (gObj_name == "KananBtn")
    //        chosenAct = m_NLM_Actions[lookUpIndex].RightChoice;

    //    chosenAct.ParseStatsImpact();
    //    ActionLogger.Instance.AddLog(m_NLM_Actions[lookUpIndex].TitleCaseScenario, chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant, chosenAct.StatVariable);
    //    UpdatePlayerData(chosenAct.StatVariable);
    //    chosenAct.DebugLog();

    //    lookUpIndex++;
    //    buttonPressedFlag = true;
    //}

    public void EntryDataButton(string gObj_name)
    {
        ActionChoiceData chosenAct = null;
        if (gObj_name == "KiriBtn")
            chosenAct = m_NLM_Actions[lookUpIndex].LeftChoice;
        else if (gObj_name == "KananBtn")
            chosenAct = m_NLM_Actions[lookUpIndex].RightChoice;

        if (chosenAct == null) return; // safety check

        chosenAct.ParseStatsImpact();
        ActionLogger.Instance.AddLog(m_NLM_Actions[lookUpIndex].TitleCaseScenario, chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant, chosenAct.StatVariable);
        UpdatePlayerData(chosenAct.StatVariable);
        chosenAct.DebugLog();

        lookUpIndex++;
        buttonPressedFlag = true;
    }

    public void LeftEntryDataButton()
    {
        ActionChoiceData chosenAct = m_NLM_Actions[lookUpIndex].LeftChoice;
        chosenAct.ParseStatsImpact();
        ActionLogger.Instance.AddLog(m_NLM_Actions[lookUpIndex].TitleCaseScenario, chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant, chosenAct.StatVariable);
        UpdatePlayerData(chosenAct.StatVariable);
        chosenAct.DebugLog();

        lookUpIndex++;
        buttonPressedFlag = true;
    }

    public void RightEntryDataButton()
    {
        ActionChoiceData chosenAct = m_NLM_Actions[lookUpIndex].RightChoice;
        chosenAct.ParseStatsImpact();
        ActionLogger.Instance.AddLog(m_NLM_Actions[lookUpIndex].TitleCaseScenario, chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant, chosenAct.StatVariable);
        UpdatePlayerData(chosenAct.StatVariable);
        chosenAct.DebugLog();

        lookUpIndex++;
        buttonPressedFlag = true;
    }

    public void UpdatePlayerData(StatVariable sentStat)
    {
        m_playerData.Kesehatan += sentStat.Health;
        m_playerData.Sosial += sentStat.Social;
        m_playerData.Energi += sentStat.Energy;
        m_playerData.Uang += sentStat.Money;
    }

    public void ResetScene()
    {
        var currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }
}
