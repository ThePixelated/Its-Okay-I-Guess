using UnityEngine;

public class CardMode : GameModeBase
{
    private bool isInteractable = false;

    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter CardMode...");
        GMM.SetCurrGameModeIndicator(GameMode.CardMode);

        CameraManager.Instance.ChangeActiveCamera(true);
        //CameraManager.Instance.CameraCardIsLocked = true;
    }

    public override void Update(GameModeManager GMM)
    {
        if (Input.GetKey(KeyCode.Alpha0)) // Forward
        {
            GMM.Switch(GMM.ExplorationMode);
        }
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting CardMode...");
        GMM.SetPrevGameModeIndicator(GameMode.CardMode);

        CameraManager.Instance.ChangeActiveCamera(false);
    }
}
