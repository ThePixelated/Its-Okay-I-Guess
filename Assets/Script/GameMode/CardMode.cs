using UnityEngine;

public class CardMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter CardMode...");
        GMM.SetCurrGameModeIndicator(GameMode.CardMode);

        GMM.HandleCameraChange(false, true);
        CameraManager.Instance.CameraCardIsLocked = true;
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

        if (!CameraManager.Instance.CameraCardIsLocked)
            GMM.HandleCameraChange(true, false);

        Debug.LogWarning("Test...");
    }
}
