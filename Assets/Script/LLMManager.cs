using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;

public class LLMManager : MonoBehaviour
{
    // Ganti dengan URL Railway kamu nanti
    private string serverUrl = "https://my-game-server-production.up.railway.app/chat";

    public TMP_InputField inputField;   // Input dari player
    public TMP_Text responseText;       // Tampilkan jawaban
    public GameObject loadingIndicator; // Opsional, loading spinner

    [System.Serializable]
    class RequestData { public string message; }

    [System.Serializable]
    class ResponseData { public string reply; }

    public void OnSendClicked()
    {
        string userMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;
        StartCoroutine(AskGemini(userMessage));
    }

    IEnumerator AskGemini(string message)
    {
        if (loadingIndicator) loadingIndicator.SetActive(true);
        responseText.text = "...";

        // Buat JSON body
        string json = JsonUtility.ToJson(new RequestData { message = message });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(serverUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var data = JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);
                responseText.text = data.reply;
            }
            else
            {
                responseText.text = "Error: " + req.error;
                Debug.LogError(req.downloadHandler.text);
            }
        }

        if (loadingIndicator) loadingIndicator.SetActive(false);
    }
}