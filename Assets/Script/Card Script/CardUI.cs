using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI text and visual elements on the card and overlay.
///
/// Layout expected in the Canvas:
/// ┌─────────────────────────────────┐
/// │  [LeftChoiceTxt]  [RightChoiceTxt]  ← fixed position, masked by card
/// │         [Card]                      ← draggable card prefab
/// │      [StatementBox]                 ← bottom center, always visible
/// │         [ConfirmBtn]                ← bottom right, toggled by CardMode
/// └─────────────────────────────────┘
///
/// Masking: LeftChoiceTxt and RightChoiceTxt sit BEHIND the card in the hierarchy.
/// A RectMask2D on the card itself clips them — as the card moves left, the left
/// text gets revealed from under the card edge.
/// </summary>
public class CardUI : MonoBehaviour
{
    [Header("Card Visual")]
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private Image cardBackground;

    [Header("Choice Texts (fixed position, revealed by card movement)")]
    [SerializeField] private TextMeshProUGUI leftChoiceTxt;   // firstChoice
    [SerializeField] private TextMeshProUGUI rightChoiceTxt;  // secondChoice

    [Header("Statement Box (always visible, bottom center)")]
    [SerializeField] private TextMeshProUGUI statementTxt;

    [Header("Confirm Button")]
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TextMeshProUGUI confirmBtnTxt;

    [Header("Choice Alpha Settings")]
    [Tooltip("How far (in pixels) card must move before text starts appearing")]
    [SerializeField] private float revealThreshold = 40f;
    [Tooltip("Distance at which text is fully visible")]
    [SerializeField] private float fullRevealDistance = 180f;

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>Populate card with action data.</summary>
    public void SetActionData(ActionChapt action)
    {
        if (statementTxt != null)
            statementTxt.text = action.statement;

        if (leftChoiceTxt != null)
            leftChoiceTxt.text = action.firstChoice;

        if (rightChoiceTxt != null)
            rightChoiceTxt.text = action.secondChoice;

        // Hide choice texts at start
        SetChoiceAlpha(leftChoiceTxt, 0f);
        SetChoiceAlpha(rightChoiceTxt, 0f);
    }

    /// <summary>Populate card with consequence data.</summary>
    public void SetConsequenceData(Consequences cons)
    {
        if (statementTxt != null)
            statementTxt.text = "..."; // consequences don't have a statement

        if (leftChoiceTxt != null)
            leftChoiceTxt.text = cons.firstChoice;

        if (rightChoiceTxt != null)
            rightChoiceTxt.text = cons.secondChoice;

        SetChoiceAlpha(leftChoiceTxt, 0f);
        SetChoiceAlpha(rightChoiceTxt, 0f);
    }

    /// <summary>
    /// Called every frame by CardController with current horizontal drag offset.
    /// Positive offset = dragging right → reveal right choice.
    /// Negative offset = dragging left  → reveal left choice.
    /// </summary>
    public void UpdateChoiceReveal(float horizontalOffset)
    {
        if (horizontalOffset < -revealThreshold)
        {
            // Dragging left → show firstChoice (left text)
            float t = Mathf.InverseLerp(-revealThreshold, -fullRevealDistance, horizontalOffset);
            SetChoiceAlpha(leftChoiceTxt, t);
            SetChoiceAlpha(rightChoiceTxt, 0f);
        }
        else if (horizontalOffset > revealThreshold)
        {
            // Dragging right → show secondChoice (right text)
            float t = Mathf.InverseLerp(revealThreshold, fullRevealDistance, horizontalOffset);
            SetChoiceAlpha(leftChoiceTxt, 0f);
            SetChoiceAlpha(rightChoiceTxt, t);
        }
        else
        {
            // Near center — hide both
            SetChoiceAlpha(leftChoiceTxt, 0f);
            SetChoiceAlpha(rightChoiceTxt, 0f);
        }
    }

    /// <summary>Force show one choice fully (after snap).</summary>
    public void ShowChoiceFull(bool isLeft)
    {
        SetChoiceAlpha(leftChoiceTxt, isLeft ? 1f : 0f);
        SetChoiceAlpha(rightChoiceTxt, isLeft ? 0f : 1f);
    }

    /// <summary>Hide both choice texts (before card throw).</summary>
    public void HideChoices()
    {
        SetChoiceAlpha(leftChoiceTxt, 0f);
        SetChoiceAlpha(rightChoiceTxt, 0f);
    }

    /// <summary>Enable or disable the confirm button.</summary>
    public void SetConfirmButton(bool interactable)
    {
        if (confirmBtn != null)
            confirmBtn.interactable = interactable;

        if (confirmBtnTxt != null)
        {
            Color c = confirmBtnTxt.color;
            c.a = interactable ? 1f : 0.4f;
            confirmBtnTxt.color = c;
        }
    }

    public void SetConfirmButtonListener(UnityEngine.Events.UnityAction action)
    {
        if (confirmBtn == null) return;
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(action);
    }

    public RectTransform CardRect => cardRect;

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    private void SetChoiceAlpha(TextMeshProUGUI txt, float alpha)
    {
        if (txt == null) return;
        Color c = txt.color;
        c.a = alpha;
        txt.color = c;
    }
}