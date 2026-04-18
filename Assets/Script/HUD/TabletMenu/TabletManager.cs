using System.Collections.Generic;
using UnityEngine;

public class TabletManager : MonoBehaviour
{
    public static TabletManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private GameObject tabletViewport;
    [SerializeField] private RectTransform indicatorUI; 
    [SerializeField] private int currentSectionIndex;
    [SerializeField] private int resumeIndex; // OII PENTING! hard coded index 5 Resume index
    [SerializeField] private List<GameObject> section = new List<GameObject>();

    public int ResumeIndex { get { return resumeIndex; } }
    public bool IsResumeBtnPressed { get; set; } = false;

    public void ChangeSection(int targetIndexSection = 0)
    {
        SectionTablet currSectPlaceholder = section[currentSectionIndex].GetComponent<SectionTablet>();
        currSectPlaceholder.SetDisableUI();

        SectionTablet targetSection = section[targetIndexSection].GetComponent<SectionTablet>();
        targetSection.SetEnableUI();

        indicatorUI.position = targetSection.GetAnchorIndicator.position;
        currentSectionIndex = targetIndexSection;
    }

    public void HandleSectionIndex(int direction)
    {
        int tempcurrentSectionIndex = currentSectionIndex;
        tempcurrentSectionIndex += direction;
        if (tempcurrentSectionIndex >= section.Count)
            tempcurrentSectionIndex = section.Count - 1;

        if (tempcurrentSectionIndex <= -1)
            tempcurrentSectionIndex = 0;

        ChangeSection(tempcurrentSectionIndex);
    }

    public void InnitTablet()
    {
        tabletViewport.SetActive(true);

        if (currentSectionIndex == resumeIndex)
            currentSectionIndex = 0;
        
        ChangeSection(currentSectionIndex);
    }

    public void TerminateTablet()
    {
        tabletViewport.SetActive(false);

        if (currentSectionIndex == resumeIndex)
            currentSectionIndex = 0;

        ChangeSection(currentSectionIndex);
    }

    public int GetSectionLenght()
    {
        return section.Count;
    }

    public int GetCurrentSectionIndex()
    {
        return currentSectionIndex;
    }

    public void ResumeBtnPressed()
    {
        IsResumeBtnPressed = true;
    }
}
