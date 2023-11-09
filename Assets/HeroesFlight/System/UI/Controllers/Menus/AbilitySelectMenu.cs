using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class AbilitySelectMenu : BaseMenu<AbilitySelectMenu>
    {
        public event Func<int, List<PassiveActiveAbilityType>> GetRandomPassiveAbility;
        public event Func<PassiveActiveAbilityType, AbilityVisualData> GetRandomPassiveAbilityVisualData;
        public event Action<PassiveActiveAbilityType> OnAbilitySelected;

        [SerializeField] private AbilityButtonUI[] abilityButtonUIs;

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
            canvasGroup.alpha = 0;
            openEffectBG.Start();

            GenerateAbilities();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();

            for (int i = 0; i < abilityButtonUIs.Length; i++)
            {
                abilityButtonUIs[i].GetAdvanceButton.onClick.RemoveAllListeners();
            }
        }

        public override void ResetMenu()
        {

        }

        public void GenerateAbilities()
        {
            List<PassiveActiveAbilityType> passiveActiveAbilityTypes = GetRandomPassiveAbility.Invoke(3);

            for (int i = 0; i < passiveActiveAbilityTypes.Count; i++)
            {
                AbilityVisualData abilityVisualData = GetRandomPassiveAbilityVisualData.Invoke(passiveActiveAbilityTypes[i]);
                abilityButtonUIs[i].SetInfo(abilityVisualData, OnAbilitySelected);
                abilityButtonUIs[i].GetAdvanceButton.onClick.AddListener(() =>
                {
                    AbilitySelected();
                });
            }
        }

        public void AbilitySelected()
        {
            Close();
        }
    }
}
