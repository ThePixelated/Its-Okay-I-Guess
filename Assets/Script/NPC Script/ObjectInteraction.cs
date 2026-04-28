using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshPro namaTxt;
    [SerializeField] private string objectID;
    public string ObjectID { get { return objectID; } }

    // tambahin isInteractable ke si objek ini (trigger dari quest. dialogue) 

    // tambahin first time interaction isFirstTIme; List<DialogueData> ... (trigger dari GetCurrentDialogueData())
    // tambahin unlocked object isLocked; List<DialogueData> ... (trigger dari quest. dialogue)

    [Header("Main Quest Config")]
    public bool hasMainQuest;
    public int mainQuestID;
    public int MQAfterQuestCountdown;
    public QuestDialoguePack mainQuestDialogues;

    [Header("Side Quest Config")]
    public bool hasSideQuest;
    public string sideQuestID;
    public int SQAfterQuestCountdown;
    public QuestDialoguePack sideQuestDialogues;

    [Header("Normal Config")]
    public List<DialogueData> normalDialogues;

    [Header("External Quest DIalogue Config")]
    // List penampung dialog titipan dari Quest luar
    public List<ExternalQuestDialogue> externalQuestDialogues;

    // Fungsi ini yang akan dipanggil oleh DialogueManager saat Player menekan tombol interaksi
    private void OnValidate()
    {
        if (namaTxt != null && (namaTxt.text == "" || namaTxt.text != null))
            namaTxt.text = gameObject.name;
        
        if (objectID != null)
            objectID = gameObject.name;
    }

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_E += Interact;
    }
    
    [Serializable] 
    public struct ExternalQuestDialogue
    {
        public string questID; // ID Quest dari luar (Misal: "SQ_B_01")
        public List<DialogueData> dialogues; // Dialog khusus untuk quest tersebut
    }


    /// <summary>
    /// berarti ExternalQuestDialogue masih disimpen di dalem objectnya masing?
    /// kalo sebuah quest udah dijalanin, opsi tersebut ilangin
    /// gua masih ga tau integrasininnya gimana
    /// seakan akan semua terpisah terus disuruh jadi satuAAA
    /// </summary>
    /// <returns></returns>

    public List<DialogueData> GetCurrentDialogueData()
    {
        // Logika alur prioritas diletakkan di sini
        return DialogueRetriever.DetermineDialogue(this);
    }

    public void Interact(string objectName)
    {
        
        if (gameObject.name == objectName)
        {
            //QuestManager.Instance.OnNPCTalked(objectID);
            //foreach (var item in npcDialogueData)
            //{
            //    if (item.IsDialogueRead)
            //    {
            //        readedDialogueData.Add(item);
            //        npcDialogueData.Remove(item);
            //    }
            //}

            // random pick
            //DialogueData pickedData = npcDialogueData[0];
            //m_dialogueManager.DialogueData = pickedData;
            //if (pickedData != null)
            //    Debug.Log(m_dialogueManager.DialogueData.name);
            //else
            //    Debug.LogWarning("picked NULL!");

            //m_dialogueManager.StartDialogue(pickedData.DialogueNodes); // core logic

            foreach (var external in externalQuestDialogues)
            {
                // Cek ke QuestManager apakah ID Quest ini sedang ON GOING
                if (QuestManager.Instance.GetQuestState(external.questID) == QuestState.Active)
                {
                    Debug.LogWarning(QuestManager.Instance.GetQuestState(external.questID));
                    break;
                }
            }

            List<DialogueData> chosenList = GetCurrentDialogueData();

            int randomIndex = UnityEngine.Random.Range(0, chosenList.Count);
            DialogueData selectedDialogue = chosenList[randomIndex];

            foreach (var external in externalQuestDialogues)
            {
                // Cek ke QuestManager apakah ID Quest ini sedang ON GOING
                if (QuestManager.Instance.GetQuestState(external.questID) == QuestState.Active)
                {
                    Debug.Log(QuestManager.Instance.GetQuestState(external.questID));
                    break;
                }
            }

            // Kirim selectedDialogue ini ke UI sistem dialog Anda yang sudah matang
            DialogueManager.Instance.StartDialogue(selectedDialogue.DialogueNodes);

            foreach (var external in externalQuestDialogues)
            {
                // Cek ke QuestManager apakah ID Quest ini sedang ON GOING
                if (QuestManager.Instance.GetQuestState(external.questID) == QuestState.Active)
                {
                    Debug.LogWarning(QuestManager.Instance.GetQuestState(external.questID));
                    break;
                }
            }

            QuestManager.Instance.OnNPCTalked(objectID);
        }
        else
            Debug.LogWarning("DialogueData NULL!");
    }
}
