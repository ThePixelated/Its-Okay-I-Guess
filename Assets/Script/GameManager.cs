using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Slider sliderKesehatan;
    [SerializeField] private Slider sliderSosial;
    [SerializeField] private Slider sliderEnergi;
    [SerializeField] private Slider sliderUang;

    [SerializeField] private DataAction m_dataAction;
    [SerializeField] private int lookUpIndex = 0;

    [SerializeField] PlayerData m_playerData;
    private List<NLM_Action> m_NLM_Actions;

    private void Start()
    {
        m_NLM_Actions = m_dataAction.NLM_Action;
    }

    private void Update()
    {
        SliderUpdate();
    }

    public void SliderUpdate()
    {
        sliderKesehatan.value = m_playerData.Kesehatan;
        sliderSosial.value = m_playerData.Sosial;
        sliderEnergi.value = m_playerData.Energi;
        sliderUang.value = m_playerData.Uang;
    }

    public void LeftEntryDataButton()
    {
        var chosenAct = m_NLM_Actions[lookUpIndex].LeftChoice;
        ActionLogger.Instance.AddLog(chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant);
        chosenAct.DebugLog();

        lookUpIndex++;
    }

    public void RightEntryDataButton()
    {
        var chosenAct = m_NLM_Actions[lookUpIndex].RightChoice;
        ActionLogger.Instance.AddLog(chosenAct.Source, chosenAct.Action, chosenAct.Coping_Tag, chosenAct.StatImpact, chosenAct.IsSignificant);
        chosenAct.DebugLog();

        lookUpIndex++;
    }
}
