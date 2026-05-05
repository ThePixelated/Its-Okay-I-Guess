using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject cameraTopDown;
    [SerializeField] private GameObject cameraCard;
    [SerializeField] private GameObject cardViewport;

    [Header("Anchor Card Camera")]
    [SerializeField] private Transform offPos;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform finalPos;

    [Header("Backdrop BG Settings")]
    [SerializeField] private SpriteRenderer backdropRenderer; // Drag gObj backdrop ke sini
    // Nilai target alpha (154/255 = ~0.603)
    private const float TargetAlpha = 154f / 255f;

    [SerializeField] private float transitionSpeed = 2f;

    private bool isTransitioning = false;
    public bool CameraCardIsLocked { get; set; } = false;

    private void Awake()
    {
        Instance = this;

        // Inisialisasi backdrop agar transparant di awal
        if (backdropRenderer != null)
        {
            SetBackdropAlpha(0f);
            backdropRenderer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isTransitioning)
        {
            cameraTopDown.transform.position = new Vector3(player.position.x, player.position.y, cameraTopDown.transform.position.z);

            if (CameraCardIsLocked)
            {
                cameraCard.transform.position = new Vector3(cameraTopDown.transform.position.x, cameraTopDown.transform.position.y, cameraCard.transform.position.z);
            }
        }
    }

    public void ChangeActiveCamera(bool toCard)
    {
        cameraCard.transform.position = new Vector3(cameraTopDown.transform.position.x, cameraTopDown.transform.position.y, cameraCard.transform.position.z);

        StopAllCoroutines();
        if (toCard)
        {
            StartCoroutine(TransitionToCard());
        }
        else
        {
            StartCoroutine(TransitionToTopDown());
        }
    }

    IEnumerator TransitionToCard()
    {
        isTransitioning = true;
        cameraCard.SetActive(true);
        cameraTopDown.SetActive(false);

        if (backdropRenderer != null)
        {
            backdropRenderer.gameObject.SetActive(true);
            // Kita mulai fading paralel (tanpa yield return) agar jalan bareng camera
            // Kecepatan fade disesuaikan agar sync dengan total durasi 2 tahap kamera
            StartCoroutine(FadeRoutine(backdropRenderer, 0f, TargetAlpha, transitionSpeed / 2f));
        }

        // Tahap 1: Off -> Start
        yield return StartCoroutine(MoveRoutine(cardViewport.transform, offPos.position, startPos.position, transitionSpeed));

        yield return new WaitForSeconds(1f);

        // Tahap 2: Start -> Final
        yield return StartCoroutine(MoveRoutine(cardViewport.transform, startPos.position, finalPos.position, transitionSpeed));

        isTransitioning = false;
        CameraCardIsLocked = true;
    }

    IEnumerator TransitionToTopDown()
    {
        isTransitioning = true;
        CameraCardIsLocked = false;

        if (backdropRenderer != null)
        {
            // Mulai fade out paralel
            StartCoroutine(FadeRoutine(backdropRenderer, TargetAlpha, 0f, transitionSpeed / 2f));
        }

        // Balik: Final -> Start
        yield return StartCoroutine(MoveRoutine(cardViewport.transform, finalPos.position, startPos.position, transitionSpeed));

        yield return new WaitForSeconds(1f);

        // Balik: Start -> Off
        yield return StartCoroutine(MoveRoutine(cardViewport.transform, startPos.position, offPos.position, transitionSpeed));

        cameraCard.SetActive(false);
        cameraTopDown.SetActive(true);

        if (backdropRenderer != null)
        {
            backdropRenderer.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }

    // Fungsi pembantu buat Gerakan (Ease In Out)
    IEnumerator MoveRoutine(Transform target, Vector3 from, Vector3 to, float speed)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            target.position = Vector3.Lerp(from, to, smoothT);
            yield return null;
        }
        target.position = to;
    }

    // Fungsi pembantu buat Fading (Ease In Out)
    IEnumerator FadeRoutine(SpriteRenderer renderer, float fromAlpha, float toAlpha, float speed)
    {
        if (renderer == null) yield break;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            SetBackdropAlpha(Mathf.Lerp(fromAlpha, toAlpha, smoothT));
            yield return null;
        }
        SetBackdropAlpha(toAlpha);
    }

    // Helper untuk ganti alpha tanpa ngetik panjang
    private void SetBackdropAlpha(float alpha)
    {
        if (backdropRenderer != null)
        {
            Color c = backdropRenderer.color;
            c.a = alpha;
            backdropRenderer.color = c;
        }
    }
}