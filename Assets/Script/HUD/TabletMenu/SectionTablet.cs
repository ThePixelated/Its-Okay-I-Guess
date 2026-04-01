using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SectionTablet : MonoBehaviour
{
    [SerializeField] private Image sprite;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color onColor;

    [SerializeField] private RectTransform anchorIndicator;
    [SerializeField] private GameObject sectionPage;

    public RectTransform GetAnchorIndicator { get { return anchorIndicator; } }
    public GameObject SectionPage { get { return sectionPage; } }

    public void SetEnableUI()
    {
        sprite.color = onColor;

        if (sectionPage != null)
            sectionPage.SetActive(true);
    }

    public void SetDisableUI()
    {
        sprite.color = defaultColor;
        if (sectionPage != null)
            sectionPage.SetActive(false);
    }
}
