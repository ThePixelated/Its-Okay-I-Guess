using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataChapter", menuName = "Scriptable Objects/DataChapter")]
public class DataChapter : ScriptableObject
{
    public List<ChaptSection> ChapterSections = new List<ChaptSection>();
}

[System.Serializable]
public class ChaptSection
{
    public string Title;
    public int ID = 1;
    public ActionChapt actionSection = new ActionChapt();
    public List<Consequences> consSection = new List<Consequences>(2);

    public ChaptSection()
    {
        actionSection.ID = ID;
    }
}


[System.Serializable]
public class ActionChapt
{
    public int ID;
    public SubChaptType actionType;
    //public GameObject ;
    [TextArea(3, 10)]
    public string statement;
    [TextArea(3, 10)]
    public string firstChoice;
    [TextArea(3, 10)]
    public string secondChoice;
    public StatVariable statEffect;
    public BranchState stateDependent;
    public int targetOriginIDBranch;
    public int targetIDBranch;
    public float weightEffect;

    public ActionChapt()
    {
        actionType = SubChaptType.ActionChapt;
        stateDependent = BranchState.Stable;
        targetOriginIDBranch = -1;
        targetIDBranch = -1;
        weightEffect = 1;
    }

    public void CalculateWeight()
    {
        statEffect.Money *= weightEffect;
        statEffect.Health *= weightEffect;
        statEffect.Energy *= weightEffect;
        statEffect.Social *= weightEffect;
    }

    public void SetTargetActionWeight(ChaptSection dataChapt, int targetIDBranch_ = -1)
    {
        if (stateDependent != BranchState.Stable)
        {
            ActionChapt targetedAction = dataChapt.actionSection;
            targetedAction.targetOriginIDBranch = ID;

            if (stateDependent == BranchState.Continues)
            {
                targetedAction.targetIDBranch = targetIDBranch_;
                targetedAction.stateDependent = BranchState.Continues;
            }

            if (stateDependent == BranchState.Closed)
            {
                targetedAction.stateDependent = BranchState.Closed;
            }

            targetedAction.weightEffect = weightEffect;
            targetedAction.CalculateWeight();
        }

        else Debug.LogWarning("SetTargetActionWeight() is being called, but Branch state is Stable!");
    }
}

[System.Serializable]
public class Consequences
{
    public string Title;
    public SubChaptType consType;
    //public string statement;
    [TextArea(3, 10)]
    public string firstChoice;
    [TextArea(3, 10)]
    public string secondChoice;
    //public StatVariable statEffect;
    //public bool isBranching;
    //public int targetIDBranch;
    //public float weightEffect;

    public Consequences()
    {
        consType = SubChaptType.Consequences;
    }
}

[System.Serializable]
public class StatVariable
{
    public float Money;
    public float Health;
    public float Energy;
    public float Social;
}

public enum SubChaptType
{
    Null,
    ActionChapt,
    Consequences
}

public enum BranchState
{
    Stable,
    Trigger,
    Continues,
    Closed
}