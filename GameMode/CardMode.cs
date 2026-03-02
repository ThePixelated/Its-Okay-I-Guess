using UnityEngine;

public class CardMode : GameModeBase
{
    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter CardMode...");
    }

    public override void Update(GameModeManager GMM)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GMM.Switch(GMM.ExplorationMode);
        }

        if (Input.GetKey(KeyCode.W)) // Forward
        {
            Debug.Log("Press W");
        }
        else if (Input.GetKey(KeyCode.A)) // Left
        {
            Debug.Log("Press A");
        }

        else if (Input.GetKey(KeyCode.S)) // Backward
        {
            Debug.Log("Press S");
        }

        else if (Input.GetKey(KeyCode.D)) // Right
        {
            Debug.Log("Press D");
        }
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting CardMode...");
    }
}
