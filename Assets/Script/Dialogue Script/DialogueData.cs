using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private bool isDialogueRead;
    [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();
    [SerializeField] private bool setRead;
    public List<DialogueNode> DialogueNodes {  get { return _dialogueNodes; } }
    public bool IsDialogueRead { get { return isDialogueRead; } set { isDialogueRead = value; } }
    public bool SetRead { get { return setRead; } set { setRead = value; } }
}
