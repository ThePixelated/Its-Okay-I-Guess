using Unity.VisualScripting;
using UnityEngine;

public class DialogueMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter DialogueMode...");
        GMM.onDialogueStop += Switching;
    }

    public override void Update(GameModeManager GMM)
    {
        GMM.HandleSwitchMode();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Dialog Interact Key Pressed...");
            PlayerManager.Instance.InteractKey_Dialogs();
        }
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting DialogueMode...");
    }

    public void Switching(GameModeManager GMM)
    {
        GMM.Switch(GMM.ExplorationMode);
    }
}
