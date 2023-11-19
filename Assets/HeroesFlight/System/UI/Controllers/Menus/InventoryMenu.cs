using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.Common.Progression;

namespace UISystem
{
    public class InventoryMenu : BaseMenu<InventoryMenu>
    {
        public event Func<StatModel> GetStatModel;
        public event Func<CharacterSO> GetSelectedCharacterSO;
        public event Action OnChangeHeroButtonClicked;
        public event Action OnStatPointButtonClicked;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton changeHeroButton;
        [SerializeField] private AdvanceButton statPointButton;
        [SerializeField] private AdvanceButton quitButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI currentAtk;
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI currentDef;

        [Header("Data")]
        [SerializeField] UiSpineViewController uiSpineViewController;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            changeHeroButton.onClick.AddListener(() => OnChangeHeroButtonClicked?.Invoke());
            statPointButton.onClick.AddListener(() => OnStatPointButtonClicked?.Invoke());
            quitButton.onClick.AddListener(Close);
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            UpdateCharacter(GetSelectedCharacterSO.Invoke());
        }

        public void UpdateCharacter(CharacterSO characterSO)
        {
            uiSpineViewController.SetupView(characterSO);
            OnStatValueChanged (GetStatModel.Invoke());
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void OnStatValueChanged(StatModel statModel)
        {
            foreach (var stat in statModel.CurrentStatDic)
            {
                switch (stat.Key)
                {
                    case StatType.PhysicalDamage:
                        currentAtk.text = stat.Value.ToString("F0");
                        break;
                    case StatType.MaxHealth:
                        currentHp.text = stat.Value.ToString("F0");
                        break;
                    case StatType.Defense:
                        currentDef.text = stat.Value.ToString("F0");
                        break;
                }
            }
        }
    }
}
