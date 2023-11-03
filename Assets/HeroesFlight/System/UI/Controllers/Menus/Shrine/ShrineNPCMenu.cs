using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UISystem;
using UnityEngine;

public class ShrineNPCMenu : BaseMenu<ShrineNPCMenu>
{
    public event Action<ShrineNPCCurrencyType> OnPurcaseRequested;

    [SerializeField] private Transform container;

    [Header("Buttons")]
    [SerializeField] private AdvanceButton runeShardsButton;
    [SerializeField] private AdvanceButton gemsButton;
    [SerializeField] private AdvanceButton adsButton;
    [SerializeField] private AdvanceButton closeButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI runeShardsText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private TextMeshProUGUI adsText;

    JuicerRuntime openEffectBG;
    JuicerRuntime openEffectContainer;
    JuicerRuntime closeEffectBG;

    public override void OnCreated()
    {
        container.localScale = Vector3.zero;

        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        openEffectContainer = container.JuicyScale(Vector3.one, 0.5f)
                                        .SetEase(Ease.EaseInQuint)
                                        .SetDelay(0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
        closeEffectBG.SetOnCompleted(CloseMenu);

        runeShardsButton.onClick.AddListener(() => Buy(ShrineNPCCurrencyType.RuneShard));
        gemsButton.onClick.AddListener(() => Buy(ShrineNPCCurrencyType.Gem));
        adsButton.onClick.AddListener(() => Buy (ShrineNPCCurrencyType.Ad));
        closeButton.onClick.AddListener(() => Close());
    }

    public override void OnOpened()
    {
        openEffectBG.Start(() => canvasGroup.alpha = 0);
        openEffectContainer.Start(() => container.localScale = Vector3.zero);
    }

    public override void OnClosed()
    {
        closeEffectBG.Start();
        ResetMenu();
    }

    public override void ResetMenu()
    {

    }

    public void Buy(ShrineNPCCurrencyType curencyType)
    {
        OnPurcaseRequested?.Invoke(curencyType);
    }
}
