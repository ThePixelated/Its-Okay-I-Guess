using UnityEngine;

public abstract class GameModeBase
{
    public abstract void Enter(GameModeManager GMM);
    public abstract void Update(GameModeManager GMM);
    public abstract void Exit(GameModeManager GMM);
}
