using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AIManager : MonoBehaviour
{
    [Header("Server Config")]
    [SerializeField] private string serverUrl = "https://ISI-URL-RAILWAY-KAMU.up.railway.app/chat";

    [Header("UI")]
    public TextMeshProUGUI outputText;

    // ─────────────────────────────────────────────
    // Data class untuk JSON request & response
    // ─────────────────────────────────────────────
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
    // Entry point — dipanggil dari luar (tombol, event, dll)
    // ─────────────────────────────────────────────
    public void RequestSummary()
    {
        var logs = ActionLogger.Instance.currentChapterLogs;

        // Pisahkan system prompt dan user message
        string systemPrompt = BuildSystemPrompt();
        string userMessage = BuildUserMessage(logs);

        outputText.text = "Narator sedang berpikir...";
        StartCoroutine(SendToGemini(systemPrompt, userMessage));
    }

    // ─────────────────────────────────────────────
    // Coroutine: kirim request ke server Railway
    // ─────────────────────────────────────────────
    private IEnumerator SendToGemini(string systemPrompt, string message)
    {
        // Buat JSON body
        var requestData = new ChatRequest
        {
            systemPrompt = systemPrompt,
            message = message
        };
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
                    outputText.text = "Terjadi kesalahan dari server.";
                    Debug.LogError("Server error: " + response.error);
                }
                else
                {
                    outputText.text = response.reply;
                    ActionLogger.Instance.ResetLogs(); // Reset setelah berhasil
                }
            }
            else
            {
                outputText.text = "Tidak bisa terhubung ke narator.";
                Debug.LogError("Request gagal: " + req.error);
                Debug.LogError("Detail: " + req.downloadHandler.text);
            }
        }
    }

    // ─────────────────────────────────────────────
    // Prompt Builders
    // ─────────────────────────────────────────────
    private string BuildSystemPrompt()
    {
        return
            "Kamu adalah sistem refleksi kesehatan mental dalam sebuah game edukasi untuk penelitian skripsi. " +
            "Tugasmu adalah merangkum pola pilihan pemain secara ringkas, jujur, dan tidak menghakimi. " +
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
        // Hitung jumlah tiap tag untuk kasih konteks pola ke AI
        int adaptiveCount = 0;
        int maladaptiveCount = 0;
        int avoidanceCount = 0;

        string choiceList = "";
        foreach (var log in logs)
        {
            switch (log.tag)
            {
                case CopingTag.Adaptive: adaptiveCount++; break;
                case CopingTag.Maladaptive: maladaptiveCount++; break;
                case CopingTag.Avoidance: avoidanceCount++; break;
            }
            choiceList += $"- [{log.tag}] Situasi: '{log.title}' → Pilihan: '{log.actionName}'\n";
        }

        string summary =
            $"Ringkasan pola: Adaptive={adaptiveCount}, Maladaptive={maladaptiveCount}, Avoidance={avoidanceCount}\n\n" +
            $"Detail pilihan:\n{choiceList}";

        return summary + "\nBerikan output sesuai format yang ditentukan.";
    }

    // ─────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────
    private string TranslateTag(CopingTag tag)
    {
        switch (tag)
        {
            case CopingTag.Adaptive: return "seperti usaha yang baik untuk bertahan";
            case CopingTag.Maladaptive: return "sedikit memaksakan diri";
            case CopingTag.Avoidance: return "seperti cara untuk melarikan diri sejenak";
            default: return "sebuah pilihan";
        }
    }
}
