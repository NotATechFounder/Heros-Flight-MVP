using HeroesFlight.Common.Enum;
using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

namespace UISystem
{
    public class CharacterSelectMenu : BaseMenu<CharacterSelectMenu>
    {
        public event Action<CharacterType, bool> OnCharacterSelected;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton quitButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI heroName;
        [SerializeField] private TextMeshProUGUI heroDescription;
        [SerializeField] private TextMeshProUGUI heroPlayStyle;
        [SerializeField] private TextMeshProUGUI heroUltimateInfo;
        [SerializeField] private TextMeshProUGUI currentAtk;
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI currentDef;

        [Header("Properties")]
        [SerializeField] CharacterSO[] allCharacterSO;
        [SerializeField] UiSpineViewController uiSpineViewController;
        [SerializeField] CharacterSelectUI characterSelectUIPrefab;
        [SerializeField] Transform characterSelectUIParent;

        private List<CharacterSelectUI> characterSelectUIs = new List<CharacterSelectUI>();
        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        CharacterSelectUI currentCharacterSelected;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            quitButton.onClick.AddListener(Close);

            for (int i = 0; i < allCharacterSO.Length; i++)
            {
                CharacterSelectUI characterSelectUI = Instantiate(characterSelectUIPrefab, characterSelectUIParent);
                characterSelectUI.OnSelected += OnCharacterSelectedSO;
                characterSelectUI.Init(allCharacterSO[i]);
                characterSelectUIs.Add(characterSelectUI);
            }
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        private void OnCharacterSelectedSO(CharacterSelectUI characterSelectUI)
        {
            if (currentCharacterSelected == characterSelectUI)
            {
                return;
            }

            if (currentCharacterSelected != null)
            {
                currentCharacterSelected.SetState(CharacterSelectUI.State.Unselected);
                OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, false);
            }

            currentCharacterSelected = characterSelectUI;
            OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, true);

            uiSpineViewController.SetupView(currentCharacterSelected.GetCharacterSO);
            heroName.text = currentCharacterSelected.GetCharacterSO.CharacterType.ToString();
            heroDescription.text = "Description: ";
            heroPlayStyle.text = "Playstyle: ";
            heroUltimateInfo.text = "Ultimate: ";
            currentAtk.text = currentCharacterSelected.GetCharacterSO.GetPlayerStatData.PhysicalDamage.max.ToString("F0");
            currentHp.text = currentCharacterSelected.GetCharacterSO.GetPlayerStatData.Health.ToString("F0");
            currentDef.text = currentCharacterSelected.GetCharacterSO.GetPlayerStatData.Defense.ToString("F0");
        }
    }
}
