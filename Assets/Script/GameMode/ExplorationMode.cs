using UnityEngine;

public class ExplorationMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter ExplorationMode...");
    }

    public override void Update(GameModeManager GMM)
    {
        GMM.HandleCharDirection();
        GMM.HandlePlayerMovement();

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GMM.Switch(GMM.CardMode);
        }
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting ExplorationMode...");
    }
}
