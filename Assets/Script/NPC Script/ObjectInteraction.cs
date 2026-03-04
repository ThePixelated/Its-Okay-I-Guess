using UnityEngine;
using System.Collections.Generic;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private List<DialogueData> npcDialogueData;
    [SerializeField] private DialogueManager m_dialogueManager;
    [SerializeField] private List<DialogueData> readedDialogueData;

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_E += StartingDialogue;
    }

    public void StartingDialogue(string objectName)
    {
        if (gameObject.name == objectName && npcDialogueData != null)
        {
            foreach (var item in npcDialogueData)
            {
                if (item.IsDialogueRead)
                {
                    readedDialogueData.Add(item);
                    npcDialogueData.Remove(item);
                }
            }

            // random pick
            DialogueData pickedData = npcDialogueData[Random.Range(0, npcDialogueData.Count)];
            m_dialogueManager.DialogueData = pickedData;
            if (pickedData != null)
                Debug.Log(m_dialogueManager.DialogueData.name);
            else
                Debug.LogWarning("picked NULL!");
            m_dialogueManager.StartDialogue(pickedData.DialogueNodes);
        }
        else
            Debug.LogWarning("DialogueData NULL!");
    }
}
