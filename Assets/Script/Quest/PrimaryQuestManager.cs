using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Primary Quest (PQ) flow: an ordered chain of Acts,
/// each containing ordered steps (Dialogue / Quest).
///
/// Flow:
///   Start -> RunStep() -> [Dialogue] wait for onDialogueStop -> AdvanceStep()
///                      -> [Quest]    wait for OnQuestCompleted() -> AdvanceStep()
///   AdvanceStep() -> next step in Act, or next Act, or story end.
///
/// TransitionMode between Acts is flagged via ActData.hasTransitionBeforeNextAct
/// but logic is deferred (TODO hook provided).
/// </summary>
public class PrimaryQuestManager : MonoBehaviour
{
    public Transform player;
    [Header("References")]
    public QuestManager m_questManager;
    //public QuestUI l;

    [Header("Story Entry Point")]
    [Tooltip("The first Act to run when the scene starts.")]
    public ActData startingAct;

    // ── Runtime state ────────────────────────────────────────────────────────
    private ActData _currentAct;
    private int _stepIndex = 0;
    private Coroutine _currentCoroutine;

    // ── Manager refs ─────────────────────────────────────────────────────────
    private DialogueManager m_dialogueManager;
    private GameModeManager m_gameModeManager;

    [HideInInspector]
    public Coroutine transCoroutine;

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

    void Start()
    {
        m_dialogueManager = DialogueManager.Instance;
        m_gameModeManager = GameModeManager.Instance;

        // Subscribe: called every time any dialogue ends
        m_gameModeManager.onDialogueStop += OnDialogueStop;

        _currentAct = startingAct;
        _stepIndex = 0;

        // Kick off initial transition, then run first step
        m_gameModeManager.Switch(m_gameModeManager.TransitionMode);

        RunStep();
        //m_gameModeManager.onDialogueStop -= OnDialogueStop;
    }

    void OnDestroy()
    {
        if (m_gameModeManager != null)
        {
            m_gameModeManager.onDialogueStop -= OnDialogueStop;
            m_gameModeManager.onTransitionStop -= OnTransitionStopForNextAct;
        }
    }

    // =========================================================================
    // Event Handlers (called by external systems)
    // =========================================================================

    /// <summary>
    /// Subscribed to GameModeManager.onDialogueStop.
    /// Fires after ANY dialogue ends — guard with IsIgnoredbyPQ.
    /// </summary>
    private void OnDialogueStop(GameModeManager gmm)
    {
        // Ignore dialogues that are not part of the PQ flow (NPC chats, SQ, etc.)
        if (m_dialogueManager.DialogueData == null) return;
        if (m_dialogueManager.DialogueData.IsIgnoredbyPQ) return;

        // Only advance if the current step is a Dialogue step
        if (_currentAct == null) return;
        PQStep current = _currentAct.steps[_stepIndex];
        if (current.type != PQStepType.Dialogue) return;

        AdvanceStep();
    }

    /// <summary>
    /// Called by QuestManager.ConfigCurrentActQuest() when a primary quest completes.
    /// </summary>
    public void OnQuestCompleted(string questID)
    {
        if (_currentAct == null) return;

        PQStep current = _currentAct.steps[_stepIndex];

        // Guard: only react to the quest we're actually waiting on
        if (current.type != PQStepType.Quest || current.questID != questID)
        {
            Debug.LogWarning($"[PQM] OnQuestCompleted called with '{questID}' but current step expects '{current.questID}'. Ignoring.");
            return;
        }

        AdvanceStep();
    }

    // =========================================================================
    // Core Flow
    // =========================================================================

    /// <summary>
    /// Moves to the next step. If the Act is exhausted, moves to nextAct.
    /// </summary>
    private void AdvanceStep()
    {
        _stepIndex++;

        if (_stepIndex >= _currentAct.steps.Count)
        {
            OnActCompleted();
            return;
        }

        RunStep();
    }

