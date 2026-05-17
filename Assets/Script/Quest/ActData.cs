using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActData", menuName = "Scriptable Objects/ActData")]
public class ActData : ScriptableObject
{
    public List<PQStep> steps = new List<PQStep>();
    public ActData nextAct; // null = game selesai / end of story
}

[System.Serializable]
public class PQStep
{
    public PQStepType type;
    public DialogueData dialogueData; // diisi kalau type = Dialogue
    public string questId;            // diisi kalau type = Quest
    public float waitTransitionTime;
}

public enum PQStepType { Dialogue, Quest }