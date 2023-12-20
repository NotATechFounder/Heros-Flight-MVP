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
    private Action OnStartedClicked;
    public Action OnShowClicked;
    public Action OnHideClicked;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform tutorialStepParent;
    [SerializeField] private TutorialStepUI tutorialStepUIPrefab;
    [SerializeField] private Transform content;

    [Header("Buttons")]
    [SerializeField] private AdvanceButton hideButton;
    [SerializeField] private AdvanceButton showButton;
    [SerializeField] private AdvanceButton startButton;

    JuicerRuntime openEffectBG;
    JuicerRuntime closeEffectBG;

    private List<TutorialStepUI> tutorialStepUIs = new List<TutorialStepUI>();

    public override void OnCreated()
    {
        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
        closeEffectBG.SetOnCompleted(CloseMenu);

        hideButton.onClick.AddListener(HideClicked);
        showButton.onClick.AddListener(ShowClicked);
        startButton.onClick.AddListener(StartedClicked);
    }

    private void HideClicked()
    {
        OnHideClicked?.Invoke();
        content.gameObject.SetActive(false);
    }

    private void ShowClicked()
    {
        OnShowClicked?.Invoke();
        content.gameObject.SetActive(true);
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

    public void Display (TutorialVisualData tutorialVisualData, Action OnStarted = null)
    {
        foreach (var item in tutorialStepUIs)
        {
            ObjectPoolManager.ReleaseObject(item);
        }

        tutorialStepUIs.Clear();

        titleText.text = tutorialVisualData.Title;

        for (int i = 0; i < tutorialVisualData.TutorialSteps.Count; i++)
        {
            TutorialStepUI tutorialStepUI = ObjectPoolManager.SpawnObject (tutorialStepUIPrefab, tutorialStepParent);
            tutorialStepUI.SetUp(tutorialVisualData.TutorialSteps[i]);
            tutorialStepUIs.Add(tutorialStepUI);
        }
        OnStartedClicked = OnStarted;
        content.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
        hideButton.gameObject.SetActive(false);
        Open();
    }

    public void StartedClicked()
    {
        startButton.gameObject.SetActive(false);
        content.gameObject.SetActive(false);
        hideButton.gameObject.SetActive(true);
        OnStartedClicked?.Invoke();
    }
}
