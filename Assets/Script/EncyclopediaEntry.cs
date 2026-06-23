using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single coping-mechanism entry shown in the Encyclopedia (Tablet Mode).
/// One entry exists per possible choice outcome — 7 Adaptive + 7 Maladaptive = 14 total.
/// Unlock status is NOT stored here (this is a shared design-time asset) —
/// see EncyclopediaProgress for the runtime/save-data unlock tracking.
/// </summary>
[CreateAssetMenu(fileName = "EncyclopediaEntry", menuName = "Scriptable Objects/EncyclopediaEntry")]
public class EncyclopediaEntry : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique stable ID — used as the save-data key. Do NOT rename after data has been saved with this ID; player progress would lose the link.")]
    public string entryID;

    [Header("Content")]
    public string title;          // nama coping mechanism (mis. "Problem-Focused Coping")
    [TextArea(1, 3)]
    public string subtitle;       // tagline pendek
    [TextArea(4, 12)]
    public string description;    // paragraf penjelasan

    [Header("Visual")]
    public Sprite icon;           // gambar besar (panel kiri atas)
    public Sprite slotThumbnail;  // icon kecil buat grid bawah — kalau kosong, fallback ke `icon`

    [Header("Classification")]
    public CopingTag tag;         // Adaptive / Maladaptive — menentukan tab mana
    [Range(1, 7)]
    public int slotIndex = 1;     // posisi di grid (1–7), selaras dengan urutan Act

    [Header("Default State")]
    [Tooltip("Jika true, entry ini terbuka dari awal tanpa perlu di-unlock lewat gameplay (mis. tutorial/freebie).")]
    public bool isUnlockedByDefault = false;

    public Sprite SlotThumbnailOrIcon => slotThumbnail != null ? slotThumbnail : icon;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(entryID))
            entryID = name;
    }
#endif
}

