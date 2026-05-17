using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private bool isIgnoredbyPQ = true;
    [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();

    public List<DialogueNode> DialogueNodes { get { return _dialogueNodes; } }
    public bool IsIgnoredbyPQ { get { return isIgnoredbyPQ; } private set { isIgnoredbyPQ = value; } }
}