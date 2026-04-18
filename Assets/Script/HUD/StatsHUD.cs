using UnityEngine;
using UnityEngine.EventSystems;


// Discalimer ini dibuat oleh AI - hellnah


public class StatsHUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hudParent;
    public GameObject panelStat; // Drag UI pop-up lo ke sini
    public float displayDuration = 2.0f; // Rentan waktu 'x'

    private bool isHovering = false;
    private bool toggleByButton = false;
    private Coroutine hoverCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Cek apakah flag masih false sebelum nampilin UI
        if (!isHovering && !toggleByButton)
        {
            isHovering = true;
            hoverCoroutine = StartCoroutine(HandleUIPopup());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset flag jadi false pas keluar area
        if (!toggleByButton)
        {
            isHovering = false;
        }
    }

    // Inget yang dianimasiin tu si hudParent
    private System.Collections.IEnumerator HandleUIPopup()
    {
        panelStat.SetActive(true);
        // Logika Pop Up (bisa pake LeanTween/DOTween buat animasi)

        // Hold selama x detik
        yield return new WaitForSeconds(displayDuration);

        // Pop Out
        panelStat.SetActive(false);
    }

    public void ToggleByButton()
    {
        if (!toggleByButton)
        {
            isHovering = true;
            StopCoroutine(hoverCoroutine);
            StopAllCoroutines();
            panelStat.SetActive(true);
            toggleByButton = true;

            // set posisi langsung selesai
        }
        else
        {
            // animnasi pop out
            toggleByButton = false;

            panelStat.SetActive(false);
        }
    }
}