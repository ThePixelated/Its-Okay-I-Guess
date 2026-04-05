using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueManager m_dialogueManager;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject imgCharacter;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();

    [Header("Debug mode")]
    [SerializeField] private string brtueNodeID = "";
    [SerializeField] private bool btnFlag = false;

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
        // animasi panel open up, dll
        dialogueBox.SetActive(true);
        Debug.LogWarning("Dialog started...");
        Render(dialogueNode);
    }

    public void Render(DialogueNode dialogueNode)
    {
        nameText.text = dialogueNode.CharName;
        dialogueText.text = dialogueNode.Text; // ini bisa dibuat efek "writing" kedepannya

        if (dialogueNode.SrcImgSprite != null)
        {
            imgCharacter.SetActive(true);
            Image img = imgCharacter.GetComponent<Image>();
            img.sprite = dialogueNode.SrcImgSprite;
        }
        else
        {
            imgCharacter.SetActive(false);
        }

        //Debug.Log("Text: " + dialogueText.text);

        //RemoveButtons();
        if (dialogueNode.Choices.Count >= 1)
            choicesPanel.SetActive(true);
        else
            choicesPanel.SetActive(false);

        foreach (var choices in dialogueNode.Choices)
        {
            GameObject tempBtn = Instantiate(buttonPrefab);
            tempBtn.transform.SetParent(buttonParent);
            // addlistener

            TextMeshProUGUI btnText = tempBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            btnText.text = choices.Text;

            Button btnListener = tempBtn.GetComponent<Button>();
            btnListener.onClick.AddListener(() => NextNode(choices.NextNodeID));

            if (choices.IsTriggerQuest)
            {
                btnListener.onClick.AddListener(() => QuestManager.Instance.AddQuest(choices.TargetQuestID));
            }

            buttons.Add(tempBtn);

            //Debug.Log("Btn: " + choices.text); // debug view
        }
    }

    
    public void CloseRender()
    {
        // animasi closing panel, dll
        Debug.LogWarning("Dialog has been closed...");
        RemoveButtons();

        dialogueBox.SetActive(false);
        choicesPanel.SetActive(false);

        m_dialogueManager.DialogueStopped();
    }

    public void RemoveButtons()
    {
        foreach (var button in buttons)
        {
            // remove onclick() dulu paling
            Destroy(button);
        }
        buttons.Clear();
    }

    void NextNode(string nextNodeID)
    {
        m_dialogueManager.GoToNodeBtn(nextNodeID);
        RemoveButtons();
    }
}
