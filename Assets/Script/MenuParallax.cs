using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [SerializeField] private float offsetMultiplier = 1f;
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 startPos;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        // 1. CEK KAMERA: Takutnya Camera.main lagi ilang/null
        if (Camera.main == null) return;

        // 2. CEK VELOCITY: Kalau isinya NaN, paksa reset ke nol
        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
        {
            velocity = Vector3.zero;
        }

        // 3. SAFETY SMOOTH TIME: Jangan sampe 0
        float safeSmoothTime = Mathf.Max(0.01f, smoothTime);

        // 4. HITUNG OFFSET
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // Cek juga hasil viewport-nya, jangan sampe NaN
        if (float.IsNaN(mousePos.x) || float.IsNaN(mousePos.y)) return;

        Vector3 offset = new Vector3(mousePos.x - 0.5f, mousePos.y - 0.5f, 0);
        Vector3 targetPos = startPos + (offset * offsetMultiplier);

        // 5. KALKULASI: Simpan hasil ke variabel sementara dulu sebelum apply
        Vector3 result = Vector3.SmoothDamp(
            transform.localPosition,
            targetPos,
            ref velocity,
            safeSmoothTime
        );

        // 6. FINAL CHECK: Baru apply kalau hasilnya valid
        if (!float.IsNaN(result.x) && !float.IsNaN(result.y))
        {
            transform.localPosition = result;
        }
    }
}