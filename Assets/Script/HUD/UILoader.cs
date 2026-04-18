using UnityEngine;
using UnityEngine.UI;

// Discalimer ini dibuat oleh AI - hellnah pt. 2

public class UILoader : MonoBehaviour
{
    public Transform parentContainer;

    // ini nanti dipanggil setiap quest added
    public void RefreshLayoutObj(GameObject targetGameObj = null)
    {
        //GameObject newUI = targetGameObj;
        //GameObject newUI = Instantiate(prefabUI, parentContainer);
        if (targetGameObj == null)
            targetGameObj = parentContainer.gameObject;
        
        RectTransform rt = targetGameObj.GetComponent<RectTransform>();

        // Panggil fungsi buat maksa rebuild layout
        RefreshLayout(rt);
    }

    public void RefreshLayout(RectTransform root)
    {
        // 1. Paksa Update semua canvas dulu
        Canvas.ForceUpdateCanvases();

        // 2. Ambil semua LayoutGroup dari anak-anak sampai ke root
        var components = root.GetComponentsInChildren<LayoutGroup>();

        // 3. Rebuild dari urutan paling bawah ke atas (reverse)
        for (int i = components.Length - 1; i >= 0; i--)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(components[i].GetComponent<RectTransform>());
        }

        // 4. Terakhir rebuild root-nya sendiri
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }
}