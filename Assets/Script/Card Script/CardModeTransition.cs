using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual transition INTO and OUT OF CardMode.
/// - Enter: world desaturates + slow-mo → card panel fades in → card thrown to center
/// - Exit:  card flies out → panel fades out → world restores
///
/// Attach to: the CardMode root GameObject (same as CardMode component).
/// </summary>
public class CardModeTransition : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The full-screen panel behind the card UI (semi-transparent overlay)")]
    [SerializeField] private CanvasGroup cardPanelGroup;

    [Tooltip("The card RectTransform that will be thrown into the center")]
    [SerializeField] private RectTransform cardRect;

    [Tooltip("Post-process volume or a full-screen Image used to simulate desaturation")]
    [SerializeField] private Image desaturateOverlay; // simple grayscale overlay trick

    [Header("Timing")]
    [SerializeField] private float desaturateDuration = 0.4f;
    [SerializeField] private float panelFadeDuration = 0.25f;
    [SerializeField] private float throwDuration = 0.5f;
    [SerializeField] private float flyOutDuration = 0.35f;

    [Header("Card Throw Settings")]
    [Tooltip("Off-screen spawn position (in anchored coords) where card starts before throw")]
    [SerializeField] private Vector2 throwStartOffset = new Vector2(0f, 900f); // above screen
    [SerializeField] private Vector2 cardCenterPosition = Vector2.zero;

    [Header("Slow-mo Settings")]
    [SerializeField] private float slowMoTimeScale = 0.15f;

    // Internal
    private Vector2 _cardHomePosition;
    private Coroutine _activeRoutine;

    private void Awake()
    {
        _cardHomePosition = cardCenterPosition;

        if (desaturateOverlay != null)
        {
            Color c = desaturateOverlay.color;
            c.a = 0f;
            desaturateOverlay.color = c;

            // CRITICAL: this overlay sits above the card visually but must
            // NOT intercept pointer events meant for the draggable card.
            desaturateOverlay.raycastTarget = false;
        }

        if (cardPanelGroup != null)
        {
            cardPanelGroup.alpha = 0f;
            // CanvasGroup itself shouldn't block raycasts either — only the
            // card's own Image should be a raycast target.
            cardPanelGroup.blocksRaycasts = true; // panel background can stay blocking, card is a child so it still receives events
        }
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>Plays the enter transition, calls onComplete when card has landed.</summary>
    public void PlayEnter(Action onComplete)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(EnterRoutine(onComplete));
    }

    /// <summary>Plays exit transition for Action phase: card flies to chosen side.</summary>
    public void PlayCardFlyOut(bool flyRight, Action onComplete)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(CardFlyOutRoutine(flyRight, onComplete));
    }

    /// <summary>Plays exit transition: card flies out, panel fades, world restores.</summary>
    public void PlayExit(bool flyRight, Action onComplete)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(ExitRoutine(flyRight, onComplete));
    }

    /// <summary>Just throw a new card in (used for consequence phase).</summary>
    public void PlayCardThrow(Action onComplete)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(CardThrowRoutine(onComplete));
    }

    // ─────────────────────────────────────────────
    //  COROUTINES
    // ─────────────────────────────────────────────

    private IEnumerator EnterRoutine(Action onComplete)
    {
        // 1. Slow-mo
        Time.timeScale = slowMoTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2. Desaturate overlay fade in (unscaled time so it isn't affected by slow-mo)
        yield return StartCoroutine(FadeOverlay(0f, 0.55f, desaturateDuration));

        // 3. Panel fade in
        if (cardPanelGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(cardPanelGroup, 0f, 1f, panelFadeDuration));

        // 4. Restore time before card animation (card throw should feel snappy)
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // 5. Throw card from above
        yield return StartCoroutine(CardThrowRoutine(onComplete));
    }

    private IEnumerator CardThrowRoutine(Action onComplete)
    {
        if (cardRect == null) { onComplete?.Invoke(); yield break; }

        // Reset card to spawn point above screen
        cardRect.anchoredPosition = _cardHomePosition + throwStartOffset;
        cardRect.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-15f, 15f));
        cardRect.localScale = Vector3.one;

        float elapsed = 0f;
        Vector2 startPos = cardRect.anchoredPosition;
        Quaternion startRot = cardRect.localRotation;

        while (elapsed < throwDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / throwDuration;

            // Ease out cubic for position
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            cardRect.anchoredPosition = Vector2.Lerp(startPos, _cardHomePosition, eased);
            cardRect.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, eased);

            yield return null;
        }

        cardRect.anchoredPosition = _cardHomePosition;
        cardRect.localRotation = Quaternion.identity;

        // Small bounce overshoot
        yield return StartCoroutine(BounceCard());

        onComplete?.Invoke();
    }

    private IEnumerator BounceCard()
    {
        float bounceDuration = 0.2f;
        float elapsed = 0f;
        Vector3 baseScale = Vector3.one;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / bounceDuration;
            float bounce = Mathf.Sin(t * Mathf.PI); // 0 → 1 → 0
            cardRect.localScale = baseScale + new Vector3(bounce * 0.08f, -bounce * 0.08f, 0f);
            yield return null;
        }

        cardRect.localScale = Vector3.one;
    }

    private IEnumerator CardFlyOutRoutine(bool flyRight, Action onComplete)
    {
        if (cardRect == null) { onComplete?.Invoke(); yield break; }

        float elapsed = 0f;
        Vector2 startPos = cardRect.anchoredPosition;
        float targetX = flyRight ? 1200f : -1200f;
        Vector2 targetPos = new Vector2(targetX, startPos.y - 200f);
        Quaternion startRot = cardRect.localRotation;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, flyRight ? -30f : 30f);

        while (elapsed < flyOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Pow(elapsed / flyOutDuration, 2f); // ease in
            cardRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            cardRect.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        onComplete?.Invoke();
    }

    private IEnumerator ExitRoutine(bool flyRight, Action onComplete)
    {
        // 1. Fly card out
        yield return StartCoroutine(CardFlyOutRoutine(flyRight, null));

        // 2. Fade panel
        if (cardPanelGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(cardPanelGroup, 1f, 0f, panelFadeDuration));

        // 3. Remove desaturation
        yield return StartCoroutine(FadeOverlay(0.55f, 0f, desaturateDuration));

        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        if (desaturateOverlay == null) yield break;
        float elapsed = 0f;
        Color c = desaturateOverlay.color;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            desaturateOverlay.color = c;
            yield return null;
        }
        c.a = to;
        desaturateOverlay.color = c;
    }
}