using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Config")]
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TextMeshProUGUI statementTxt;
    [SerializeField] private TextMeshProUGUI firstChoiceTxt;
    [SerializeField] private TextMeshProUGUI secondChoiceTxt;
    [SerializeField] private Slider moneySlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider socialSlider;


    [Header("Logic Component")]
    [SerializeField] private DataChapter dataChapter;
    [SerializeField] private int indexChapter = 0;
    [SerializeField] private int indexCurrentChaptSect = 0;

    private ChaptSection _currentChaptSect;
    private bool actionFlag = true;
    //private Coroutine 

    private void Start()
    {
        _currentChaptSect = dataChapter.ChapterSections[indexCurrentChaptSect];

        ConfirmLogicBtn();

        UIManager.Instance.fadeImage.StartFadeOut(6);
    }

    private string isClick = "input idle..";
    public void ConfirmLogicBtn()
    {
        clickCor = StartCoroutine(ClickStatus());

        if (actionFlag)
        {
            statementTxt.text = _currentChaptSect.actionSection.statement;
            firstChoiceTxt.text = _currentChaptSect.actionSection.firstChoice;
            secondChoiceTxt.text = _currentChaptSect.actionSection.secondChoice;

            actionFlag = false;
        }

        else
        {
            indexCurrentChaptSect++;
            _currentChaptSect = dataChapter.ChapterSections[indexCurrentChaptSect];

            statementTxt.text = "...";
            firstChoiceTxt.text = _currentChaptSect.consSection[0].firstChoice;
            secondChoiceTxt.text = _currentChaptSect.consSection[0].secondChoice;

            actionFlag = true;
        }

        // calculation

        // update ui
    }

    private Coroutine clickCor;
    IEnumerator ClickStatus()
    {
        isClick = "Button confimer is clicked!";

        yield return new WaitForSeconds(1);

        isClick = "input idle..";

        StopCoroutine(clickCor);
        clickCor = null;
    }
}
