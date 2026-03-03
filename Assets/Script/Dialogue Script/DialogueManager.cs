using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameModeManager m_gameModeManager;
    [SerializeField] private DialogueUI m_dialogueUI;
    [SerializeField] private DialogueNode currentNode;

    private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

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

        currentNode = nodeLookup["start"];

        Debug.LogWarning("Is currentNode Nan: " + (currentNode == null) + " - DataNode: " + (nodeLookup == null));

        m_dialogueUI.StartRender(currentNode);
    }

    public void GoToNodeBtn(string nodeID) // BUTTON DOANG
    {
        Debug.LogWarning("Go to node BUTTON");
        if (nodeID == null || nodeID.Length == 0)
        {
            m_dialogueUI.CloseRender();
            //currentNode = nodeLookup["start"];
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
                //currentNode = nodeLookup["start"];
                return;
            }

            currentNode = nodeLookup[currentNode.NextNodeID];
            m_dialogueUI.Render(currentNode);
        }
    }

    public void DialogueStopped()
    {
        m_gameModeManager.DialogueStopped();
    }
}
