using Pelumi.Juicer;
using UISystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SummaryMenu : BaseMenu<SummaryMenu>
{
    public event Action OnContinueButtonClicked;

    [SerializeField] private AdvanceButton continueButton;

    JuicerRuntime openEffectBG;
    JuicerRuntime closeEffectBG;

    public override void OnCreated()
    {
        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
        closeEffectBG.SetOnComplected(CloseMenu);

        continueButton.onClick.AddListener(CloseButtonAction);
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

    private void CloseButtonAction()
    {
        OnContinueButtonClicked?.Invoke();
        Close();
    }
}
