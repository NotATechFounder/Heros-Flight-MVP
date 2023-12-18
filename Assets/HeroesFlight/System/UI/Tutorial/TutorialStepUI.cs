using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStepUI : MonoBehaviour
{
    public TextMeshProUGUI stepNameText;
    public Image stepImage;
    public TextMeshProUGUI stepDescriptionText;

    public void SetUp(string header, TutorialStep tutorialStep)
    {
        stepNameText.text = tutorialStep.stepName;
        stepImage.sprite = tutorialStep.stepImage;
        stepDescriptionText.text = tutorialStep.stepDescription;
    }
}
