using UnityEngine;

public class ExplorationMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter ExplorationMode...");
        GMM.SetCurrGameModeIndicator(GameMode.ExplorationMode);
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
        GMM.SetPrevGameModeIndicator(GameMode.ExplorationMode);
    }
}
