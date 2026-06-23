using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Sends the FULL accumulated action log (all days, ~7 actions) to the Gemini
/// backend once — triggered at the end of the final day (ActData.triggersFinalSummary).
/// The resulting summary text is saved to PlayerPrefs so a separate Summary Screen
/// scene can read it back without needing AIManager or a live network call again.
/// </summary>
public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [Header("Server Config")]
    [SerializeField] private string serverUrl = "https://llm-ioig-production.up.railway.app";

    [Header("UI (optional — leave empty if this runs invisibly before scene transition)")]
    public TextMeshProUGUI outputText;

    private const string SUMMARY_SAVE_KEY = "Game_FinalSummary";

    /// <summary>Fired when the summary request finishes (success or fail), so PQM can proceed.</summary>
    public event Action<bool> OnSummaryRequestFinished; // bool = success

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    private class ChatRequest
    {
        public string systemPrompt;
        public string message;
    }

    [System.Serializable]
    private class ChatResponse
    {
        public string reply;
        public string error;
    }

    // ─────────────────────────────────────────────
    //  ENTRY POINT — called once, at end of final day
    // ─────────────────────────────────────────────
    public void RequestFinalSummary()
    {
        var logs = ActionLogger.Instance.AllLogs;

        string systemPrompt = BuildSystemPrompt();
        string userMessage = BuildUserMessage(logs);

        if (outputText != null)
            outputText.text = "Narator sedang berpikir...";

        StartCoroutine(SendToGemini(systemPrompt, userMessage));
    }

    private IEnumerator SendToGemini(string systemPrompt, string message)
    {
        var requestData = new ChatRequest { systemPrompt = systemPrompt, message = message };
        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(serverUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ChatResponse>(req.downloadHandler.text);

                if (!string.IsNullOrEmpty(response.error))
                {
                    Debug.LogError("Server error: " + response.error);
                    if (outputText != null) outputText.text = "Terjadi kesalahan dari server.";
                    OnSummaryRequestFinished?.Invoke(false);
                }
                else
                {
                    if (outputText != null) outputText.text = response.reply;

                    SaveFinalSummary(response.reply);
                    OnSummaryRequestFinished?.Invoke(true);
                }
            }
            else
            {
                Debug.LogError("Request gagal: " + req.error);
                Debug.LogError("Detail: " + req.downloadHandler.text);
                if (outputText != null) outputText.text = "Tidak bisa terhubung ke narator.";
                OnSummaryRequestFinished?.Invoke(false);
            }
        }
    }

    // ─────────────────────────────────────────────
    //  SUMMARY PERSISTENCE — read later by Summary Screen scene
    // ─────────────────────────────────────────────

    private void SaveFinalSummary(string summaryText)
    {
        PlayerPrefs.SetString(SUMMARY_SAVE_KEY, summaryText);
        PlayerPrefs.Save();
        Debug.Log("[AIManager] Final summary saved.");
    }

    /// <summary>Call this from the Summary Screen scene to display the saved result.</summary>
    public static string LoadFinalSummary()
    {
        return PlayerPrefs.HasKey(SUMMARY_SAVE_KEY) ? PlayerPrefs.GetString(SUMMARY_SAVE_KEY) : null;
    }

    // ─────────────────────────────────────────────
    //  PROMPT BUILDERS
    // ─────────────────────────────────────────────

    private string BuildSystemPrompt()
    {
        return
            "Kamu adalah sistem refleksi kesehatan mental dalam sebuah game edukasi untuk penelitian skripsi. " +
            "Tugasmu adalah merangkum pola pilihan pemain SELAMA 3 HARI bermain secara ringkas, jujur, dan tidak menghakimi. " +
            "\n\nFORMAT OUTPUT (wajib ikuti, gunakan label berikut):\n" +
            "Pola Hari Ini: [1 kalimat — gambaran umum kecenderungan coping yang terlihat]\n" +
            "Yang Sudah Baik: [1 kalimat — pilihan adaptif yang perlu diapresiasi]\n" +
            "Yang Perlu Diperhatikan: [1 kalimat — pilihan yang berpotensi kurang sehat jika dilakukan terus-menerus]\n" +
            "\nATURAN WAJIB:\n" +
            "- Bicara langsung ke 'Kamu', bukan 'pemain' atau 'dia'\n" +
            "- JANGAN buat narasi panjang atau puitis\n" +
            "- JANGAN diagnosis medis atau label psikologis klinis (misal: 'kamu mengalami depresi')\n" +
            "- JANGAN sebut nama gangguan mental apapun\n" +
            "- Gunakan bahasa Indonesia yang lugas, hangat, dan singkat\n" +
            "- Total output maksimal 4 kalimat";
    }

    private string BuildUserMessage(List<LogEntry> logs)
    {
        int adaptiveCount = 0;
        int maladaptiveCount = 0;

        string choiceList = "";
        foreach (var log in logs)
        {
            switch (log.tag)
            {
                case CopingTag.Adaptive: adaptiveCount++; break;
                case CopingTag.Maladaptive: maladaptiveCount++; break;
            }
            choiceList += $"- [{log.tag}] ({log.source}) Situasi: '{log.title}' → Pilihan: '{log.actionName}'\n";
        }

        string summary =
            $"Ringkasan pola dari {logs.Count} pilihan selama 3 hari: Adaptive={adaptiveCount}, Maladaptive={maladaptiveCount}\n\n" +
            $"Detail pilihan:\n{choiceList}";

        return summary + "\nBerikan output sesuai format yang ditentukan.";
    }
}