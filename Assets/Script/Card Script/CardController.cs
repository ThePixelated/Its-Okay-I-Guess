using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles all card drag interaction:
/// - Mouse + Touch input
/// - Rotation and position interpolation toward ghost placeholders
/// - Drop zone detection (green areas)
/// - Snap to placeholder or return to center
///
/// Attach to: the Card GameObject (which has a RectTransform).
/// Requires: EventSystem in the scene, GraphicRaycaster on Canvas.
/// </summary>
public class CardController : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Card Transform")]
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private Canvas parentCanvas; // needed to convert screen→canvas coords

    [Header("Ghost Placeholder Transforms")]
    [Tooltip("The dashed rectangle on the LEFT side")]
    [SerializeField] private RectTransform leftPlaceholder;
    [Tooltip("The dashed rectangle on the RIGHT side")]
    [SerializeField] private RectTransform rightPlaceholder;

    [Header("Drop Zones (the green areas)")]
    [SerializeField] private RectTransform leftDropZone;
    [SerializeField] private RectTransform rightDropZone;

    [Header("Drag Settings")]
    [Tooltip("Max rotation in degrees when card is fully at placeholder")]
    [SerializeField] private float maxRotation = 20f;
    [Tooltip("How fast card follows cursor")]
    [SerializeField] private float dragSmoothing = 0f; // 0 = instant follow
    [Tooltip("Distance at which card is considered 'fully' at placeholder")]
    [SerializeField] private float fullLerpDistance = 220f;

    [Header("Snap Settings")]
    [SerializeField] private float snapDuration = 0.18f;
    [SerializeField] private float returnDuration = 0.22f;

    // ── Events ──
    /// <summary>Fired when card snaps to left. bool = true → left, false → right.</summary>
    public event Action<bool> OnSnapped;
    /// <summary>Fired when card returns to center after release outside zone.</summary>
    public event Action OnReturnedToCenter;
    /// <summary>Fired every frame while dragging. float = horizontal offset from center.</summary>
    public event Action<float> OnDragOffset;

    // ── State ──
    public enum CardState { Idle, Dragging, Snapping, Snapped, Returning }
    private CardState _state = CardState.Idle;

    private Vector2 _centerPosition;
    private Vector2 _dragOffset;      // offset from card pivot to pointer on pick-up
    private bool _isInteractable = false;

    // Snap coroutine data
    private Vector2 _snapStartPos;
    private Quaternion _snapStartRot;
    private Vector2 _snapTargetPos;
    private Quaternion _snapTargetRot;
    private float _snapElapsed;
    private float _snapDur;

    private bool _snappedLeft;

    // ─────────────────────────────────────────────
    //  INIT
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (cardRect == null) cardRect = GetComponent<RectTransform>();
        _centerPosition = cardRect.anchoredPosition;
    }

    public void SetInteractable(bool value) => _isInteractable = value;

    /// <summary>Reset card to center, clear snap state.</summary>
    public void ResetToCenter()
    {
        cardRect.anchoredPosition = _centerPosition;
        cardRect.localRotation = Quaternion.identity;
        _state = CardState.Idle;
        _isInteractable = false;
    }

    public bool IsSnapped => _state == CardState.Snapped;
    public bool SnappedLeft => _snappedLeft;

    // ─────────────────────────────────────────────
    //  POINTER EVENTS
    // ─────────────────────────────────────────────

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isInteractable) return;
        if (_state == CardState.Snapping || _state == CardState.Returning) return;

        // Allow picking the card back up even if it's currently Snapped,
        // so the player can change their mind before pressing confirm.
        _state = CardState.Dragging;

        // Calculate pick-up offset so card doesn't jump to pointer center
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        _dragOffset = cardRect.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isInteractable || _state != CardState.Dragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        Vector2 targetPos = localPoint + _dragOffset;

        // Clamp horizontal movement so the card cannot be dragged past
        // the placeholder position on either side.
        float minX = leftPlaceholder != null ? leftPlaceholder.anchoredPosition.x : float.NegativeInfinity;
        float maxX = rightPlaceholder != null ? rightPlaceholder.anchoredPosition.x : float.PositiveInfinity;
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        // Clamp vertical movement to stay within the range spanned by the
        // placeholders and the center position (whichever is highest/lowest),
        // so the card can't be dragged arbitrarily far up/down.
        float leftY = leftPlaceholder != null ? leftPlaceholder.anchoredPosition.y : _centerPosition.y;
        float rightY = rightPlaceholder != null ? rightPlaceholder.anchoredPosition.y : _centerPosition.y;
        float minY = Mathf.Min(_centerPosition.y, leftY, rightY);
        float maxY = Mathf.Max(_centerPosition.y, leftY, rightY);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        cardRect.anchoredPosition = targetPos;

        // Calculate horizontal progress toward each placeholder
        float horizontalOffset = targetPos.x - _centerPosition.x;
        ApplyDragTransform(horizontalOffset);

        OnDragOffset?.Invoke(horizontalOffset);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isInteractable || _state != CardState.Dragging) return;

        // Check which drop zone the card is in
        bool inLeft = IsInsideZone(cardRect, leftDropZone);
        bool inRight = IsInsideZone(cardRect, rightDropZone);

        if (inLeft)
            SnapTo(leftPlaceholder, isLeft: true);
        else if (inRight)
            SnapTo(rightPlaceholder, isLeft: false);
        else
            ReturnToCenter();
    }

    // ─────────────────────────────────────────────
    //  UPDATE (handles snap lerp)
    // ─────────────────────────────────────────────

    private void Update()
    {
        if (_state == CardState.Snapping || _state == CardState.Returning)
        {
            _snapElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_snapElapsed / _snapDur);
            float eased = EaseOutCubic(t);

            cardRect.anchoredPosition = Vector2.Lerp(_snapStartPos, _snapTargetPos, eased);
            cardRect.localRotation = Quaternion.Lerp(_snapStartRot, _snapTargetRot, eased);

            if (t >= 1f)
            {
                cardRect.anchoredPosition = _snapTargetPos;
                cardRect.localRotation = _snapTargetRot;

                if (_state == CardState.Snapping)
                {
                    _state = CardState.Snapped;
                    OnSnapped?.Invoke(_snappedLeft);
                }
                else // Returning
                {
                    _state = CardState.Idle;
                    OnReturnedToCenter?.Invoke();
                }
            }
        }
    }

    // ─────────────────────────────────────────────
    //  PRIVATE HELPERS
    // ─────────────────────────────────────────────

    private void ApplyDragTransform(float horizontalOffset)
    {
        bool goingRight = horizontalOffset > 0f;

        // Use the actual distance to the relevant placeholder as the lerp range,
        // so full rotation is reached exactly when the card hits the clamp.
        float maxDistance = goingRight
            ? Mathf.Abs((rightPlaceholder != null ? rightPlaceholder.anchoredPosition.x : _centerPosition.x + fullLerpDistance) - _centerPosition.x)
            : Mathf.Abs((leftPlaceholder != null ? leftPlaceholder.anchoredPosition.x : _centerPosition.x - fullLerpDistance) - _centerPosition.x);

        if (maxDistance <= 0.01f) maxDistance = fullLerpDistance;

        float t = Mathf.Clamp01(Mathf.Abs(horizontalOffset) / maxDistance);
        float targetRot = goingRight ? -maxRotation : maxRotation;

        cardRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, targetRot, t));
    }

    private void SnapTo(RectTransform placeholder, bool isLeft)
    {
        _state = CardState.Snapping;
        _snappedLeft = isLeft;

        _snapStartPos = cardRect.anchoredPosition;
        _snapStartRot = cardRect.localRotation;
        _snapTargetPos = placeholder.anchoredPosition;
        _snapTargetRot = placeholder.localRotation;
        _snapElapsed = 0f;
        _snapDur = snapDuration;

        // NOTE: stays interactable so player can re-grab the card and change
        // their choice before confirming. CardMode disables it explicitly
        // once "Pilih Aksi" is pressed via SetInteractable(false).
    }

    private void ReturnToCenter()
    {
        _state = CardState.Returning;

        _snapStartPos = cardRect.anchoredPosition;
        _snapStartRot = cardRect.localRotation;
        _snapTargetPos = _centerPosition;
        _snapTargetRot = Quaternion.identity;
        _snapElapsed = 0f;
        _snapDur = returnDuration;
    }

    private bool IsInsideZone(RectTransform card, RectTransform zone)
    {
        if (zone == null) return false;

        // Use world-space rects so this works correctly regardless of
        // card/zone having different parents or pivots.
        Rect cardWorldRect = GetWorldRect(card);
        Rect zoneWorldRect = GetWorldRect(zone);

        return zoneWorldRect.Overlaps(cardWorldRect);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        // corners[0] = bottom-left, corners[2] = top-right
        float xMin = corners[0].x, yMin = corners[0].y;
        float xMax = corners[2].x, yMax = corners[2].y;
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
}