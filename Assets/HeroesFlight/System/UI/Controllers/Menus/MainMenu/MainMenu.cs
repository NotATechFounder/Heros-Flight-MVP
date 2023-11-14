using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace UISystem
{
    public class MainMenu : BaseMenu<MainMenu>
    {
        public event Action OnPlayButtonPressed;
        public event Action OnSettingsButtonPressed;
        public event Action OnTraitButtonPressed;
        public event Action OnInventoryButtonPressed;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI gemText;

        [SerializeField] private AdvanceButton playButton;
        [SerializeField] private AdvanceButton settingsButton;
        [SerializeField] private AdvanceButton traitsButton;
        [SerializeField] private AdvanceButton inventoryButton;
        [SerializeField] private CharacterUI characterUIPrefab;

        public override void OnCreated()
        {
            playButton.onClick.AddListener(()=>
            {
                OnPlayButtonPressed?.Invoke();
            });

            settingsButton.onClick.AddListener(() =>
            {
                OnSettingsButtonPressed?.Invoke();
            });
            
            traitsButton.onClick.AddListener(() =>
            {
                OnTraitButtonPressed?.Invoke();
            });

            inventoryButton.onClick.AddListener(() =>
            {
                OnInventoryButtonPressed?.Invoke();
            });
        }

        public override void OnOpened()
        {

        }

        public override void OnClosed()
        {
            CloseMenu();
        }

        public override void ResetMenu()
        {

        }

        public void UpdateGoldText(float gold)
        {
            goldText.text = gold.ToString();
        }

        public void UpdateGemText(float gem)
        {
            gemText.text = gem.ToString();
        }
    }
}

