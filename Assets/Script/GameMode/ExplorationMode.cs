using UnityEngine;

public class ExplorationMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter ExplorationMode...");
    }

    public override void Update(GameModeManager GMM)
    {
        GMM.HandleSwitchMode();

        GMM.HandleCharDirection();
        GMM.HandlePlayerMovement();
        GMM.HandleObjectInteractable();
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting ExplorationMode...");
    }
}
