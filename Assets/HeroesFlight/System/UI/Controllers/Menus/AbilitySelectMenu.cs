using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class AbilitySelectMenu : BaseMenu<AbilitySelectMenu>
    {
        public event Action OnGemReRoll;
        public event Action OnAdsReRoll;

        public event Func<int, List<ActiveAbilityType>, List<ActiveAbilityType>> GetRandomActiveAbility;
        public event Func<int, List<PassiveAbilityType>, List<PassiveAbilityType>> GetRandomPassiveAbility;
        public event Func<PassiveAbilityType, int> GetPassiveAbilityLevel;

        public event Func<ActiveAbilityType, ActiveAbilityVisualData> GetRandomActiveAbilityVisualData;
        public event Func<PassiveAbilityType, PassiveAbilityVisualData> GetRandomPassiveAbilityVisualData;
        public event Func<ActiveAbilityType, int> GetActiveAbilityLevel;

        public event Action<ActiveAbilityType> OnRegularAbilitySelected;
        public event Action<PassiveAbilityType> OnPassiveAbilitySelected;
        public event Action<ActiveAbilityType, ActiveAbilityType> OnActiveAbilitySwapped;

        [SerializeField] private AbilityButtonUI[] abilityButtonUIs;
        [SerializeField] private AdvanceButton gemReRollButton;
        [SerializeField] private AdvanceButton adsReRollButton;

        List<PassiveAbilityType> currentPassiveDisplayed = new List<PassiveAbilityType>();
        ActiveAbilityType currentActiveDisplayed = ActiveAbilityType.None;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            gemReRollButton.onClick.AddListener(() => OnGemReRoll?.Invoke());
            adsReRollButton.onClick.AddListener(() => OnAdsReRoll?.Invoke());
        }

        public override void OnOpened()
        {
            adsReRollButton.SetVisibility(GameButtonVisiblity.Visible);
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            GenerateAbilities();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void GenerateAbilities()
        {
            RefreshAbilities();

            List<PassiveAbilityType> passiveAbilityTypes = GetRandomPassiveAbility.Invoke(2, currentPassiveDisplayed);

            ActiveAbilityType passiveActiveAbilityType = GetRandomActiveAbility.Invoke(1, new List<ActiveAbilityType>() { currentActiveDisplayed })[0];

            currentPassiveDisplayed = passiveAbilityTypes;
            for (int i = 0; i < passiveAbilityTypes.Count; i++)
            {
                PassiveAbilityVisualData abilityVisualData = GetRandomPassiveAbilityVisualData.Invoke(passiveAbilityTypes[i]);
                abilityButtonUIs[i].SetInfo(abilityVisualData.Icon, "Passive", abilityVisualData.DisplayName, abilityVisualData.Description, GetPassiveAbilityLevel(passiveAbilityTypes[i]));
                PassiveAbilityType passiveAbilityType = passiveAbilityTypes[i];
                abilityButtonUIs[i].GetAdvanceButton.onClick.AddListener(() =>
                {
                    OnPassiveAbilitySelected?.Invoke(passiveAbilityType);
                    AbilitySelected();
                });
            }

            currentActiveDisplayed = passiveActiveAbilityType;
            ActiveAbilityVisualData regularAbilityVisualData = GetRandomActiveAbilityVisualData.Invoke(passiveActiveAbilityType);
            abilityButtonUIs[2].SetInfo(regularAbilityVisualData.Icon, "Active", regularAbilityVisualData.DisplayName, regularAbilityVisualData.Description, GetActiveAbilityLevel(passiveActiveAbilityType));
            abilityButtonUIs[2].GetAdvanceButton.onClick.AddListener(() =>
            {
                OnRegularAbilitySelected?.Invoke(passiveActiveAbilityType);
                AbilitySelected();
            });
        }

        public void ReRoll()
        {
            GenerateAbilities();
        }

        public void RefreshAbilities()
        {
            for (int i = 0; i < abilityButtonUIs.Length; i++)
            {
                abilityButtonUIs[i].GetAdvanceButton.onClick.RemoveAllListeners();
            }
        }

        public void AbilitySelected()
        {
            Close();
        }

        public void AdsReRoll()
        {
            ReRoll();
            adsReRollButton.SetVisibility(GameButtonVisiblity.Hidden);
        }
    }
}
