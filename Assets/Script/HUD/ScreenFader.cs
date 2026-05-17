using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Memastikan objek ini punya CanvasGroup agar deteksi klik aman
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f; // Durasi animasi (detik)
    [SerializeField] private Color fadeColor = Color.black; // Warna transisi (biasanya hitam)

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        // Ambil referensi jika belum di-assign manual
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (fadeImage == null) fadeImage = GetComponent<Image>();

        // Set warna awal dan pastikan transparan saat start game
        fadeImage.color = fadeColor;
        SetFaderState(1f, true);
    }

    // Pemicu FADE IN (Layar menggelap / Alpha ke 1)
    public void StartFadeIn(float duration = 1f)
    {
        fadeDuration = duration;
        StartNewFade(1f, true); // Target Alpha 1, Blokir input
    }

    // Pemicu FADE OUT (Layar terang kembali / Alpha ke 0)
    public void StartFadeOut(float duration = 1f)
    {
        fadeDuration = duration;
        StartNewFade(0f, false); // Target Alpha 0, Jangan blokir input
    }

    private void StartNewFade(float targetAlpha, bool shouldBlockInput)
    {
        // 1. Stop animasi yang sedang berjalan agar tidak balapan
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);

        // 2. Tentukan apakah saat animasi jalan, input mouse harus diblokir atau tidak
        // Biasanya saat fade IN, input diblokir biar player ga klik apa-apa dulu.
        canvasGroup.blocksRaycasts = shouldBlockInput;

        // 3. Mulai animasi baru
        currentFadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    IEnumerator FadeRoutine(float target)
    {
        float startAlpha = canvasGroup.alpha; // Kita mainin alpha di CanvasGroup
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / fadeDuration;

            // Menggunakan SmoothStep agar transisinya halus (Ease In Out)
            float smoothT = Mathf.SmoothStep(0f, 1f, percent);

            // Lerp nilai alpha
            canvasGroup.alpha = Mathf.Lerp(startAlpha, target, smoothT);

            yield return null;
        }

        // Pastikan nilai akhir presisi
        canvasGroup.alpha = target;
        currentFadeCoroutine = null;

        // Opsional: Jika Fade Out kelar, pastikan interactable dimatikan total
        if (target == 0f)
        {
            canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.interactable = true;
        }

        fadeDuration = 1f;
    }

    // Helper untuk set status fader secara instan (tanpa animasi)
    public void SetFaderState(float alpha, bool blockInput)
    {
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = blockInput;
        canvasGroup.interactable = blockInput;
    }
}