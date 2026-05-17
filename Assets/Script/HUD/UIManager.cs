
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Config Scripts")]
    public UILoader m_UILoader;
    public StatsHUD m_StatsHUD;

    [Header("Config GameObjects")]
    [SerializeField] private GameObject statsHUD;
    [SerializeField] private GameObject questHUD;
    [SerializeField] private GameObject cardUI;
    
    public ScreenFader fadeImage;

    private void Awake()
    {
        Instance = this;
    }

    public void GoToScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
}
