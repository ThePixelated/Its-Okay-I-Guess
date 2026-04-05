using UnityEngine;
using System.Collections.Generic;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private List<DialogueData> npcDialogueData;
    //[SerializeField] private List<List<DialogueData>> DialogueDataMainQuest;
    [SerializeField] private DialogueManager m_dialogueManager;
    //[SerializeField] private List<DialogueData> readedDialogueData;

    [SerializeField] private int mainQuestID = -1;

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_E += StartingDialogue;
    }

    public void DialogueValidation()
    {
        
    }

    public void StartingDialogue(string objectName)
    {
        if (gameObject.name == objectName && npcDialogueData != null)
        {
            //foreach (var item in npcDialogueData)
            //{
            //    if (item.IsDialogueRead)
            //    {
            //        readedDialogueData.Add(item);
            //        npcDialogueData.Remove(item);
            //    }
            //}

            // random pick
            DialogueData pickedData = npcDialogueData[0];
            //m_dialogueManager.DialogueData = pickedData;
            //if (pickedData != null)
            //    Debug.Log(m_dialogueManager.DialogueData.name);
            //else
            //    Debug.LogWarning("picked NULL!");

            m_dialogueManager.StartDialogue(pickedData.DialogueNodes); // core logic
        }
        else
            Debug.LogWarning("DialogueData NULL!");
    }

    //public DialogueData ReturnAndValidateDialogueData()
    //{
    //    DialogueData pickedDialogue = null;

    //    // Dialogue validation for main quest
    //    if (!(mainQuestID <= -1))
    //    {
    //        if (SceneData.instance.CurrentMainQuestIndex > mainQuestID) // After Quest
    //        {
    //            pickedDialogue = npcDialogueData[0][Random.Range(0, npcDialogueData[0].Count)];
    //        }

    //        else if (SceneData.instance.CurrentMainQuestIndex == mainQuestID) // Saat Quest
    //        {
    //            pickedDialogue = npcDialogueData[1][Random.Range(0, npcDialogueData[1].Count)];
    //        }

    //        else if (SceneData.instance.CurrentMainQuestIndex < mainQuestID && mainQuestID > 0) // Before Quest
    //        {
    //            pickedDialogue = npcDialogueData[2][Random.Range(0, npcDialogueData[2].Count)];
    //        }

    //        return pickedDialogue;
    //    }

    //    // Dialogue validation for side quest
    //    if (true)
    //    {

    //    }

    //    return pickedDialogue;
    //}
}
