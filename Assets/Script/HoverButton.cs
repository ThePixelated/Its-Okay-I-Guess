using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Test - Enter Button: {eventData.pointerEnter.name}");
        // Cek apakah yang di-hover adalah object dengan tag "Button" atau punya komponen Button
        if (eventData.pointerEnter != null)
        {
            UIManager.Instance.hoveredButton = eventData.pointerEnter;
            UIManager.Instance.UpdateText();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.hoveredButton = null;
        UIManager.Instance.ClearText();
    }
}
