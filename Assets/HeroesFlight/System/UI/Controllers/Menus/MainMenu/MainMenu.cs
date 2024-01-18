using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Pelumi.Juicer;

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
        public GameObject LockIcon;
        public GameObject UpdatePin;
        public int requiredLvl = 0;
    }

    public struct QuestStatusRequest
    {
        public string questName;
        public int questProgress;
        public int questGoal;
        public float normalizedProgress => (float)questProgress / questGoal;
        public bool isCompleted => questProgress >= questGoal;

        public QuestStatusRequest(string questName, int questProgress, int questGoal)
        {
            this.questName = questName;
            this.questProgress = questProgress;
            this.questGoal = questGoal;
        }
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

        public event Action OnQuestClaimButtonPressed;
        public event Func<QuestStatusRequest> questStatusRequest;

        public event Func<LevelSystem.ExpIncreaseResponse> GetCurrentAccountLevelXP;

        [Header("Top UI")] [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI gemText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI energyTimeText;

        [Header("Account UI")] [SerializeField]
        private TextMeshProUGUI acctLevelText;

        [SerializeField] private Image acctLevelBar;

        [Header("Quest")] [SerializeField] private TextMeshProUGUI questText;
        [SerializeField] private TextMeshProUGUI questProgressText;
        [SerializeField] private Image questProgressBar;
        [SerializeField] private AdvanceButton questClaimButton;

        [Header("Main Buttons")] [SerializeField]
        private AdvanceButton addGoldButton;

        [SerializeField] private AdvanceButton addGemButton;
        [SerializeField] private AdvanceButton playButton;
        [SerializeField] private AdvanceButton settingsButton;
        [SerializeField] private AdvanceButton dailyRewardButton;

        [Header("Nav Buttons")] [SerializeField]
        private NavigationButton[] navigationButtons;

        [Header("World")] [SerializeField] private Image worldImage;
        [SerializeField] private Image worldLock;
        [SerializeField] private TextMeshProUGUI worldNameText;
        [SerializeField] private TextMeshProUGUI worldLevelText;
        [SerializeField] private AdvanceButton worldLeftButton;
        [SerializeField] private AdvanceButton worldRightButton;

        private Dictionary<WorldType, WorldVisualSO> worldVisualDic = new Dictionary<WorldType, WorldVisualSO>();

        private WorldVisualSO[] worldVisualSOList;

        public NavigationButton[] NavigationButtons => navigationButtons;

        private NavigationButton currentNavigationButton;

        JuicerRuntime questComplectedEffect;

        [Header("Debug")] [SerializeField] private WorldType worldInView;

        public override void OnCreated()
        {
            questComplectedEffect = questProgressBar.JuicyColour(Color.red, 0.5f);
            questComplectedEffect.SetLoop(-1).SetOnCompleted(() => questProgressBar.color = Color.yellow);

            addGoldButton.onClick.AddListener(() => { AddGold?.Invoke(); });

            addGemButton.onClick.AddListener(() => { AddGem?.Invoke(); });

            playButton.onClick.AddListener(() => { OnPlayButtonPressed?.Invoke(); });

            settingsButton.onClick.AddListener(() => { OnSettingsButtonPressed?.Invoke(); });

            worldLeftButton.onClick.AddListener(() => { NavigateWorld(-1); });

            worldRightButton.onClick.AddListener(() => { NavigateWorld(1); });

            dailyRewardButton.onClick.AddListener(() => { OnDailyRewardButtonPressed?.Invoke(); });

            foreach (NavigationButton navigationButton in navigationButtons)
            {
                navigationButton.advanceButton.onClick.AddListener(() =>
                {
                    // if (currentNavigationButton != null && currentNavigationButton == navigationButton)
                    // {
                    //     return;
                    // }
                    UpdateButtonPinState(navigationButton.navigationButtonType, false);

                    currentNavigationButton = navigationButton;
                    OnNavigationButtonClicked?.Invoke(navigationButton.navigationButtonType);
                });
            }

            questClaimButton.onClick.AddListener(() =>
            {
                OnQuestClaimButtonPressed?.Invoke();
                questComplectedEffect.Stop();
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

        public void Open(int playerLVl)
        {
            ProcessButtonStates(playerLVl);
            Open();
        }

        public void ProcessButtonStates(int playerLVl)
        {
            foreach (var buttonEntry in navigationButtons)
            {
                var shouldBeUnlocked = playerLVl >= buttonEntry.requiredLvl;

                buttonEntry.advanceButton.interactable = shouldBeUnlocked;
                buttonEntry.LockIcon.SetActive(!shouldBeUnlocked);
            }
        }

        public void UpdateGoldText(float gold)
        {
            goldText.text = gold.ToString();
        }

        public void UpdateGemText(float gem)
        {
            gemText.text = gem.ToString();
        }

        public void UpdateEnergyText(float energy)
        {
            energyText.text = energy.ToString();
        }

        public void UpdateEnergyTime(string time)
        {
            energyTimeText.text = time;
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
                case CurrencyKeys.Energy:
                    UpdateEnergyText(sO.GetCurrencyAmount);
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

            DisplayWorldInfo(worldInView, isUnlocked);
        }

        private void DisplayWorldInfo(WorldType worldType, bool isUnlocked)
        {
            playButton.SetVisibility(isUnlocked ? GameButtonVisiblity.Visible : GameButtonVisiblity.Hidden);
            worldLock.gameObject.SetActive(!isUnlocked);

            worldImage.sprite = worldVisualDic[worldType].icon;
            worldNameText.text = worldVisualDic[worldType].worldName;
            worldLevelText.text = isUnlocked ? GetMaxLevelReached?.Invoke(worldType).ToString() + " / 30" : "Locked";
        }

        public void UpdateQuestInfo(QuestStatusRequest questStatusRequest)
        {
            questText.text = questStatusRequest.questName;
            questProgressText.text = questStatusRequest.isCompleted
                ? "Claim Reward"
                : questStatusRequest.questProgress.ToString() + " / " + questStatusRequest.questGoal.ToString();
            questProgressBar.fillAmount = questStatusRequest.normalizedProgress;
            questClaimButton.interactable = questStatusRequest.isCompleted;

            if (questStatusRequest.isCompleted)
            {
                questComplectedEffect.Start();
            }
        }

        public void ClickNavigationButton(MenuNavigationButtonType menuNavigationButtonType)
        {
            NavigationButton navigationButton =
                navigationButtons.FirstOrDefault(x => x.navigationButtonType == menuNavigationButtonType);
            if (navigationButton != null)
            {
                navigationButton.advanceButton.onClick.Invoke();
            }
        }

        public void AccountLevelUp(LevelSystem.ExpIncreaseResponse response)
        {
            acctLevelText.text = response.currentLevel.ToString();
            acctLevelBar.fillAmount = response.normalizedExp;
        }

        public void UpdateButtonPinState(MenuNavigationButtonType buttonType, bool isEnabled)
        {
            Debug.Log($"should disable pin for button {buttonType}");
            foreach (var button in navigationButtons)
            {
                if (button.navigationButtonType == buttonType)
                {
                    if (button.UpdatePin != null)
                    {
                        Debug.Log($" disable pin for button {buttonType} with state {isEnabled}");
                        button.UpdatePin.SetActive(isEnabled);
                    }
                    else
                    {
                        Debug.LogError($"{buttonType} do not have pin object on it ");
                    }

                    break;
                }
            }
        }
    }
}