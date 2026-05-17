using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryQuestManager : MonoBehaviour
{
    public ActData currentAct;

    private int _stepIndex = 0;
    private Coroutine _currentCoroutine;

    private DialogueManager m_dialogueManager;
    private GameModeManager m_gameModeManager;
    public QuestManager m_questManager;

    void Start()
    {
        m_dialogueManager = DialogueManager.Instance;
        m_gameModeManager = GameModeManager.Instance;

        GameModeManager.Instance.onDialogueStop += OnDialogueStop;

        m_gameModeManager.Switch(m_gameModeManager.TransitionMode);
        RunStep(); // kick off Act pertama, step pertama
    }

    // Dipanggil otomatis setiap kali dialogue selesai
    private void OnDialogueStop(GameModeManager gmm)
    {
        // Guard: kalau dialogue yang baru selesai bukan bagian PQ, ignore
        if (m_dialogueManager.DialogueData.IsIgnoredbyPQ) return;

        AdvanceStep();
    }

    // Dipanggil dari QuestManager ketika quest selesai
    public void OnQuestCompleted()
    {
        // Pastikan quest yang selesai memang quest di step ini
        //PQStep current = currentAct.steps[_stepIndex];
        //if (current.type != PQStepType.Quest || current.questId != questId) return;

        AdvanceStep();
    }

    private void AdvanceStep()
    {
        _stepIndex++;

        if (_stepIndex >= currentAct.steps.Count)
        {
            // Act selesai
            if (currentAct.nextAct != null)
            {
                currentAct = currentAct.nextAct;
                _stepIndex = 0;
                RunStep();
            }
            else
            {
                Debug.Log("Story selesai.");
            }
            return;
        }

        RunStep();
    }

    private void RunStep()
    {
        PQStep step = currentAct.steps[_stepIndex];

        // Stop coroutine lama dulu sebelum mulai yang baru
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        switch (step.type)
        {
            case PQStepType.Dialogue:
                _currentCoroutine = StartCoroutine(RunDialogueStep(step));
                break;

            case PQStepType.Quest:
                m_questManager.AddPrimaryQuest(step.questId);
                // Tidak advance di sini — nunggu OnQuestCompleted() dipanggil
                break;
        }
    }

    private IEnumerator RunDialogueStep(PQStep step)
    {
        yield return new WaitForSeconds(step.waitTransitionTime);
        m_gameModeManager.Switch(m_gameModeManager.DialogueMode);
        m_dialogueManager.DialogueData = step.dialogueData;
        m_dialogueManager.StartDialogue(step.dialogueData.DialogueNodes);
        _currentCoroutine = null;
    }
}