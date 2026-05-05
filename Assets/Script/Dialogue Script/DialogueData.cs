using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private bool isIgnoredbyPQ = true;
    [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();

    public List<DialogueNode> DialogueNodes { get { return _dialogueNodes; } }
    public bool IsIgnoredbyPQ { get { return isIgnoredbyPQ; } private set { isIgnoredbyPQ = value; } }

    public PMQuestInfo PMQuestInfo = new PMQuestInfo();
}

[System.Serializable]
public class PMQuestInfo
{
    public bool isTriggerQuest;
    public string targetQuest;
    public bool isTriggerDialogue;
    public string targetDialogueData;
    public int waitTransitionTime;
    public GameMode gameModeTransition;
}
