using UnityEngine;
using System.Collections.Generic;

public class ActionLogger : MonoBehaviour
{
    public static ActionLogger Instance;
    public List<LogEntry> currentChapterLogs = new List<LogEntry>();

    private const string SAVE_KEY = "Game_Chapter_Logs"; // Nama kunci di PlayerPrefs

    public List<LogEntry> ChapterLogs { get { return currentChapterLogs; } }

    private void Awake()
    {
        Instance = this;
        LoadLogs(); // Otomatis load saat game mulai
    }

    public void AddLog(string title, string source, string action, CopingTag tag, string impact, bool significant, StatVariable statVariable)
    {
        if (!significant && string.IsNullOrEmpty(action)) return;

        LogEntry newEntry = new LogEntry(title, source, action, tag, impact, significant, statVariable);
        currentChapterLogs.Add(newEntry);

        // SETIAP nambah log, langsung save biar aman kalau browser crash
        SaveLogs();
    }

    // --- FUNGSI SAVE (Object -> JSON -> PlayerPrefs) ---
    public void SaveLogs()
    {
        LogWrapper wrapper = new LogWrapper();
        wrapper.logs = currentChapterLogs;

        // Ubah list jadi string JSON
        string json = JsonUtility.ToJson(wrapper);

        // Simpan string ke browser
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Log Berhasil Disimpan: " + json);
    }

    // --- FUNGSI LOAD (PlayerPrefs -> JSON -> Object) ---
    public void LoadLogs()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            LogWrapper wrapper = JsonUtility.FromJson<LogWrapper>(json);

            currentChapterLogs = wrapper.logs;
            Debug.Log("Log Berhasil Di-load!");
        }
    }

    public void ResetLogs()
    {
        currentChapterLogs.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY); // Hapus data di browser
    }
}