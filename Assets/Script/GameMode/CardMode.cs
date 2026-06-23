using UnityEngine;

/// <summary>
/// CardMode — pure class (GameModeBase), semua refs Unity diambil dari CardModeManager.Instance.
///
/// FLOW:
/// 1. PrimaryQuestManager panggil CardModeManager.Instance.PrepareCardSession(data, index)
/// 2. GMM.Switch(CardMode) → Enter() dipanggil
/// 3. Transition: desaturate + slow-mo + kartu dilempar ke tengah
/// 4. Phase Action: player drag kartu, snap, tekan "Pilih Aksi" → statEffect diterapkan
/// 5. Kartu fly out → Phase Consequence: kartu baru dilempar
/// 6. Player drag lagi (visual only) → tekan "Pilih Aksi" → kartu fly out → balik Exploration
/// </summary>
public class CardMode : GameModeBase
{
    /// <summary>
    /// Fired right before switching back to ExplorationMode, when the
    /// full Action → Consequence flow has finished. PrimaryQuestManager
    /// subscribes to this to know when to AdvanceStep().
    /// </summary>
    public event System.Action OnCardActionFinished;

    // ── Runtime state ──
    private ChaptSection _currentSection;
    private string _currentActID;

    private enum Phase { Action, Consequence }
    private Phase _phase;

    private bool _waitingForConfirm = false;
    private bool _chosenLeft = false;

    private GameModeManager _gmm;

    // Shorthand refs dari singleton
    private CardModeManager CMM => CardModeManager.Instance;

    // ─────────────────────────────────────────────
    //  GAME MODE BASE OVERRIDES
    // ─────────────────────────────────────────────

    public override void Enter(GameModeManager GMM)
    {
        _gmm = GMM;
        GMM.SetCurrGameModeIndicator(GameMode.CardMode);

        Debug.Log("[CardMode] Enter");

        if (CMM == null)
        {
            Debug.LogError("[CardMode] CardModeManager.Instance is null!");
            return;
        }

        // Aktifkan panel card UI
        if (CMM.cardModeRoot != null)
            CMM.cardModeRoot.SetActive(true);

        // Setup confirm button
        CMM.cardUI.SetConfirmButton(false);
        CMM.cardUI.SetConfirmButtonListener(OnConfirmPressed);

        // Subscribe card events
        CMM.cardController.OnSnapped += OnCardSnapped;
        CMM.cardController.OnReturnedToCenter += OnCardReturnedToCenter;
        CMM.cardController.OnDragOffset += OnCardDragOffset;

        // Mulai transisi masuk
        CMM.transition.PlayEnter(OnEnterTransitionComplete);
    }

    public override void Update(GameModeManager GMM)
    {
        // Debug: paksa keluar
        if (Input.GetKeyDown(KeyCode.Escape))
            ForceExit();
    }

    public override void Exit(GameModeManager GMM)
    {
        Debug.Log("[CardMode] Exit");
        GMM.SetPrevGameModeIndicator(GameMode.CardMode);

        if (CMM == null) return;

        // Unsubscribe
        CMM.cardController.OnSnapped -= OnCardSnapped;
        CMM.cardController.OnReturnedToCenter -= OnCardReturnedToCenter;
        CMM.cardController.OnDragOffset -= OnCardDragOffset;

        if (CMM.cardModeRoot != null)
            CMM.cardModeRoot.SetActive(false);
    }

    // ─────────────────────────────────────────────
    //  PUBLIC — dipanggil CardModeManager.PrepareCardSession
    // ─────────────────────────────────────────────

    public void SetData(DataChapter dataChapter, int sectionIndex, string actID)
    {
        _currentSection = dataChapter.ChapterSections[sectionIndex];
        _currentActID = actID;
        Debug.Log($"[CardMode] Data set: {_currentSection.Title} (source: {_currentActID})");
    }

    // ─────────────────────────────────────────────
    //  ENTER TRANSITION SELESAI
    // ─────────────────────────────────────────────

    private void OnEnterTransitionComplete()
    {
        _phase = Phase.Action;
        LoadActionPhase();
    }

    // ─────────────────────────────────────────────
    //  PHASE 1 — ACTION
    // ─────────────────────────────────────────────

    private void LoadActionPhase()
    {
        _waitingForConfirm = false;
        CMM.cardUI.HideChoices();
        CMM.cardUI.SetActionData(_currentSection.actionSection);
        CMM.cardUI.SetConfirmButton(false);
        CMM.cardController.ResetToCenter();
        CMM.cardController.SetInteractable(true);

        Debug.Log("[CardMode] Phase: Action");
    }

