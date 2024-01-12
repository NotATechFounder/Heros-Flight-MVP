using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

public class HealingNPCMenu : BaseMenu<HealingNPCMenu>
{
    public event Func<ShrineNPCCurrencyType, int> GetCurrencyPrice;
    public event Func<ShrineNPCCurrencyType, bool> OnPurchaseRequested;

    public event Action OnPurchaseCompleted;


   [Header("Buttons")]
    [SerializeField] private Transform container;
    [SerializeField] private Transform currencyHolder;
    [SerializeField] private AdvanceButton runeShardsButton;
    [SerializeField] private AdvanceButton gemsButton;
    [SerializeField] private AdvanceButton adsButton;
    [SerializeField] private AdvanceButton closeButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI runeShardsText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private TextMeshProUGUI adsText;

    [Header("Blockers")]
    [SerializeField] private GameObject runesBlocker;
    [SerializeField] private GameObject gemsBlocker;
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
        adsButton.onClick.AddListener(() => Buy(ShrineNPCCurrencyType.Ad));
        closeButton.onClick.AddListener(() => Close());
    }

    public override void OnOpened()
    {
        openEffectBG.Start(() => canvasGroup.alpha = 0);
        openEffectContainer.Start(() => container.localScale = Vector3.zero);

        if(GetCurrencyPrice != null)
        {
            runeShardsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.RuneShard).ToString();
            gemsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.Gem).ToString();
            adsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.Ad).ToString();
        }

        currencyHolder.gameObject.SetActive(true);
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
        if(OnPurchaseRequested?.Invoke(curencyType) == true)
        {
            OnPurchaseCompleted?.Invoke();
            Close();
        }
    }

    public void Open(float playerRuneShards, float playerGems)
    {
        SetupButtonsView(playerRuneShards, playerGems);
        Open();
    }

    private void SetupButtonsView(float playerRuneShards, float playerGems)
    {
        runesBlocker.SetActive(playerRuneShards<GetCurrencyPrice(ShrineNPCCurrencyType.RuneShard));
        runeShardsButton.interactable = playerRuneShards >= GetCurrencyPrice(ShrineNPCCurrencyType.RuneShard);
        gemsBlocker.SetActive(playerGems<GetCurrencyPrice(ShrineNPCCurrencyType.Gem));
        gemsButton.interactable = playerGems >= GetCurrencyPrice(ShrineNPCCurrencyType.Gem);
    }
}
