using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueManager m_dialogueManager;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choicesPanel;
    public GameObject stopDialouePanel;
    public GameObject namePanel;
    [SerializeField] private GameObject imgCharacter;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();

    [Header("Typing Effect")]
    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private float commaDelay = 0.3f;
    [SerializeField] private float periodDelay = 0.55f;

    [Header("Debug mode")]
    [SerializeField] private string brtueNodeID = "";
    [SerializeField] private bool btnFlag = false;

    private Coroutine typingCoroutine;
    public bool IsTyping { get; private set; } = false;

    private void Awake()
    {
        m_dialogueManager = gameObject.GetComponent<DialogueManager>();
    }

    private void Update()
    {
        if (btnFlag)
        {
            btnFlag = false;
            NextNode(brtueNodeID);
            brtueNodeID = "";
        }
    }

    public void StartRender(DialogueNode dialogueNode)
    {
        dialogueBox.SetActive(true);
        Debug.LogWarning("Dialog started...");
        Render(dialogueNode);
    }

    public void Render(DialogueNode dialogueNode)
    {
        // Nama & gambar karakter
        nameText.text = dialogueNode.CharName;
        namePanel.SetActive(!string.IsNullOrWhiteSpace(dialogueNode.CharName));

        if (dialogueNode.SrcImgSprite != null)
        {
            imgCharacter.SetActive(true);
            imgCharacter.GetComponent<Image>().sprite = dialogueNode.SrcImgSprite;
        }
        else
        {
            imgCharacter.SetActive(false);
        }

        // Sembunyiin choices dulu, akan dimunculkan setelah typing selesai
        choicesPanel.SetActive(false);
        RemoveButtons();

        // Mulai typing effect
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialogueNode.Text, dialogueNode.Choices));
    }

    // Dipanggil dari DialogueManager ketika player pencet skip saat masih typing
    public void SkipTyping(string fullText, List<Choices> choices)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = null;

        dialogueText.text = fullText;
        IsTyping = false;

        FinishRender(choices);
    }

    private IEnumerator TypeText(string fullText, List<Choices> choices)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;

            float delay = charDelay;
            if (c == ',') delay = commaDelay;
            else if (c == '.' || c == '!' || c == '?') delay = periodDelay;
            SoundEffectManager.Play("TypingEffect", true);

            yield return new WaitForSeconds(delay);
        }

        IsTyping = false;
        typingCoroutine = null;
        FinishRender(choices);
    }

    // Dipanggil setelah typing selesai (baik natural maupun di-skip)
    private void FinishRender(List<Choices> choices)
    {
        if (choices != null && choices.Count >= 1)
        {
            choicesPanel.SetActive(true);
            SpawnChoiceButtons(choices);
        }
    }

    private void SpawnChoiceButtons(List<Choices> choices)
    {
        foreach (var choice in choices)
        {
            GameObject tempBtn = Instantiate(buttonPrefab);
            tempBtn.transform.SetParent(buttonParent);

            TextMeshProUGUI btnText = tempBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            btnText.text = choice.Text;

            Button btnListener = tempBtn.GetComponent<Button>();
            btnListener.onClick.AddListener(() => NextNode(choice.NextNodeID));

            if (choice.IsTriggerQuest)
            {
                if (choice.TargetQuestID.StartsWith("SQ_", System.StringComparison.OrdinalIgnoreCase))
                    btnListener.onClick.AddListener(() => QuestManager.Instance.AddQuest(choice.TargetQuestID));
                else if (choice.TargetQuestID.StartsWith("MQ_", System.StringComparison.OrdinalIgnoreCase))
                    btnListener.onClick.AddListener(() => QuestManager.Instance.ChangeToCardGame());
            }

            buttons.Add(tempBtn);
        }
    }

    public void CloseRender()
    {
        Debug.LogWarning("Dialog has been closed...");

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        IsTyping = false;

        RemoveButtons();
        dialogueBox.SetActive(false);
        choicesPanel.SetActive(false);

        m_dialogueManager.DialogueStopped();
    }

    public void RemoveButtons()
    {
        foreach (var button in buttons)
            Destroy(button);
        buttons.Clear();
    }

    void NextNode(string nextNodeID)
    {
        m_dialogueManager.GoToNodeBtn(nextNodeID);
        RemoveButtons();
    }

    public void PQDialogueValidation(DialogueData dialogueData)
    {
        if (dialogueData.name.StartsWith("PQ_", System.StringComparison.OrdinalIgnoreCase))
        {
            SetStopDialoguePanel(false);
            Debug.Log("Stop Dialogue Button disabled!!!");
        }
        else
            SetStopDialoguePanel(true);
    }

    public void SetStopDialoguePanel(bool state)
    {
        stopDialouePanel.SetActive(state);
    }
}