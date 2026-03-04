using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string NodeID;
    public Sprite SrcImgSprite;
    public string CharName;
    [TextArea(3, 10)]
    public string Text;
    public List<Choices> Choices;
    public string NextNodeID;
}

[System.Serializable]
public class Choices
{
    public string text;
    public string NextNodeID;
}