    // ─────────────────────────────────────────────
    //  PHASE 2 — CONSEQUENCE
    // ─────────────────────────────────────────────

    private void LoadConsequencePhase()
    {
        int consIndex = _chosenLeft ? 0 : 1;

        if (_currentSection.consSection == null ||
            _currentSection.consSection.Count <= consIndex)
        {
            Debug.LogWarning("[CardMode] Consequence data tidak ditemukan, skip ke exit.");
            FinishCardMode();
            return;
        }

        Consequences cons = _currentSection.consSection[consIndex];

        CMM.cardUI.HideChoices();
        CMM.cardUI.SetConsequenceData(cons);
        CMM.cardUI.SetConfirmButton(false);

        // Lempar kartu baru
        CMM.transition.PlayCardThrow(() =>
        {
            CMM.cardController.ResetToCenter();
            CMM.cardController.SetInteractable(true);
            _waitingForConfirm = false;
            _phase = Phase.Consequence;
            Debug.Log("[CardMode] Phase: Consequence");
        });
    }

    // ─────────────────────────────────────────────
    //  CARD EVENTS
    // ─────────────────────────────────────────────

    private void OnCardSnapped(bool isLeft)
    {
        _waitingForConfirm = true;
        _chosenLeft = isLeft;
        CMM.cardUI.ShowChoiceFull(isLeft);
        CMM.cardUI.SetConfirmButton(true);
        Debug.Log($"[CardMode] Snap → {(isLeft ? "KIRI" : "KANAN")}");
    }

    private void OnCardReturnedToCenter()
    {
        _waitingForConfirm = false;
        CMM.cardUI.HideChoices();
        CMM.cardUI.SetConfirmButton(false);
        CMM.cardController.SetInteractable(true);
    }

    private void OnCardDragOffset(float offset)
    {
        CMM.cardUI.UpdateChoiceReveal(offset);
    }

    // ─────────────────────────────────────────────
    //  CONFIRM BUTTON
    // ─────────────────────────────────────────────

    private void OnConfirmPressed()
    {
        if (!_waitingForConfirm) return;

        // Lock the card immediately so it can't be grabbed mid fly-out animation
        CMM.cardController.SetInteractable(false);
        _waitingForConfirm = false;

        if (_phase == Phase.Action)
        {
            ActionChapt action = _currentSection.actionSection;

            // Apply stat effect ke PlayerData
            if (CMM.playerData != null)
                CMM.playerData.ApplyStatEffect(action.statEffect);

            // Catat log untuk NLM (hanya Phase Action, Consequence tidak di-log)
            if (ActionLogger.Instance != null)
            {
                string chosenText = _chosenLeft ? action.firstChoice : action.secondChoice;
                CopingTag chosenTag = _chosenLeft ? action.firstChoiceTag : action.secondChoiceTag;

                ActionLogger.Instance.AddLog(
                    _currentSection.Title,
                    _currentActID,
                    chosenText,
                    chosenTag,
                    action.statEffect);
            }

            // Unlock entry ensiklopedia sesuai pilihan
            if (EncyclopediaProgress.Instance != null)
            {
                EncyclopediaEntry chosenEntry = _chosenLeft
                    ? action.firstChoiceEncyclopedia
                    : action.secondChoiceEncyclopedia;

                if (chosenEntry != null)
                    EncyclopediaProgress.Instance.UnlockEntry(chosenEntry);
            }

            // Kartu fly out → load consequence
            CMM.transition.PlayCardFlyOut(!_chosenLeft, () =>
            {
                LoadConsequencePhase();
            });
        }
        else // Consequence
        {
            // Kartu fly out → selesai
            CMM.transition.PlayCardFlyOut(!_chosenLeft, FinishCardMode);
        }
    }

    // ─────────────────────────────────────────────
    //  EXIT
    // ─────────────────────────────────────────────

    private void FinishCardMode()
    {
        CMM.transition.PlayExit(!_chosenLeft, () =>
        {
            OnCardActionFinished?.Invoke();
            _gmm.Switch(_gmm.ExplorationMode);
        });
    }

    private void ForceExit()
    {
        OnCardActionFinished?.Invoke();
        _gmm.Switch(_gmm.ExplorationMode);
    }
}