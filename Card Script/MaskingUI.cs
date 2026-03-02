using UnityEngine;
using UnityEngine.UIElements;

public class MaskingUI : MonoBehaviour
{
    [SerializeField] private Transform cardPos;
    [SerializeField] private RectTransform firstChoiceMask;
    [SerializeField] private RectTransform secondChoiceMask;

    private void Start()
    {
        GameEvents.instance.onUIMaskTriggerStay += OnMasking;
        GameEvents.instance.onUIMaskTriggerExit += OnFinishedMask;
    }

    private void OnMasking(string tag)
    {
        RectTransform targetChoiceMask = null;

        if (tag == "leftSide")
            targetChoiceMask = firstChoiceMask;

        else if (tag == "rightSide")
            targetChoiceMask = secondChoiceMask;

        targetChoiceMask.sizeDelta = new Vector2(Mathf.Abs((630 / 2.428f) * cardPos.transform.position.x), targetChoiceMask.sizeDelta.y);
    }

    private void OnFinishedMask(string tag)
    {
        RectTransform targetChoiceMask = null;

        if (tag == "leftSide")
            targetChoiceMask = firstChoiceMask;

        else if (tag == "rightSide")
            targetChoiceMask = secondChoiceMask;

        targetChoiceMask.sizeDelta = new Vector2(0, targetChoiceMask.sizeDelta.y);
    }
}
