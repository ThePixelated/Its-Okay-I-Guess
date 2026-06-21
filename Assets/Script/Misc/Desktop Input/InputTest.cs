using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))] // Atau BoxCollider2D kalo pure 2D project
public class InputTest : MonoBehaviour
{
    public static InputTest instance;

    [Header("References")]
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Transform leftPlaceholder;
    [SerializeField] private Transform rightPlaceholder;
    // Origin bisa di-set otomatis pas Start, atau assign manual
    private Vector3 originPos;
    private Quaternion originRot;

    [Header("Settings")]
    [SerializeField] private float liftHeight = -1.71f;
    [SerializeField] private float liftSpeed = 10f; // Kecepatan naik
    [SerializeField] private float returnSpeed = 10f; // Kecepatan balik ke posisi/slide
    [SerializeField] private float snapDistance = 1.5f; // Jarak trigger snap

    // State Variables
    private Rigidbody rb;
    private bool isDragging = false;
    private float currentZ; // Variable buat nyimpen Z saat ini (buat animasi naik turun)
    private float lockedY;
    private Vector3 dragOffset;
    private float zDistanceToCamera;

    // Boundary X (Batas Kiri Kanan)
    private float minX, maxX;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Simpan posisi LOKAL relatif terhadap CardViewport

        originPos = transform.localPosition;
        originRot = transform.localRotation;

        // Set awal physics: Kinematic ON (Gak jatuh dulu)
        rb.isKinematic = true;
        rb.useGravity = false;

