using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UISystem.Entries;
using UnityEngine;

public class TutorialMenu : BaseMenu<TutorialMenu>
{
    [Header("Game Tutorial")]
    [SerializeField] private Transform content;

    [Header("UI Tutorial")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private TutorialVisualData tutorialVisual;
    [SerializeField] private int currentStepIndex = 0;
    JuicerRuntime openEffectBG;
    JuicerRuntime closeEffectBG;

    public override void OnCreated()
    {
        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
        closeEffectBG.SetOnCompleted(CloseMenu);
    }


    public override void OnOpened()
    {
        openEffectBG.Start();
    }

    public override void OnClosed()
    {
        closeEffectBG.Start();
    }

    public override void ResetMenu()
    {

    }

    public void SetTutorialDataToDisplay(TutorialVisualData tutorialVisualData, Action OnDisplayed = null)
    {
        currentStepIndex = 0;
        tutorialVisual = tutorialVisualData;
        titleText.text = tutorialVisualData.Title;
        descriptionText.text = tutorialVisualData.TutorialSteps[currentStepIndex].stepDescription;
        OnDisplayed?.Invoke();
    }

    public void NextVisualStep()
    {
        currentStepIndex++;
        if (currentStepIndex >= tutorialVisual.TutorialSteps.Count)
        {
            return;
        }
        descriptionText.text = tutorialVisual.TutorialSteps[currentStepIndex].stepDescription;
    }

    public void DisplayMessage(string info)
    {
        titleText.text = "";
        descriptionText.text = info;
    }
}
