using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UISystem.Entries;
using UnityEngine;

public class TutorialMenu : BaseMenu<InventoryMenu>
{
    public event Action OnContinueButtonClicked;

    [SerializeField] private Transform tutorialStepParent;
    [SerializeField] private TutorialStepUI tutorialStepUIPrefab;

    [SerializeField] private AdvanceButton advanceButton;

    JuicerRuntime openEffectBG;
    JuicerRuntime closeEffectBG;

    private List<TutorialStepUI> tutorialStepUIs = new List<TutorialStepUI>();

    public override void OnCreated()
    {
        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
        closeEffectBG.SetOnCompleted(CloseMenu);

        advanceButton.onClick.AddListener(CloseButtonAction);
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

    public void Display (TutorialStep[] tutorialSteps)
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            TutorialStepUI tutorialStepUI = ObjectPoolManager.SpawnObject (tutorialStepUIPrefab, tutorialStepParent);
            tutorialStepUI.SetUp(i.ToString(), tutorialSteps[i]);
            tutorialStepUIs.Add(tutorialStepUI);
        }
    }

    private void CloseButtonAction()
    {
        OnContinueButtonClicked?.Invoke();
        Close();
    }
}
