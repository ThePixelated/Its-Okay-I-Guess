using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{   
    public static QuestManager Instance;

    public QuestUI m_questUI;
    public PrimaryQuestManager m_primaryQuestManager;

    [Header("Assign Data Quest")]
    public List<Quest> mainQuestDB = new List<Quest>();
    public List<Quest> subMainQuestDB = new List<Quest>();
    public List<Quest> sideQuestDB = new List<Quest>();

    [Header("Tracked Active Quest")]
    public Quest currentActiveActQuest;
    public Quest currentActiveSQ;
    public List<Quest> onHoldSQ = new List<Quest>();
    public List<Quest> onSuccessSQ = new List<Quest>();

    public Dictionary<string, Quest> questLookUp = new Dictionary<string, Quest>();
    //public Dictionary<string, QuestState> sideQuestDatabase = new Dictionary<string, QuestState>();

    private void Awake()
    {
        Instance = this;

        InnitNodeLookupQuest(mainQuestDB);
        InnitNodeLookupQuest(subMainQuestDB);
        InnitNodeLookupQuest(sideQuestDB);
    }

    private void InnitNodeLookupQuest(List<Quest> targetQuest)
    {
        foreach (var questData in targetQuest)
        {
            questData.ResetValue();
            foreach (var item in questData.objectives)
            {
                if (item.ObjectivesIDs != null || item.ObjectivesIDs.Count > 0)
                {
                    item.InnitObjectiveIDs();
                }
            }
        }

        foreach (var questData in targetQuest)
        {
            questLookUp.Add(questData.QuestID, questData);
            //sideQuestDatabase.Add(questData.QuestID, questData.questState);
            Debug.LogWarning("Innit Quest DB.... - " + questData.QuestID);
        }
    }

    public void OnNPCInteract(string npcID, GameObject npcGameObj)
    {
        if (currentActiveSQ != null)
            InteractQuestHandler(currentActiveSQ, npcID, npcGameObj, true);

        Debug.LogWarning("---------- DEVIDER ----------");
        
        if (currentActiveActQuest != null)
            InteractQuestHandler(currentActiveActQuest, npcID, npcGameObj, false);
    }

    public void InteractQuestHandler(Quest quest, string npcID, GameObject npcGameObj, bool isActiveSQ)
    {
        foreach (QuestObjective obj in quest.objectives)
        {
            if (obj.ObjectivesIDs.Contains(npcID))
            {
                if (ObjectiveTypeValidation(obj, quest, npcID, npcGameObj, isActiveSQ))
                {
                    Debug.Log($"Currently talk: {npcID} - target objective: {obj.ObjectiveID}");

                    obj.Current_Amount += 1;
                    Debug.Log($"Objective {obj.ObjectiveID} di quest {quest.QuestID} selesai!");

                    obj.ObjectivesIDs.Remove(npcID);

                    SoundEffectManager.Play("StrikeObjective", true);
                }
         
                if (quest.IsAllObjectivesComplete())
                {
                    SoundEffectManager.Play("FinishedPQ");
                    Debug.Log($"Quest {quest.QuestID} siap diselesaikan!");

                    if (isActiveSQ)
                        ConfigOnHoldSQtoCurrentSQ();
                    else
                        ConfigCurrentActQuest();

                    if (currentActiveActQuest == null && currentActiveSQ == null)
                    {
                        m_questUI.HidePanelQuest();
                    }
                }

                m_questUI.UpdateQuestUI();
            }
        }
    }

    public bool ObjectiveTypeValidation(QuestObjective obj, Quest quest, string npcID, GameObject npcGameObj, bool isActiveSQ)
    {
        bool countRequiredfromEndLoc = true;

        switch (obj.Type)
        {
            case ObjectiveType.Single:
                npcGameObj.SetActive(false);
                Debug.Log("Collectable");
                break;
            case ObjectiveType.Collectable:
                npcGameObj.SetActive(false);
                Debug.Log("Collectable");
                break;
            case ObjectiveType.TalkNPC:
                Debug.Log("TalkNPC");
                break;
            case ObjectiveType.GoToLocation:
                countRequiredfromEndLoc = LocationValidation(quest, ObjectiveType.GoToLocation);
                Debug.Log("GOTO LOCATION LOGIC: " + countRequiredfromEndLoc);
                if (countRequiredfromEndLoc)
                {
                    var endLocObj = npcGameObj.GetComponent<ObjectComponent>();
                    endLocObj.PlayTriggerAnim("Start");
                    
                    //if (m_primaryQuestManager.transCoroutine == null)
                    //{
                    //    GameModeManager.Instance.Switch(GameModeManager.Instance.TransitionMode);
                    //    StartCoroutine(m_primaryQuestManager.WaitAndNotify());
                    //}
                }
                break;
            case ObjectiveType.Use:
                //npcGameObj.SetActive(false);
                var tempCompt = npcGameObj.GetComponent<ObjectInteraction>();
                tempCompt.m_objectComponent.SetDefaultSprite();
                Debug.Log("Use");
                break;
            case ObjectiveType.ReachLocation:
                Debug.Log("Reach Location");
                break;
            case ObjectiveType.Interactable:
                Debug.Log("Interacable");
                break;
            case ObjectiveType.EndLocation:
                countRequiredfromEndLoc = LocationValidation(quest, ObjectiveType.EndLocation);
                if (countRequiredfromEndLoc)
                {
                    GameModeManager.Instance.Switch(GameModeManager.Instance.TransitionMode);
                    var endLocObj = npcGameObj.GetComponent<ObjectComponent>();
                    endLocObj.PlayTriggerAnim("Start", ObjectiveType.EndLocation);

                    // Simpan target scene, tapi JANGAN LoadScene di sini
                    //_pendingSceneTransition = quest.targetSceneName; // flag baru di QuestManager
                }
                break;
            case ObjectiveType.Custom:
                var ObjInteract = npcGameObj.GetComponent<ObjectInteraction>();
                ObjInteract.isInteractable = false;
                Debug.Log("Custom");
                break;
        }

        Debug.Log("FINAL VALUE: " + countRequiredfromEndLoc);
        return countRequiredfromEndLoc;
    }

    private bool LocationValidation(Quest quest, ObjectiveType objType)
    {
        int exception = 0;
        bool returnVal = true;
        foreach (QuestObjective otherObj in quest.objectives)
        {
            if (otherObj.Current_Amount < otherObj.RequiredAmount && otherObj.Type != objType)
            {
                returnVal = false;
            }

            //if (otherObj.Type == ObjectiveType.GoToLocation)
            //{
            //    exception++;
            //}

            //if (exception >= 2)
            //{
            //    returnVal = true;
            //}
        }
        return returnVal;
    }

    public void ConfigOnHoldSQtoCurrentSQ()
    {
        if (onHoldSQ != null && onHoldSQ.Count > 0)
        {
            currentActiveSQ.questState = QuestState.Success;

            onSuccessSQ.Add(currentActiveSQ);
            onHoldSQ.Remove(currentActiveSQ);

            currentActiveSQ = onHoldSQ[0];

            onHoldSQ.Remove(currentActiveSQ);
            currentActiveSQ.questState = QuestState.Active;
        }
    }

    public void ConfigCurrentActQuest()
    {
        string questID = currentActiveActQuest.QuestID;

        currentActiveActQuest.questState = QuestState.Success;
        currentActiveActQuest = null;

        m_primaryQuestManager.OnQuestCompleted(questID);
    }

    public void SuccessQuest(Quest quest)
    {
        //gapapap kalo ga pake quesy manager soS
    }

    public void ChangeToCardGame()
    {
        GameModeManager.Instance.Switch(GameModeManager.Instance.CardMode);
    }

    public void AddPrimaryQuest(string questID)
    {
        Debug.Log("Primary Quest Added!");
        questLookUp[questID].questState = QuestState.Active;

        currentActiveActQuest = questLookUp[questID];
        m_questUI.UpdateQuestUI();

        m_questUI.ShowPanelQuest();
        SoundEffectManager.Play("AddedPQ");
    }

    public void AddQuest(string questID)
    {
        Debug.Log("Primary Quest Added!");
        questLookUp[questID].questState = QuestState.Active;

        if (currentActiveSQ != null)
        {
            currentActiveSQ.questState = QuestState.OnHold;
            onHoldSQ.Add(currentActiveSQ);
        }

        currentActiveSQ = questLookUp[questID];

        m_questUI.UpdateQuestUI();
    }

    //public Dictionary<int, QuestState> mainQuestDatabase = new Dictionary<int, QuestState>();
    //public QuestState GetMainQuestState(int sqID)
    //{
    //    if (mainQuestDatabase.ContainsKey(sqID))
    //    {
    //        Debug.Log($"QUEST State RETRIVE: {mainQuestDatabase[sqID]}");
    //        return mainQuestDatabase[sqID];
    //    }
    //    return QuestState.Unassigned; // Default jika belum terdaftar
    //}
    
    //public QuestState GetSideQuestState(string sqID)
    //{
    //    if (sideQuestDatabase.ContainsKey(sqID))
    //    {
    //        Debug.Log($"QUEST State RETRIVE: {sideQuestDatabase[sqID]}");
    //        return sideQuestDatabase[sqID];
    //    }
    //    return QuestState.Unassigned; // Default jika belum terdaftar
    //}

    public QuestState GetQuestState(string sqID)
    {
        if (questLookUp.ContainsKey(sqID))
        {
            Debug.Log($"QUEST State RETRIVE: {questLookUp[sqID]}");
            return questLookUp[sqID].questState;
        }
        return QuestState.Unassigned; // Default jika belum terdaftar
    }
}

public enum QuestState
{
    Unassigned, // Belum diambil / Belum mulai
    Active,     // Sedang berjalan / Belum selesai
    OnHold, 
    Success     // Sudah selesai
}
