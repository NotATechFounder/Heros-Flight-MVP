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
        public event Func<CharacterSO[]> GetAllCharacterSO;
        public event Action<CharacterType, bool> OnCharacterSelected;
        public event Func<CharacterType, bool> OnTryBuyCharacter;

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

        [Header("Unlocking")]
        [SerializeField] private GameObject unlockUI;
        [SerializeField] private AdvanceButton unlockButton;
        [SerializeField] private TextMeshProUGUI unlockPrice;
        [SerializeField] private TextMeshProUGUI unlockDescription;

        [Header("Properties")]
        [SerializeField] UiSpineViewController uiSpineViewController;
        [SerializeField] CharacterSelectUI characterSelectUIPrefab;
        [SerializeField] Transform characterSelectUIParent;

        private List<CharacterSelectUI> characterSelectUIs = new List<CharacterSelectUI>();
        private JuicerRuntime openEffectBG;
        private  JuicerRuntime closeEffectBG;
        private CharacterSelectUI currentCharacterSelected;
        private CharacterSelectUI currentCharacterInView;
        private bool isCharacterLoaded;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            quitButton.onClick.AddListener(Close);
            
            unlockButton.onClick.AddListener(() =>
            {
                if (OnTryBuyCharacter?.Invoke(currentCharacterInView.GetCharacterSO.CharacterType) == true)
                {
                    unlockUI.SetActive(false);

                    if (currentCharacterSelected != null)
                    {
                        currentCharacterSelected.SetState(CharacterSelectUI.State.Unselected);
                        OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, false);
                    }
                    currentCharacterSelected = currentCharacterInView;
                    currentCharacterSelected.SetState(CharacterSelectUI.State.Selected);
                    OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, true);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
            });
            
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();

            // if (!isCharacterLoaded)
            // {
                // SetupCharacterCards();
                // }
            // isCharacterLoaded = true;
            
            SetupCharacterCards();
        }

        private void SetupCharacterCards()
        {
            CharacterSO[] allCharacterSO = GetAllCharacterSO.Invoke();
            for (int i = 0; i < allCharacterSO.Length; i++)
            {
                CharacterSelectUI characterSelectUI = Instantiate(characterSelectUIPrefab, characterSelectUIParent);
                characterSelectUI.OnSelected += OnCharacterSelectedSO;
                characterSelectUI.Init(allCharacterSO[i]);
                characterSelectUIs.Add(characterSelectUI);
            }
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            CleanUpUi();
        }

        private void CleanUpUi()
        {
            foreach (var entry in characterSelectUIs)
            {
                Destroy(entry.gameObject);
            }

            characterSelectUIs.Clear();
        }

        public override void ResetMenu()
        {

        }

        private void OnCharacterSelectedSO(CharacterSelectUI characterSelectUI)
        {
            if (currentCharacterInView == characterSelectUI)
            {
                return;
            }

            if (currentCharacterInView != null)
            {
                currentCharacterInView.ToggleInView(false);
            }   

            currentCharacterInView = characterSelectUI;

            currentCharacterInView.ToggleInView(true);

            if (characterSelectUI.GetCharacterSO.CharacterData.isUnlocked)
            {
                if (currentCharacterSelected != null)
                {
                    currentCharacterSelected.SetState(CharacterSelectUI.State.Unselected);
                    OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, false);
                }
                currentCharacterSelected = characterSelectUI;
                currentCharacterSelected.SetState(CharacterSelectUI.State.Selected);
                OnCharacterSelected?.Invoke(currentCharacterSelected.GetCharacterSO.CharacterType, true);
            }
            else
            {
                unlockPrice.text = currentCharacterInView.GetCharacterSO.UnlockPrice.ToString("F0");
                unlockDescription.text =characterSelectUI.GetCharacterSO.CharacterUiData.UnlockDescription;
            }

            uiSpineViewController.SetupView(currentCharacterInView.GetCharacterSO);
            heroName.text = currentCharacterInView.GetCharacterSO.CharacterType.ToString();
            heroDescription.text = currentCharacterInView.GetCharacterSO.CharacterUiData.Description;
            heroPlayStyle.text = currentCharacterInView.GetCharacterSO.CharacterUiData.PlaystyleDescription;
            heroUltimateInfo.text = currentCharacterInView.GetCharacterSO.CharacterUiData.UltimateDescription;
            currentAtk.text = currentCharacterInView.GetCharacterSO.GetPlayerStatData.PhysicalDamage.max.ToString("F0");
            currentHp.text = currentCharacterInView.GetCharacterSO.GetPlayerStatData.Health.ToString("F0");
            currentDef.text = currentCharacterInView.GetCharacterSO.GetPlayerStatData.Defense.ToString("F0");
            unlockUI.SetActive(!currentCharacterInView.GetCharacterSO.CharacterData.isUnlocked);
        }
    }
}
