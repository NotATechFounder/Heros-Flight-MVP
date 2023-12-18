using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Container;
using StansAssets.Foundation.Extensions;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.UI
{
    public class UiSystem : IUISystem
    {
        public UiSystem(DataSystemInterface dataSystemInterface) { }

        public event Action OnReturnToMainMenuRequest;

        public event Action OnRestartLvlRequest;

        public event Action OnReviveCharacterRequest;

        public event Action OnSpecialButtonClicked;

        public event Action<int> OnPassiveAbilityButtonClicked;

        public UIEventHandler UiEventHandler { get; private set; }

        public CountDownTimer GameTimer { get; private set; }

         const string MainMenuMusicID = "MainMenu";

         const string GameMusicID = "ForestStart";

         const string GameMusicLoopID = "ForestLoop";

        UiContainer container;


        public void Init(Scene scene = default, Action onComplete = null)
        {
            UiEventHandler = scene.GetComponent<UIEventHandler>();
            container = scene.GetComponent<UiContainer>();
            GameTimer = new CountDownTimer(UiEventHandler);

            UiEventHandler.Init(() =>
            {
                UiEventHandler.MainMenu.OnMenuOpened += () => { AudioManager.PlayMusic(MainMenuMusicID); };

                UiEventHandler.MainMenu.OnSettingsButtonPressed += () => { UiEventHandler.SettingsMenu.Open(); };

                UiEventHandler.MainMenu.OnDailyRewardButtonPressed += () => { UiEventHandler.DailyRewardMenu.Open(); };

                UiEventHandler.MainMenu.OnNavigationButtonClicked += MainMenu_OnNavigationButtonClicked;

                UiEventHandler.SettingsMenu.OnBackButtonPressed += () => { UiEventHandler.SettingsMenu.Close(); };

                UiEventHandler.GameMenu.OnMenuOpened += () =>
                {
                    AudioManager.BlendTwoMusic(GameMusicID, GameMusicLoopID);
                };

                UiEventHandler.GameMenu.OnSpecialAttackButtonClicked += () => { OnSpecialButtonClicked?.Invoke(); };

                UiEventHandler.GameMenu.OnPassiveAbilityButtonClicked += (index) => { OnPassiveAbilityButtonClicked?.Invoke(index); };

                onComplete?.Invoke();
            });
        }

        public void AssignGameEvents()
        {
            UiEventHandler.GameMenu.OnLevelUpComplete += (level) => { UiEventHandler.AbilitySelectMenu.Open(); };

            UiEventHandler.GameMenu.OnPauseButtonClicked += () => { UiEventHandler.PauseMenu.Open(); };

            UiEventHandler.PauseMenu.OnSettingsButtonClicked += () => { UiEventHandler.SettingsMenu.Open(); };

            UiEventHandler.PauseMenu.OnResumeButtonClicked += () => { UiEventHandler.PauseMenu.Close(); };

            UiEventHandler.PauseMenu.OnQuitButtonClicked += () =>
            {
                UiEventHandler.ConfirmationMenu.Display(UiEventHandler.BackToMenuConfirmation, ReturnToMainMenu,null);
            };

            UiEventHandler.ReviveMenu.OnWatchAdsButtonClicked += () =>
            {
                OnReviveCharacterRequest?.Invoke();
                return true;
            };

            UiEventHandler.ReviveMenu.OnGemButtonClicked += () =>
            {
                OnRestartLvlRequest?.Invoke();
                return true;
            };


            UiEventHandler.SummaryMenu.OnMenuOpened += () => { };

            UiEventHandler.SummaryMenu.GetCurrentGold = () => { return UiEventHandler.GameMenu.CoinText.text; };

            UiEventHandler.SummaryMenu.OnContinueButtonClicked += () =>
            {
                OnReturnToMainMenuRequest?.Invoke();
                UiEventHandler.SummaryMenu.Close();
            };
        }

        private void MainMenu_OnNavigationButtonClicked(UISystem.MenuNavigationButtonType obj)
        {
            TryDisableMenu(UiEventHandler.SettingsMenu);

            switch (obj)
            {
                case MenuNavigationButtonType.Shop:
                    ProceedButtonShopClick();
                    break;
                case MenuNavigationButtonType.Inventory:
                   ProceedInventoryButtonClicked();
                    break;
                case MenuNavigationButtonType.World:
                    ProceedWorldButtonClicked();
                    break;
                case MenuNavigationButtonType.Traits:
                    ProceedTraitsButtonClicked();
                    break;
            }
        }

        void TryDisableMenu<T>(BaseMenu<T> menu) where T: BaseMenu<T>
        {
            if (menu.isActiveAndEnabled)
            {
              
                menu.Close();
            }
        }

        void TryEnableMenu<T>(BaseMenu<T> menu) where T : BaseMenu<T>
        {
            if (!menu.isActiveAndEnabled)
            {

                menu.Open();
            }
        }

        private void ProceedTraitsButtonClicked()
        {
            TryDisableMenu(UiEventHandler.ShopMenu);
            TryDisableMenu(UiEventHandler.InventoryMenu);
            TryDisableMenu(UiEventHandler.CharacterSelectMenu);
        }

        private void ProceedWorldButtonClicked()
        {
            TryDisableMenu(UiEventHandler.ShopMenu);
            TryDisableMenu(UiEventHandler.InventoryMenu);
            TryDisableMenu(UiEventHandler.CharacterSelectMenu);
            TryDisableMenu(UiEventHandler.TraitTreeMenu);
        }

        private void ProceedInventoryButtonClicked()
        {
            TryDisableMenu(UiEventHandler.ShopMenu);
            TryDisableMenu(UiEventHandler.CharacterSelectMenu);
            TryDisableMenu(UiEventHandler.TraitTreeMenu);
        }

        private void ProceedButtonShopClick()
        {
            TryDisableMenu(UiEventHandler.InventoryMenu);
            TryDisableMenu(UiEventHandler.CharacterSelectMenu);
            TryDisableMenu(UiEventHandler.TraitTreeMenu);

            TryEnableMenu (UiEventHandler.ShopMenu);      
        }

        public void Reset() { }

        public void DisplayStartInfoMessage(float duration)
        {
            UiEventHandler.GameMenu.DisplayInfoMessage(UISystem.GameMenu.InfoMessageType.Start, duration);
        }

        public void UpdateUltimateButton(float value)
        {
            UiEventHandler.GameMenu.CheckIfSpecialReady(value);
        }


        public void UpdateSpecialEnemyHealthBar(float value)
        {
            UiEventHandler.GameMenu.UpdateBossHealthFill(value);
        }

        public void ToggleSpecialEnemyHealthBar(bool isEnabled)
        {
            if (isEnabled)
            {
                UiEventHandler.GameMenu.UpdateBossHealthFill(1);
                UiEventHandler.GameMenu.ToggleBossHpBar(true);
            }
            else
            {
                UiEventHandler.GameMenu.ToggleBossHpBar(false);
            }
        }

        public void UpdateGameTimeUI(float timeLeft)
        {
            UiEventHandler.GameMenu.UpdateTimerText(timeLeft);
        }

        public void ReturnToMainMenu()
        {
            OnReturnToMainMenuRequest?.Invoke();
            UiEventHandler.GameMenu.Close();
            UiEventHandler.PauseMenu.Close();
            UiEventHandler.ConfirmationMenu.Close();
        }   

        public void ShowDamageText(float damage, Transform target, bool isCritical, bool targetIsPlayer,
            bool isHeal = false)
        {
            if (targetIsPlayer)
            {
                var damageString = string.Empty;
                 damageString = !isCritical
                    ? $"{(int)damage}"
                    : $"!!{(int)damage}!!";
                var color = isHeal ? Color.green : Color.red;
                PopUpManager.Instance.PopUpTextAtTransfrom(target, Vector3.zero, damageString,
                    color);
            }
            else
            {
                var damageText = NumberConverter.ConvertNumberToString((int)damage);
                var spriteAsset = container.GetDamageTextSprite(isCritical);
                var size = !isCritical ? 60 : 100;
                PopUpManager.Instance.PopUpTextAtTransfrom(target, Vector3.zero, damageText,
                    spriteAsset, size);
            }
        }

        public void UpdateEnemiesCounter(int enemiesLeft)
        {
            UiEventHandler.GameMenu.UpdateEnemyCountText(enemiesLeft);
        }

        public void ShowPopupAtPosition(string info, Vector2 pos, Color color)
        {
            PopUpManager.Instance.PopUpAtTextPosition(pos, Vector3.zero, info, color);
        }

        public void UpdateComboUI(int count)
        {
            UiEventHandler.GameMenu.UpdateComboCounterText(count);
        }


        public void ShowSpecialEnemyWarning(EncounterType encounterType)
        {
            UiEventHandler.GameMenu.ShowMiniBossWarning(encounterType);
        }

        public void UpdateCoinsUi(int amount)
        {
            UiEventHandler.GameMenu.UpdateCoinText(amount);
        }
    }
}