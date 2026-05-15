using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameManager m_gameManager;
    public GameObject hoveredButton;

    [SerializeField] private TextMeshProUGUI kesehatanStat;
    [SerializeField] private TextMeshProUGUI sosialStat;
    [SerializeField] private TextMeshProUGUI energiStat;
    [SerializeField] private TextMeshProUGUI keuanganStat;

    [SerializeField] private TextMeshProUGUI titleCase;
    [SerializeField] private TextMeshProUGUI descAction;

    private void Awake()
    {
        // Jangan lupa inisialisasi singleton-nya
        if (Instance == null) Instance = this;
    }

    

    private void Start()
    {
        var currentScenario = m_gameManager.m_NLM_Actions[m_gameManager.lookUpIndex];
        titleCase.text = currentScenario.TitleCaseScenario;
        ClearText();
    }

    private void Update()
    {
        if (m_gameManager.buttonPressedFlag)
        {
            m_gameManager.buttonPressedFlag = false;

            if (m_gameManager.lookUpIndex < m_gameManager.m_NLM_Actions.Count)
            {
                var currentScenario = m_gameManager.m_NLM_Actions[m_gameManager.lookUpIndex];
                titleCase.text = currentScenario.TitleCaseScenario;

                UpdateText();
            }
        }
    }

    

    public void UpdateText()
    {
        if (hoveredButton == null) return;

        ActionChoiceData actionChoiceData = null;
        var currentScenario = m_gameManager.m_NLM_Actions[m_gameManager.lookUpIndex];

        if (hoveredButton.name == "KiriBtn")
            actionChoiceData = currentScenario.LeftChoice;
        else if (hoveredButton.name == "KananBtn")
            actionChoiceData = currentScenario.RightChoice;

        if (actionChoiceData != null)
        {
            kesehatanStat.text = actionChoiceData.StatVariable.Health.ToString();
            sosialStat.text = actionChoiceData.StatVariable.Social.ToString();
            energiStat.text = actionChoiceData.StatVariable.Energy.ToString();
            keuanganStat.text = actionChoiceData.StatVariable.Money.ToString();

            descAction.text = actionChoiceData.Action;
        }
    }

    public void ClearText()
    {
        kesehatanStat.text = "";
        sosialStat.text = "";
        energiStat.text = "";
        keuanganStat.text = "";

        descAction.text = "";
    }
}