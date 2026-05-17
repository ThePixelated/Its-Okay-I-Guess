using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


// Discalimer ini dibuat oleh AI - hellnah


public class StatsHUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform hudParent;
    //public GameObject panelStat; // Drag UI pop-up lo ke sini
    [SerializeField] private Vector2 hiddenPos; // Posisi saat sembunyi (misal Y = 500)
    [SerializeField] private Vector2 shownPos;
    public float displayDuration = 2.0f; // Rentan waktu 'x'
    [SerializeField] private float transitionDuration = 0.5f;

    private bool isHovering = false;
    private bool toggleByButton = false;
    private Coroutine hoverCoroutine;
    private Coroutine activeCoroutine;

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
    private IEnumerator HandleUIPopup()
    {
        //panelStat.SetActive(true);
        // Logika Pop Up (bisa pake LeanTween/DOTween buat animasi)
        activeCoroutine = StartCoroutine(SlideRoutine(shownPos));

        // Hold selama x detik
        yield return new WaitForSeconds(displayDuration);

        activeCoroutine = StartCoroutine(SlideRoutine(hiddenPos));
        // Pop Out
        //panelStat.SetActive(false);
    }

    public void ToggleByButton()
    {
        if (!toggleByButton)
        {
            isHovering = true;
            StopCoroutine(hoverCoroutine);
            StopAllCoroutines();
            //panelStat.SetActive(true);
            activeCoroutine = StartCoroutine(SlideRoutine(shownPos));
            toggleByButton = true;

            // set posisi langsung selesai
        }
        else
        {
            // animnasi pop out
            toggleByButton = false;
            activeCoroutine = StartCoroutine(SlideRoutine(hiddenPos));

            //panelStat.SetActive(false);
        }
    }

    IEnumerator SlideRoutine(Vector2 target)
    {
        Vector2 startPos = hudParent.anchoredPosition;
        float elapsed = 0;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / transitionDuration;

            // Menggunakan SmoothStep agar ada efek perlambatan (Ease Out)
            float curve = Mathf.SmoothStep(0, 1, percent);

            hudParent.anchoredPosition = Vector2.Lerp(startPos, target, curve);
            yield return null;
        }

        hudParent.anchoredPosition = target;
        StopCoroutine(activeCoroutine);
        activeCoroutine = null;
    }
}