    /// <summary>
    /// Called when all steps in the current Act are done.
    /// </summary>
    private void OnActCompleted()
    {
        Debug.Log($"[PQM] Act '{_currentAct.actID}' completed.");

        if (_currentAct.nextAct == null)
        {
            if (!string.IsNullOrEmpty(_currentAct.targetSceneName))
            {
                m_gameModeManager.Switch(m_gameModeManager.TransitionMode);
                transCoroutine = StartCoroutine(WaitAndNotify());
            }
            return;
        }

        if (_currentAct.isSoftTransition && _currentAct.targetSpawnPoint != null)
        {
            m_gameModeManager.Switch(m_gameModeManager.TransitionMode);
            // Teleport player, tidak load scene

            // Kalau ada Act berikutnya setelah teleport, bisa di-chain ke StartNextAct

            transCoroutine = StartCoroutine(WaitAndNotify());
        }

        if (_currentAct.hasTransitionBeforeNextAct)
        {
            m_gameModeManager.onTransitionStop += OnTransitionStopForNextAct;
            m_gameModeManager.Switch(m_gameModeManager.TransitionMode);
            return;
        }

        StartNextAct(_currentAct.nextAct);
    }

    private void OnTransitionStopForNextAct(GameModeManager GMM)
    {
        // Langsung unsubscribe — one-shot
        m_gameModeManager.onTransitionStop -= OnTransitionStopForNextAct;
        StartNextAct(_currentAct.nextAct);
    }

    /// <summary>
    /// Switches to the given Act and runs its first step.
    /// </summary>
    private void StartNextAct(ActData nextAct)
    {
        _currentAct = nextAct;
        _stepIndex = 0;
        Debug.Log($"[PQM] Starting Act '{_currentAct.actID}'.");
        RunStep();
    }

    /// <summary>
    /// Executes the current step based on its type.
    /// Always stops any existing coroutine before starting a new one.
    /// </summary>
    private void RunStep()
    {
        if (_currentAct == null || _stepIndex >= _currentAct.steps.Count)
        {
            Debug.LogError("[PQM] RunStep() called with invalid state.");
            return;
        }

        StopCurrentCoroutine();

        PQStep step = _currentAct.steps[_stepIndex];

        switch (step.type)
        {
            case PQStepType.Dialogue:
                _currentCoroutine = StartCoroutine(RunDialogueStep(step));
                break;

            case PQStepType.Quest:
                RunQuestStep(step);
                break;

            default:
                Debug.LogError($"[PQM] Unknown PQStepType: {step.type}");
                break;
        }
    }

    // =========================================================================
    // Step Executors
    // =========================================================================

    private IEnumerator RunDialogueStep(PQStep step)
    {
        Debug.Log($"[PQM] Dialogue step — waiting {step.waitTransitionTime}s then starting '{step.dialogueData.name}'.");

        yield return new WaitForSeconds(step.waitTransitionTime);

        m_gameModeManager.Switch(m_gameModeManager.DialogueMode);
        m_dialogueManager.DialogueData = step.dialogueData;
        m_dialogueManager.StartDialogue(step.dialogueData.DialogueNodes);

        _currentCoroutine = null;
        // Flow continues via OnDialogueStop() callback
    }

    private void RunQuestStep(PQStep step)
    {
        Debug.Log($"[PQM] Quest step — adding quest '{step.questID}'.");
        m_questManager.AddPrimaryQuest(step.questID);
        // Flow continues via OnQuestCompleted() callback
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private void StopCurrentCoroutine()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    public IEnumerator WaitAndNotify()
    {
        ActData currentAct = _currentAct;
        //// Nunggu satu frame biar Animator-nya update ke state baru
        yield return null;

        //// Ambil durasi animasi yang sedang jalan sekarang
        //float duration = anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(1);

        UIManager.Instance.fadeImage.StartFadeIn();
        yield return new WaitForSeconds(1.5f);


        if (currentAct.nextAct == null)
        {
            Debug.Log("Fading....");
            //yield return new WaitForSeconds(1.5f);
            Debug.Log("Transisi");
            SceneManager.LoadScene(currentAct.targetSceneName);
        }
        else
        {
            player.transform.position = currentAct.targetSpawnPoint.position;

            Debug.Log("Fading....");
            yield return new WaitForSeconds(1.5f);
            Debug.Log("Transisi");
            
            UIManager.Instance.fadeImage.StartFadeOut();
        }
    }
}
