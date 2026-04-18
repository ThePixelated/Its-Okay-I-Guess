using UnityEngine;

public class DialogueMode : GameModeBase
{
    public bool test;

    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter DialogueMode...");
        GMM.onDialogueStop += Switching;

        GMM.SetCurrGameModeIndicator(GameMode.DialogueMode);
    }

    public override void Update(GameModeManager GMM)
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Dialog Interact Key Pressed...");
            PlayerManager.Instance.InteractKey_Dialogs();
        }

        // mouse skip dialog input box ?
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting DialogueMode...");
        GMM.SetPrevGameModeIndicator(GameMode.DialogueMode);
    }

    public void Switching(GameModeManager GMM)
    {
        // if main quest activated, masuk ke
        //GMM.Switch(GMM.CardMode);

        GMM.Switch(GMM.ExplorationMode);
    }
}
