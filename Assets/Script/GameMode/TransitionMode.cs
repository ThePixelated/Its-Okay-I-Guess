using UnityEngine;

public class TransitionMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter TransitionMode...");
        GMM.SetCurrGameModeIndicator(GameMode.TransitionMode);
    }

    public override void Update(GameModeManager GMM)
    {

    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exit TransitionMode...");
        GMM.SetPrevGameModeIndicator(GameMode.TransitionMode);

        GMM.TransitionStop();
    }
}
