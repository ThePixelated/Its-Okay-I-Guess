using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

[RequireComponent(typeof(ObjectComponent))]
public class ObjectInteraction : MonoBehaviour
{
    public bool isInteractable = true;

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

    [HideInInspector]
    public ObjectComponent m_objectComponent;

    // Fungsi ini yang akan dipanggil oleh DialogueManager saat Player menekan tombol interaksi
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (namaTxt != null)
        {
            namaTxt.text = gameObject.name;
            //Debug.Log(namaTxt.text);
        }
        
        objectID = gameObject.name;

        if (GetComponent<ObjectComponent>() == null)
        {
            gameObject.AddComponent<ObjectComponent>();
            //Debug.Log($"Otomatis nambahin ScriptX di {gameObject.name} biar gak error pas runtime!");
        }
    }
    #endif

    private void Awake()
    {
        isInteractable = true;
    }

    private void Start()
    {
        PlayerManager.Instance.onInteractKey_E += Interact;
        isInteractable = true;

        m_objectComponent = GetComponent<ObjectComponent>();
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
        return DialogueRetriever.DetermineDialogue(this);
    }

    public void Interact(string objectName)
    {
        if (gameObject.name == objectName && isInteractable)
        {
            Debug.LogWarning($"Object {gameObject.name} is selected! - Target gObj: {objectName}");
            List<DialogueData> chosenList = GetCurrentDialogueData();

            if (chosenList != null)
            {
                int randomIndex = UnityEngine.Random.Range(0, chosenList.Count);
                DialogueData selectedDialogue = chosenList[randomIndex];
                //Debug.Log();

                DialogueManager.Instance.DialogueData = selectedDialogue;
                DialogueManager.Instance.StartDialogue(selectedDialogue.DialogueNodes);
                GameModeManager.Instance.Switch(GameModeManager.Instance.DialogueMode);
            }

            QuestManager.Instance.OnNPCInteract(objectID, gameObject);
        }
        else
            Debug.Log($"Object {gameObject.name} is selected! - Target gObj: {objectName}");
    }
}