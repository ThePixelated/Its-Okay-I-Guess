using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData instance;

    [SerializeField] private int currentChapterIndex;
    [SerializeField] private int currentMainQuestIndex;

    public int CurrentChapterIndex { get { return currentChapterIndex; } }
    public int CurrentMainQuestIndex { get { return currentMainQuestIndex; } }

    private void Awake()
    {
        instance = this;
    }

    public void SetCurrentChapterIndex(int setValue) => currentChapterIndex = setValue;
    public void SetCurrentMainQuestIndex(int setValue) => currentMainQuestIndex = setValue;
    public void IncrementCurrentChapterIndex(int setValue) => currentChapterIndex += setValue;
    public void IncrementCurrentMainQuestIndex(int setValue) => currentMainQuestIndex += setValue;
}