        // Setup Batas (Clamp) berdasarkan posisi placeholder
        // Asumsi placeholder kiri X-nya lebih kecil
        if (leftPlaceholder && rightPlaceholder)
        {
            minX = leftPlaceholder.position.x;
            maxX = rightPlaceholder.position.x;
        }
    }

    void OnMouseDown()
    {
        if (leftPlaceholder == null || rightPlaceholder == null) return;

        confirmBtn.interactable = false;

        isDragging = true;

        // 1. Reset Physics State (Mode Terbang)
        StopAllCoroutines(); // Stop kalo ada animasi drop yg belum kelar
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None; // Lepas constraint biar bisa rotasi script

        // 2. Kunci Y Axis & Setup Offset
        lockedY = transform.position.y;
        currentZ = transform.position.z; // Start Z dari posisi sekarang

        // Hitung jarak ke kamera berdasarkan target kedalaman (liftHeight)
        // Ini biar mouse drag akurat di kedalaman -1.71f
        Vector3 targetDepthPoint = new Vector3(transform.position.x, transform.position.y, liftHeight);
        zDistanceToCamera = Camera.main.WorldToScreenPoint(targetDepthPoint).z;

        dragOffset = transform.position - GetMouseWorldPos();

        // 3. Mulai Animasi Angkat Kartu (Z axis only)
        StartCoroutine(AnimateLift());
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        // 1. Dapatkan posisi mouse di dunia
        Vector3 worldMousePos = GetMouseWorldPos() + dragOffset;

        // 2. KONVERSI: Ubah dari posisi dunia ke posisi lokal relatif terhadap parent (CardViewport)
        Vector3 localMousePos = transform.parent.InverseTransformPoint(worldMousePos);

        // 3. Clamp X (Gunakan nilai lokal)
        float clampedX = Mathf.Clamp(localMousePos.x, minX, maxX);

        // 4. Update posisi LOKAL
        transform.localPosition = new Vector3(clampedX, lockedY, currentZ);

        HandleDynamicRotation(clampedX);
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Tentukan Target (Origin atau Snap ke Placeholder?)
        Transform targetTransform = GetTargetTransform();

        // Jalankan Sequence: Slide ke posisi -> Lalu Jatohin
        StartCoroutine(SlideAndDropSequence(targetTransform));
    }

    // --- LOGIC HELPERS ---

    // Menghitung rotasi berdasarkan posisi X relatif terhadap Origin vs Placeholder
    void HandleDynamicRotation(float currentX)
    {
        // 0 = Origin, 1 = Placeholder Kiri/Kanan
        float t = 0;
        Quaternion targetRot = originRot;

        if (currentX < originPos.x) // Lagi di kiri
        {
            // Hitung persentase jarak ke kiri (0 sampe 1)
            t = Mathf.InverseLerp(originPos.x, leftPlaceholder.position.x, currentX);
            targetRot = Quaternion.Lerp(originRot, leftPlaceholder.rotation, t);
        }
        else if (currentX > originPos.x) // Lagi di kanan
        {
            t = Mathf.InverseLerp(originPos.x, rightPlaceholder.position.x, currentX);
            targetRot = Quaternion.Lerp(originRot, rightPlaceholder.rotation, t);
        }

        // Apply rotasi
        transform.rotation = targetRot;
    }

    Transform GetTargetTransform()
    {
        // Cek jarak ke Left
        if (Vector3.Distance(transform.position, leftPlaceholder.position) <= snapDistance)
            return leftPlaceholder;

        // Cek jarak ke Right
        if (Vector3.Distance(transform.position, rightPlaceholder.position) <= snapDistance)
            return rightPlaceholder;

        // Kalau jauh semua, balik ke Origin (kita butuh bikin dummy transform buat origin data)
        // Hack dikit: Kita balikin null, nanti di coroutine kita handle logic originnya
        return null;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zDistanceToCamera;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // --- COROUTINES (ANIMATIONS) ---

    // Animasi Z naik pelan-pelan pas diklik
    IEnumerator AnimateLift()
    {
        while (isDragging && Mathf.Abs(currentZ - liftHeight) > 0.01f)
        {
            currentZ = Mathf.Lerp(currentZ, liftHeight, liftSpeed * Time.deltaTime);
            yield return null;
        }
        currentZ = liftHeight; // Pastiin pas
    }

    // Sequence: Geser Rapi -> Jatuh Physics
    IEnumerator SlideAndDropSequence(Transform target)
    {
        Vector3 targetPos;
        Quaternion targetRot;

        // Tentukan koordinat target
        if (target != null) // Snap ke Placeholder
        {
            targetPos = target.position; // Z-nya pasti 0 (asumsi placeholder nempel meja)
            targetRot = target.rotation;
        }
        else // Balik ke Origin
        {
            targetPos = originPos;
            targetRot = originRot;
        }

        // PHASE 1: SLIDE (ALIGNMENT)
        // Kita geser X dan Y nya aja, Z nya biarin tetep melayang (liftHeight) dulu
        // Biar kesannya dia geser di udara baru jatoh

        // Kita pake targetPos.x dan targetPos.y, tapi Z nya pake liftHeight
        Vector3 slideTarget = new Vector3(targetPos.x, targetPos.y, liftHeight);

        while (Vector3.Distance(transform.position, slideTarget) > 0.05f || Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            // Pindah posisi halus
            transform.position = Vector3.Lerp(transform.position, slideTarget, returnSpeed * Time.deltaTime);
            // Pindah rotasi halus
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, returnSpeed * Time.deltaTime);
            yield return null;
        }

        // Paksa presisi sebelum jatoh
        transform.position = slideTarget;
        transform.rotation = targetRot;

        confirmBtn.interactable = true;
        

        // PHASE 2: PHYSICS DROP (THE "BOUNCY" FEEL)

        // Kunci Rotasi biar jatohnya 'ceplek' (flat) gak ngegelinding
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;

        // Nyalain Gravity
        rb.isKinematic = false;
        rb.useGravity = true;

        // Selesai. Rigidbody akan handle sisanya (jatoh ke Z=0 mentok collider).
        // Nanti pas kartu mau dipickup lagi, logic OnMouseDown akan ngereset ini semua.
    }

    public IEnumerator SlideOutAnimation()
    {
        yield return null;
    }

    public IEnumerator SlideInAnimation()
    {
        transform.position = originPos;
        transform.rotation = originRot;

        yield return null;
    }
}