using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private DialogueData npcDialogueData;
    [SerializeField] private DialogueManager m_dialogueManager;

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_E += StartingDialogue;
    }

    public void StartingDialogue()
    {
        m_dialogueManager.StartDialogue(npcDialogueData.DialogueNodes);
    }
}
