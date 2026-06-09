using UnityEngine;

public class DialogueMode : GameModeBase
{
    public bool test;

    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter DialogueMode...");

        //PlayerManager.Instance.InteractKey_E(GMM.InteratableID);

        GMM.onDialogueStop += Switching;

        GMM.SetCurrGameModeIndicator(GameMode.DialogueMode);

        QuestManager.Instance.m_questUI.HidePanelQuest();
    }

    public override void Update(GameModeManager GMM)
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Dialog Interact Key Pressed..." + Input.location);
            PlayerManager.Instance.InteractKey_Dialogs();
        }

        // ini update dari custom FSM, nama scriptnya DialogueMode.cs, base arch nya dari GameModeBase
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting DialogueMode...");
        GMM.onDialogueStop -= Switching;
        GMM.SetPrevGameModeIndicator(GameMode.DialogueMode);

        QuestManager.Instance.m_questUI.ShowPanelQuest();
    }

    public void Switching(GameModeManager GMM)
    {
        // if main quest activated, masuk ke
        //GMM.Switch(GMM.CardMode);

        GMM.Switch(GMM.ExplorationMode);
    }

    
}
