using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class AbilitySelectMenu : BaseMenu<AbilitySelectMenu>
    {
        public event Func<int, List<RegularActiveAbilityType>, List<RegularActiveAbilityType>> GetRandomActiveAbility;
        public event Func<int, List<PassiveAbilityType>, List<PassiveAbilityType>> GetRandomPassiveAbility;
        public event Func<PassiveAbilityType, int> GetPassiveAbilityLevel;

        public event Func<RegularActiveAbilityType, RegularAbilityVisualData> GetRandomActiveAbilityVisualData;
        public event Func<PassiveAbilityType, PassiveAbilityVisualData> GetRandomPassiveAbilityVisualData;
        public event Func<RegularActiveAbilityType, int> GetActiveAbilityLevel;

        public event Action<RegularActiveAbilityType> OnRegularAbilitySelected;
        public event Action<PassiveAbilityType> OnPassiveAbilitySelected;
        public event Action<RegularActiveAbilityType, RegularActiveAbilityType> OnActiveAbilitySwapped;

        [SerializeField] private AbilityButtonUI[] abilityButtonUIs;
        [SerializeField] private AdvanceButton reRollButton;

        List<PassiveAbilityType> currentPassiveDisplayed = new List<PassiveAbilityType>();
        RegularActiveAbilityType currentActiveDisplayed = RegularActiveAbilityType.None;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            reRollButton.onClick.AddListener(ReRoll);
        }

        public override void OnOpened()
        {
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

            bool activeAbilityChance = UnityEngine.Random.Range(0, 100) < 50;

            //bool activeAbilityChance = true;

            List<PassiveAbilityType> passiveAbilityTypes = GetRandomPassiveAbility.Invoke(3, currentPassiveDisplayed);

            currentPassiveDisplayed.Clear();

            for (int i = 0; i < passiveAbilityTypes.Count; i++)
            {
                if (activeAbilityChance && i == 2)
                {
                    RegularActiveAbilityType passiveActiveAbilityType = GetRandomActiveAbility.Invoke(1, new List<RegularActiveAbilityType>() { currentActiveDisplayed })[0];
                    currentActiveDisplayed = passiveActiveAbilityType;
                    RegularAbilityVisualData regularAbilityVisualData = GetRandomActiveAbilityVisualData.Invoke(passiveActiveAbilityType);
                    abilityButtonUIs[i].SetInfo(regularAbilityVisualData.Icon,"Active", regularAbilityVisualData.DisplayName, regularAbilityVisualData.Description, GetActiveAbilityLevel(passiveActiveAbilityType));
                    abilityButtonUIs[i].GetAdvanceButton.onClick.AddListener(() =>
                    {
                        OnRegularAbilitySelected?.Invoke(passiveActiveAbilityType);
                        AbilitySelected();
                    });
                }
                else
                {
                    currentPassiveDisplayed.Add(passiveAbilityTypes[i]);
                    PassiveAbilityVisualData abilityVisualData = GetRandomPassiveAbilityVisualData.Invoke(passiveAbilityTypes[i]);
                    abilityButtonUIs[i].SetInfo(abilityVisualData.Icon, "Passive", abilityVisualData.DisplayName, abilityVisualData.Description, GetPassiveAbilityLevel(passiveAbilityTypes[i]));
                    PassiveAbilityType passiveAbilityType = passiveAbilityTypes[i];
                    abilityButtonUIs[i].GetAdvanceButton.onClick.AddListener(() =>
                    {
                        OnPassiveAbilitySelected?.Invoke(passiveAbilityType);
                        AbilitySelected();
                    });
                }
            }
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
    }
}
