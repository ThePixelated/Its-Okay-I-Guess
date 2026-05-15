using UnityEngine;

public class PlayerData: MonoBehaviour
{
    public static PlayerData Instance;

    public int Kesehatan;
    public int Sosial;
    public int Energi;
    public int Uang;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }
}
