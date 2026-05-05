using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PrimaryQuestManager : MonoBehaviour
{
    public QuestManager m_questManager;
    public QuestUI m_questUI;

    [Header("Setup Dialogue")]
    [SerializeField] private DialogueData currentDialogueData;
    public List<DialogueData> dialogueData = new List<DialogueData>();

    private Dictionary<string, DialogueData> nodeDialogueData = new Dictionary<string, DialogueData>();
    private Coroutine _currentCoroutine;

    private DialogueManager m_dialogueManager;
    private GameModeManager m_gameModeManager;

    async void Start()
    {
        GameModeManager.Instance.onDialogueStop += ConfigPrimaryQuest;

        m_dialogueManager = DialogueManager.Instance;
        m_gameModeManager = GameModeManager.Instance;

        // NON PERMANENT !!!!
        m_gameModeManager.Switch(m_gameModeManager.DialogueMode);

        foreach (var DialogueData in dialogueData)
            nodeDialogueData.Add(DialogueData.name, DialogueData);

        await Task.Delay(12000);

        currentDialogueData = dialogueData[0];
        m_dialogueManager.DialogueData = currentDialogueData;
        m_dialogueManager.StartDialogue(currentDialogueData.DialogueNodes);
        //m_gameModeManager.Switch(m_gameModeManager.DialogueMode);
    }

    public void ConfigPQAfterQuest()
    {
        PMQuestInfo curPMQ_Info = currentDialogueData.PMQuestInfo;
        DialogueData newDialogueData = currentDialogueData;

        _currentCoroutine = StartCoroutine(StartDialogue(curPMQ_Info.waitTransitionTime, newDialogueData));
    }

    public void ConfigPrimaryQuest(GameModeManager GMM)
    {
        if (m_dialogueManager.DialogueData.IsIgnoredbyPQ)
            return;

        //_currentCoroutine = StartCoroutine(StartPrimaryChain());
        Debug.Log("Config Primary Quest");
        // bisa dijadiin kalo targetdialoguedata = null, berarti udah selesai. (g jg sih..)

        PMQuestInfo curPMQ_Info = currentDialogueData.PMQuestInfo;
        DialogueData newDialogueData = currentDialogueData;
        
        if (!string.IsNullOrEmpty(curPMQ_Info.targetDialogueData))
            newDialogueData = nodeDialogueData[currentDialogueData.PMQuestInfo.targetDialogueData];

        if (curPMQ_Info.isTriggerDialogue)
            _currentCoroutine = StartCoroutine(StartDialogue(curPMQ_Info.waitTransitionTime, newDialogueData));

        if (curPMQ_Info.isTriggerQuest)
        {
            Debug.Log($"Target: {curPMQ_Info.targetQuest}");
            m_questManager.AddPrimaryQuest(curPMQ_Info.targetQuest);
        }

        currentDialogueData = newDialogueData;
    }

    public IEnumerator StartDialogue(float timer, DialogueData targetData)
    {
        Debug.Log("Starting dialogue.. waiting");
        yield return new WaitForSeconds(timer);
        Debug.Log("DIALOGUE INNITATE...");
        m_gameModeManager.Switch(m_gameModeManager.DialogueMode);
        m_dialogueManager.DialogueData = currentDialogueData;
        m_dialogueManager.StartDialogue(targetData.DialogueNodes);

        StopCoroutine(_currentCoroutine);
    }
}
