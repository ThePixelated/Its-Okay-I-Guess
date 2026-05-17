using UnityEngine;
using UnityEngine.SceneManagement;

public class TabletMode : GameModeBase
{
    private int direction;
    private TabletManager m_TabletManager;

    public override void Enter(GameModeManager GMM)
    {
        Debug.LogWarning("Enter TabletMode...");
        //GMM.SetCurrGameModeIndicator(GameMode.TabletMode);

        m_TabletManager = TabletManager.instance;
        m_TabletManager.InnitTablet();
    }

    public override void Update(GameModeManager GMM)
    {
        // tiap section punya handle switchnya sendiri
        // ...

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = -1;
            m_TabletManager.HandleSectionIndex(direction);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 1;
            m_TabletManager.HandleSectionIndex(direction);
        }

        // Resume Section
        if ((Input.GetKeyDown(KeyCode.Return) && m_TabletManager.GetCurrentSectionIndex() == m_TabletManager.ResumeIndex) || m_TabletManager.IsTargetBtnPressed)
        {
            m_TabletManager.IsTargetBtnPressed = false;
            GMM.Switch(GMM.PreviousMode);
        }

        // Exit Section - hardcoded Exit button di index 6 
        if ((Input.GetKeyDown(KeyCode.Return) && m_TabletManager.GetCurrentSectionIndex() == 6) || m_TabletManager.IsTargetBtnPressed)
        {
            m_TabletManager.IsTargetBtnPressed = false;
            //GMM.Switch(GMM.PreviousMode);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.LogWarning("Exiting TabletMode...");
        m_TabletManager.TerminateTablet();
        //GMM.SetPrevGameModeIndicator(GameMode.TabletMode);
    }

    //public void Switching(GameModeManager GMM)
    //{
    //    GMM.Switch(GMM.ExplorationMode);
    //}
}
