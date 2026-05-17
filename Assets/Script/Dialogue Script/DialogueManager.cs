using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameModeManager m_gameModeManager;
    [SerializeField] private DialogueUI m_dialogueUI;
    [SerializeField] private DialogueData m_dialogueData;
    [SerializeField] private DialogueNode currentNode;

    private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
    
    public DialogueData DialogueData { get { return m_dialogueData; } set { m_dialogueData = value; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_Dialogs += GoToNode;
    }

    public void StartDialogue(List<DialogueNode> dialogueData)
    {
        m_dialogueUI.PQDialogueValidation(DialogueData);

        nodeLookup = new Dictionary<string, DialogueNode>();
        foreach (var node in dialogueData)
        {
            nodeLookup.Add(node.NodeID, node);
            Debug.LogWarning("Innit node.... - " + node.NodeID);
        }

        Debug.Log("Done __innit node: " + nodeLookup);

        currentNode = nodeLookup.FirstOrDefault().Value;

        Debug.LogWarning("Is currentNode Nan: " + (currentNode == null) + " - DataNode: " + (nodeLookup == null));

        m_dialogueUI.StartRender(currentNode);
    }

    public void GoToNodeBtn(string nodeID) // BUTTON DOANG
    {
        Debug.LogWarning("Go to node BUTTON");
        if (nodeID == null || nodeID.Length == 0)
        {
            m_dialogueUI.CloseRender();
            return;
        }

        currentNode = nodeLookup[nodeID];
        m_dialogueUI.Render(currentNode);
    }

    public void GoToNode()
    {
        if (currentNode.Choices.Count <= 0)
        {
            Debug.LogWarning("Go to node NON-BUTTON");
            if (currentNode.NextNodeID == null || currentNode.NextNodeID.Length == 0)
            {
                m_dialogueUI.CloseRender();
                return;
            }

            currentNode = nodeLookup[currentNode.NextNodeID];
            m_dialogueUI.Render(currentNode);
        }
    }

    public void DialogueStopped()
    {
        m_gameModeManager.DialogueStopped();
        m_dialogueUI.SetStopDialoguePanel(true);
        //ReadableSetCheck();
    }
}

[System.Serializable]
public class QuestDialoguePack
{
    // Setiap fase berisi list agar bisa diacak variasinya
    public List<DialogueData> beforeQuest;
    public List<DialogueData> duringQuest;
    public List<DialogueData> afterQuest;
}

public static class DialogueRetriever
{
    public static List<DialogueData> DetermineDialogue(ObjectInteraction npc)
    {
        // Ambil data global dari SceneData atau QuestManager Anda
        int globalMQIndex = 0; //SceneData.Instance.CurrentMainQuestIndex

        // ----------------------------------------------------
        // PRIORITAS 1: EVALUASI MAIN QUEST
        // ----------------------------------------------------
        if (npc.hasMainQuest)
        {
            // Jika saat ini memang giliran Main Quest milik NPC ini
            if (globalMQIndex == npc.mainQuestID)
            {
                // Anda perlu mengecek status MQ ini di QuestManager Anda.
                // Anggap saja kita punya fungsi pengecekan statusnya.
                QuestState mqState = QuestManager.Instance.GetQuestState(npc.mainQuestID.ToString());

                switch (mqState)
                {
                    case QuestState.Unassigned:
                        return npc.mainQuestDialogues.beforeQuest;
                    case QuestState.Active:
                        return npc.mainQuestDialogues.duringQuest;
                    case QuestState.Success:
                        return npc.mainQuestDialogues.afterQuest;
                }
            }
        }

        foreach (var external in npc.externalQuestDialogues)
        {
            // Cek ke QuestManager apakah ID Quest ini sedang ON GOING
            if (QuestManager.Instance.GetQuestState(external.questID) == QuestState.Active)
            {
                return external.dialogues; // Balikin dialog titipan ini
            }
        }

        // ----------------------------------------------------
        // PRIORITAS 2: EVALUASI SIDE QUEST
        // ----------------------------------------------------
        // Masuk ke sini jika NPC tidak punya MQ, ATAU MQ-nya tidak aktif (indeks global < atau > dari ID NPC)
        if (npc.hasSideQuest && !string.IsNullOrEmpty(npc.sideQuestID))
        {
            QuestState sqState = QuestManager.Instance.GetQuestState(npc.sideQuestID);
            Debug.Log($"NPC ID: {npc.ObjectID} - Quest State: {sqState}");

            switch (sqState)
            {
                case QuestState.Unassigned:
                    return npc.sideQuestDialogues.beforeQuest;
                case QuestState.Active:
                    return npc.sideQuestDialogues.duringQuest;
                case QuestState.Success:

                    npc.SQAfterQuestCountdown--;
                    if (npc.SQAfterQuestCountdown > 0)
                        return npc.sideQuestDialogues.afterQuest;

                    npc.hasSideQuest = false;
                    break;
            }
        }

        // ----------------------------------------------------
        // PRIORITAS 3: FALLBACK (TIDAK ADA QUEST / SEMUA SELESAI)
        // ----------------------------------------------------
        if (npc.normalDialogues == null || npc.normalDialogues.Count <= 0)
            return null;

        return npc.normalDialogues;
    }
}
//-gimana caranya biar yang udah dibaca masuk ke list read
//- terus nanti re-init, semuanya kereset.

//- dia ngebaca semua dialogueNode
//- button ga kereset