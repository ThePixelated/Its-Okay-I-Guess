using UnityEngine;

/// <summary>
/// Singleton MonoBehaviour yang memegang semua referensi komponen Unity
/// untuk CardMode. CardMode (pure class) mengakses semuanya via Instance.
///
/// Attach to: GameObject "CardModeManager" di scene (selalu aktif).
/// CardModeRoot (panel UI kartu) di-toggle aktif/nonaktif oleh CardMode.
/// </summary>
public class CardModeManager : MonoBehaviour
{
    public static CardModeManager Instance { get; private set; }

    [Header("Card Components")]
    public CardController cardController;
    public CardUI cardUI;
    public CardModeTransition transition;

    [Header("Player Data")]
    public PlayerData playerData;

    [Header("Canvas Root")]
    [Tooltip("Root GameObject panel card UI — dimatikan saat tidak aktif")]
    public GameObject cardModeRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pastikan panel card UI mati di awal
        if (cardModeRoot != null)
            cardModeRoot.SetActive(false);
    }

    /// <summary>
    /// Dipanggil oleh PrimaryQuestManager sebelum GMM.Switch(CardMode).
    /// Menyimpan data chapter yang akan dipakai sesi ini.
    /// </summary>
    public void PrepareCardSession(DataChapter dataChapter, int sectionIndex)
    {
        // Teruskan ke CardMode via GameModeManager
        var cardMode = GameModeManager.Instance.CardMode as CardMode;
        if (cardMode != null)
            cardMode.SetData(dataChapter, sectionIndex);
        else
            Debug.LogError("[CardModeManager] CardMode cast gagal!");
    }
}