using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

public class ActiveAbilityRerollerNPCMenu : BaseMenu<ActiveAbilityRerollerNPCMenu>
{
    public event Func<ShrineNPCCurrencyType, int> GetCurrencyPrice;
    public event Func<ShrineNPCCurrencyType, bool> OnPurchaseRequested;

    public event Func<List<RegularActiveAbilityType>> GetEqquipedActiveAbilityTypes;
    public event Func<int, List<RegularActiveAbilityType>, List<RegularActiveAbilityType>> GetRandomActiveAbilityTypes;
    public event Func<RegularActiveAbilityType, RegularAbilityVisualData> GetActiveAbilityVisualData;
    public event Action<RegularActiveAbilityType, RegularActiveAbilityType> OnActiveAbilitySwapped;

    [Header("Buttons")]
    [SerializeField] private Transform container;
    [SerializeField] private Transform currencyHolder;
    [SerializeField] private AdvanceButton runeShardsButton;
    [SerializeField] private AdvanceButton gemsButton;
    [SerializeField] private AdvanceButton adsButton;
    [SerializeField] private AdvanceButton closeButton;

    [Header("Ability Buttons")]
    [SerializeField] private Transform abilityHolder;
    [SerializeField] private AbilityRerollButtonUI[] eqquipedAbilityRerollButtonUIs;
    [SerializeField] private AbilityRerollButtonUI[] newAbilityRerollButtonUIs;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI runeShardsText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private TextMeshProUGUI adsText;

    JuicerRuntime openEffectBG;
    JuicerRuntime openEffectContainer;
    JuicerRuntime closeEffectBG;

    [Header("Debug")]
    [SerializeField] List<RegularActiveAbilityType> eqquipedActiveAbilities= new List<RegularActiveAbilityType>();
    [SerializeField] private RegularActiveAbilityType selectedNewAbility = RegularActiveAbilityType.None;

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

        abilityHolder.gameObject.SetActive(false);
    }

    public override void OnOpened()
    {
        openEffectBG.Start(() => canvasGroup.alpha = 0);
        openEffectContainer.Start(() => container.localScale = Vector3.zero);

        if (GetCurrencyPrice != null)
        {
            runeShardsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.RuneShard).ToString();
            gemsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.Gem).ToString();
            adsText.text = GetCurrencyPrice(ShrineNPCCurrencyType.Ad).ToString();
        }

        eqquipedActiveAbilities = GetEqquipedActiveAbilityTypes?.Invoke();
        currencyHolder.gameObject.SetActive(eqquipedActiveAbilities.Count != 0);
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
        if (OnPurchaseRequested?.Invoke(curencyType) == true)
        {
            currencyHolder.gameObject.SetActive(false);
            abilityHolder.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);    
            LoadAbilities();
        }
    }

    public void LoadAbilities()
    {
        for (int i = 0; i < eqquipedActiveAbilities.Count; i++)
        {
            RegularActiveAbilityType oldAbiity = eqquipedActiveAbilities[i];
            eqquipedAbilityRerollButtonUIs[i].Init(GetActiveAbilityVisualData.Invoke(eqquipedActiveAbilities[i]).Icon);
            eqquipedAbilityRerollButtonUIs[i].OnClick = () =>
            {
                if (selectedNewAbility != RegularActiveAbilityType.None)
                {
                    OnActiveAbilitySwapped?.Invoke(oldAbiity, selectedNewAbility);
                    selectedNewAbility = RegularActiveAbilityType.None;
                    ReRollComplected();
                }
            };
        }

        List<RegularActiveAbilityType> newAbilities = GetRandomActiveAbilityTypes?.Invoke(3, eqquipedActiveAbilities);
        for (int i = 0; i < newAbilities.Count; i++)
        {
            RegularActiveAbilityType regularActiveAbilityType = newAbilities[i];
            newAbilityRerollButtonUIs[i].Init(GetActiveAbilityVisualData.Invoke(newAbilities[i]).Icon);
            newAbilityRerollButtonUIs[i].OnToggle = (state) =>
            {
                selectedNewAbility = regularActiveAbilityType;
            };
        }     
    }

    public void ReRollComplected()
    {
        //LoadAbilities();
        abilityHolder.gameObject.SetActive(false);
        Close();
    }
}
