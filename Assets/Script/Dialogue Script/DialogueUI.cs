using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueManager m_dialogueManager;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI dialogueText;
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
        Debug.LogWarning("Dialog started...");
        Render(dialogueNode);
    }

    public void Render(DialogueNode dialogueNode)
    {
        dialogueText.text = dialogueNode.Text; // ini bisa dibuat efek "writing" kedepannya
        Debug.Log("Text: " + dialogueText.text);

        foreach (var choices in dialogueNode.Choices)
        {
            // Create button make prefab
            // addlistener

            Debug.Log("Btn: " + choices.text); // debug view
        }
    }

    
    public void CloseRender()
    {
        // animasi closing panel, dll
        Debug.LogWarning("Dialog has been closed...");
    }

    public void RemoveButtons()
    {
        foreach (var button in buttons)
        {
            // remove onclick() dulu paling
            Destroy(button);
        }
    }

    void NextNode(string nextNodeID)
    {
        m_dialogueManager.GoToNodeBtn(nextNodeID);
        RemoveButtons();
    }
}
