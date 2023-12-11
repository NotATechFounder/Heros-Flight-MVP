using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UISystem
{
    public enum MenuNavigationButtonType
    {
        World,
        Shop,
        Inventory,
        Traits,
        Other,
    }

    [System.Serializable]
    public class NavigationButton
    {
        public MenuNavigationButtonType navigationButtonType;
        public AdvanceButton advanceButton;
    }

    public class MainMenu : BaseMenu<MainMenu>
    {
        public event Action OnPlayButtonPressed;
        public event Action OnSettingsButtonPressed;

        public event Action<MenuNavigationButtonType> OnNavigationButtonClicked;

        public event Action OnDailyRewardButtonPressed;

        public event Func<WorldType, bool> IsWorldUnlocked;
        public event Action<WorldType> OnWorldChanged;
        public event Func<WorldType, int> GetMaxLevelReached;   

        public event Action AddGold;
        public event Action AddGem;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI gemText;

        [Header("Quest")]
        [SerializeField] private TextMeshProUGUI questText;
        [SerializeField] private Image questProgressBar;
        [SerializeField] private AdvanceButton questClaimButton;

        [Header("Main Buttons")]
        [SerializeField] private AdvanceButton addGoldButton;
        [SerializeField] private AdvanceButton addGemButton;
        [SerializeField] private AdvanceButton playButton;
        [SerializeField] private AdvanceButton settingsButton;
        [SerializeField] private AdvanceButton dailyRewardButton;

        [Header("Nav Buttons")]
        [SerializeField] private NavigationButton[] navigationButtons;

        [Header("World")]
        [SerializeField] private Image worldImage;
        [SerializeField] private Image worldLock;
        [SerializeField] private TextMeshProUGUI worldNameText;
        [SerializeField] private TextMeshProUGUI worldLevelText;
        [SerializeField] private AdvanceButton worldLeftButton;
        [SerializeField] private AdvanceButton worldRightButton;

        private Dictionary<WorldType, WorldVisualSO> worldVisualDic = new Dictionary<WorldType, WorldVisualSO>();

        private WorldVisualSO[] worldVisualSOList;

        public NavigationButton[] NavigationButtons => navigationButtons;

        private NavigationButton currentNavigationButton;

        [Header("Debug")]
        [SerializeField] private WorldType worldInView;

        public override void OnCreated()
        {
            addGoldButton.onClick.AddListener(() =>
            {
                AddGold?.Invoke();
            });

            addGemButton.onClick.AddListener(() =>
            {
                AddGem?.Invoke();
            });

            playButton.onClick.AddListener(()=>
            {
                OnPlayButtonPressed?.Invoke();
            });

            settingsButton.onClick.AddListener(() =>
            {
                OnSettingsButtonPressed?.Invoke();
            });
            
            worldLeftButton.onClick.AddListener(() =>
            {
                NavigateWorld(-1);
            }); 

            worldRightButton.onClick.AddListener(() =>
            {
                NavigateWorld(1);
            });

            dailyRewardButton.onClick.AddListener(() =>
            {
                OnDailyRewardButtonPressed?.Invoke();
            });

            foreach (NavigationButton navigationButton in navigationButtons)
            {
                navigationButton.advanceButton.onClick.AddListener(() =>
                {
                    if (currentNavigationButton != null && currentNavigationButton == navigationButton)
                    {
                        return;
                    }
                    currentNavigationButton = navigationButton;
                    OnNavigationButtonClicked?.Invoke(navigationButton.navigationButtonType);
                });
            }
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

        public void CurrencyChanged(CurrencySO sO, bool increase)
        {
            switch (sO.GetKey)
            {
                case CurrencyKeys.Gold:
                    UpdateGoldText(sO.GetCurrencyAmount);
                    break;
                case CurrencyKeys.Gem:
                    UpdateGemText(sO.GetCurrencyAmount);
                    break;
            }
        }
         
        public void LoadWorlds(WorldVisualSO[] worldVisualSOs)
        {
            worldVisualSOList = worldVisualSOs;
            worldVisualDic.Clear();
            foreach (WorldVisualSO worldVisualSO in worldVisualSOList)
            {
                worldVisualDic.Add(worldVisualSO.worldType, worldVisualSO);
            }
            worldInView = WorldType.World1;
            DisplayWorldInfo(worldInView, true);
        }

        private void NavigateWorld(int direction)
        {
            worldInView = (WorldType)(((int)worldInView + direction) % worldVisualSOList.Length);
            if (worldInView < 0)
            {
                worldInView = (WorldType)worldVisualSOList.Length - 1;
            }
            bool isUnlocked = IsWorldUnlocked?.Invoke(worldInView) ?? false;

            if (isUnlocked)
            {
                OnWorldChanged?.Invoke(worldInView);
            }
        }

        private void DisplayWorldInfo(WorldType worldType, bool isUnlocked)
        {
            playButton.interactable = isUnlocked;
            worldLock.gameObject.SetActive(!isUnlocked);
            DisplayWorldInfo(worldInView, isUnlocked);

            worldImage.sprite = worldVisualDic[worldType].icon;
            worldNameText.text = worldVisualDic[worldType].worldName;
            worldLevelText.text = isUnlocked ? GetMaxLevelReached?.Invoke(worldType).ToString() + " / 30" : "Locked";
        }
    }
}

