using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStepUI : MonoBehaviour
{
    public Image stepImage;
    public TextMeshProUGUI stepDescriptionText;

    public void SetUp(TutorialStep tutorialStep)
    {
        stepImage.sprite = tutorialStep.stepImage;
        stepDescriptionText.text = tutorialStep.stepDescription;
    }
}
