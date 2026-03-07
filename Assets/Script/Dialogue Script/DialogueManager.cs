using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameModeManager m_gameModeManager;
    [SerializeField] private DialogueUI m_dialogueUI;
    [SerializeField] private DialogueData m_dialogueData;
    [SerializeField] private DialogueNode currentNode;

    private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
    
    public DialogueData DialogueData { get { return m_dialogueData; } set { m_dialogueData = value; } }

    private void Awake()
    {
        m_dialogueUI = gameObject.GetComponent<DialogueUI>();
    }

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_Dialogs += GoToNode;
    }

    public void StartDialogue(List<DialogueNode> dialogueData)
    {
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

    public void GoToNode() // BUTTON DOANG
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
        //ReadableSetCheck();
    }

    public bool QuestChecker()
    {
        const string questKey = "quest_";

        if (currentNode.NodeID.Contains(questKey))
            return true;
        
        return false;
    }

    public void QuestInnitialize()
    {
        string questID = currentNode.NodeID.Substring(currentNode.NodeID.IndexOf("#")+1);
        //QuestManager.Instance..Add(new Quest("Find the Key", "Find the key to unlock the door."));
    }

    private void ReadableSetCheck()
    {
        if (m_dialogueData.SetRead)
        {
            m_dialogueData.IsDialogueRead = m_dialogueData.SetRead;
        }
    }
}

//-gimana caranya biar yang udah dibaca masuk ke list read
//- terus nanti re-init, semuanya kereset.

//- dia ngebaca semua dialogueNode
//- button ga kereset