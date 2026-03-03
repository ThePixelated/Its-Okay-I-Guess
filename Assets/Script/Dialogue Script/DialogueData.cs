using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private Sprite currentSprite;
    //[SerializeField] private List<Sprite> normalSprite = new List<Sprite>();
    //[SerializeField] private List<Sprite> happySprite = new List<Sprite>();
    //[SerializeField] private List<Sprite> angrySprite = new List<Sprite>();
    //[SerializeField] private List<Sprite> sadSprite = new List<Sprite>();
    [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();
    public List<DialogueNode> DialogueNodes {  get { return _dialogueNodes; } }
    public Sprite CurrentSprite { get { return currentSprite; } private set { currentSprite = value; } }
    //public List<Sprite> NormalSprite {  get { return normalSprite; } }
    //public List<Sprite> HappySprite {  get { return happySprite; } }
    //public List<Sprite> AngrySprite {  get { return angrySprite; } }
    //public List<Sprite> SadSprite {  get { return sadSprite; } }

    public void SetCurrentSprite(Sprite newSprite)
    {
        CurrentSprite = newSprite;
    }
}
