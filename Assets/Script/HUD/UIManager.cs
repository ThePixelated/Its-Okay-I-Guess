
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Config Scripts")]
    public UILoader m_UILoader;
    public StatsHUD m_StatsHUD;

    [Header("Config GameObjects")]
    [SerializeField] private GameObject statsHUD;
    [SerializeField] private GameObject questHUD;
    [SerializeField] private GameObject cardUI;

    [Header("Config Dialog UI")]
    public GameObject stopDialouePanel;
    public GameObject dialogueNamePanel;

    public ScreenFader fadeImage;

    private void Awake()
    {
        Instance = this;
    }

    public void PQDialogueValidation(DialogueData dialogueData)
    {
        if (dialogueData.name.StartsWith("PQ_", System.StringComparison.OrdinalIgnoreCase))
        {
            SetStopDialoguePanel(false);
            Debug.Log("Stop Dialogue Button disabled!!!");
        }
        else
            SetStopDialoguePanel(true);
    }

    public void SetStopDialoguePanel(bool state)
    {
        stopDialouePanel.SetActive(state);
    }
}
