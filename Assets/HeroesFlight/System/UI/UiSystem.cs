using System;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.UI.Container;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.UI
{
    public class UiSystem : IUISystem
    {
        public UiSystem(IDataSystemInterface dataSystemInterface, GamePlaySystemInterface gamePlaySystem)
        {
            dataSystem = dataSystemInterface;
            gameplaySystem = gamePlaySystem;
            gameplaySystem.OnEnemyDamaged += HandleEnemyDamaged;
            gameplaySystem.OnCharacterHealthChanged += HandleCharacterHealthChanged;
            gameplaySystem.OnRemainingEnemiesLeft += UpdateEnemiesCounter;
            gameplaySystem.OnCharacterDamaged += HandleCharacterDamaged;
            gameplaySystem.OnCharacterHeal += HandleCharacterHeal;
            gameplaySystem.OnCharacterComboChanged += UpdateComboUI;
            gameplaySystem.OnMinibossSpawned += HandleMiniboss;
            gameplaySystem.OnMinibossHealthChange += HandleMinibossHealthChange;
            gameplaySystem.GameTimer.OnTimeTick += UpdateGameTimeUI;
            gameplaySystem.OnBoosterActivated += HandleBoosterActivated;
            gameplaySystem.OnCoinsCollected += HandleCoinChange;
        }

        public event Action OnReturnToMainMenuRequest;
        public event Action OnRestartLvlRequest;
        public event Action OnReviveCharacterRequest;

        public UIEventHandler UiEventHandler { get; private set; }

        public CountDownTimer GameTimer { get; private set; }

        public const string MainMenuMusicID = "MainMenu";

        public const string GameMusicID = "ForestStart";

        public const string GameMusicLoopID = "ForestLoop";

        UiContainer container;
        GamePlaySystemInterface gameplaySystem;
        IDataSystemInterface dataSystem;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            UiEventHandler = scene.GetComponent<UIEventHandler>();
            container = scene.GetComponent<UiContainer>();
            GameTimer = new CountDownTimer(UiEventHandler);

            UiEventHandler.Init(() =>
            {
                UiEventHandler.MainMenu.OnMenuOpened += () =>
                {
                    AudioManager.PlayMusic(MainMenuMusicID);
                };

                UiEventHandler.MainMenu.OnPlayButtonPressed += OnPlayButtonPressed;
                UiEventHandler.MainMenu.OnSettingsButtonPressed += () =>
                {
                    UiEventHandler.SettingsMenu.Open();
                };

                UiEventHandler.SettingsMenu.OnBackButtonPressed += () =>
                {
                    UiEventHandler.SettingsMenu.Close();
                };

                UiEventHandler.GameMenu.OnMenuOpened += () =>
                {
                    AudioManager.BlendTwoMusic(GameMusicID, GameMusicLoopID);
                };


                UiEventHandler.GameMenu.OnPauseButtonClicked += () =>
                {
                    UiEventHandler.PauseMenu.Open();
                };

                //UiEventHandler.GameMenu.GetCoinText = () =>
                //{
                //    return dataSystem.GetCurrencyAmount(CurrencyKeys.Gold);
                //};

                UiEventHandler.PauseMenu.OnSettingsButtonClicked += () =>
                {
                    UiEventHandler.SettingsMenu.Open();
                };

                UiEventHandler.PauseMenu.OnResumeButtonClicked += () =>
                {
                    UiEventHandler.PauseMenu.Close();
                };

                UiEventHandler.PauseMenu.OnQuitButtonClicked += () =>
                {
                    UiEventHandler.ConfirmationMenu.Display(UiEventHandler.BackToMenuConfirmation, ReturnToMainMenu,
                        null);
                };
         
                UiEventHandler.ReviveMenu.OnWatchAdsButtonClicked += () =>
                {
                    OnReviveCharacterRequest?.Invoke();
                    return true;
                };
                UiEventHandler.ReviveMenu.OnCloseButtonClicked += () =>
                {
                    UiEventHandler.SummaryMenu.Open();
                };
                UiEventHandler.ReviveMenu.OnCountDownCompleted += () =>
                {
                    UiEventHandler.SummaryMenu.Open();
                };

                UiEventHandler.ReviveMenu.OnGemButtonClicked += () =>
                {
                    OnRestartLvlRequest?.Invoke();
                    return true;
                };

                UiEventHandler.SummaryMenu.OnMenuOpened += () =>
                {
 
                };

                UiEventHandler.SummaryMenu.OnContinueButtonClicked += () =>
                {
                    OnReturnToMainMenuRequest?.Invoke();
                    UiEventHandler.SummaryMenu.Close();
                };

                onComplete?.Invoke();
            });
        }

        public void Reset()
        {

        }

        private void HandleCurrencyChange(CurrencySO currencySO, bool arg2)
        {
            switch (currencySO.GetKey)
            {
                case CurrencyKeys.Gold:
                    UiEventHandler.GameMenu.UpdateCoinText(currencySO.GetCurrencyAmount);
                    break;
            }
        }

        private void HandleCoinChange(int amount)
        {
            UiEventHandler.GameMenu.UpdateCoinText(amount);
        }

        void HandleMinibossHealthChange(float value)
        {
            UiEventHandler.GameMenu.UpdateBossHealthFill(value);
        }

        void HandleMiniboss(bool isEnabled)
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


        void UpdateGameTimeUI(float timeLeft)
        {
            UiEventHandler.GameMenu.UpdateTimerText(timeLeft);
        }

        private void OnPlayButtonPressed()
        {
            UiEventHandler.MainMenu.Close();
            UiEventHandler.GameMenu.Open();
        }

        public void ReturnToMainMenu()
        {
            OnReturnToMainMenuRequest?.Invoke();
            UiEventHandler.GameMenu.Close();
            UiEventHandler.PauseMenu.Close();
            UiEventHandler.ConfirmationMenu.Close();
        }

        public void OpenPuzzleConfirmation()
        {
            UiEventHandler.ConfirmationMenu.Display(UiEventHandler.PuzzleConfirmation, UiEventHandler.PuzzleMenu.Open,
                null);
        }

        void HandleCharacterHealthChanged(int obj)
        {
        }

        void HandlePlayerDeath()
        {
            UiEventHandler.ReviveMenu.Open();
        }

        void HandleEnemyDamaged(DamageModel damageModel)
        {
            var damageText = NumberConverter.ConvertNumberToString((int)damageModel.Amount);
            var spriteAsset = container.GetDamageTextSprite(damageModel.DamageType);
            var size = damageModel.DamageType == DamageType.NoneCritical ? 60 : 100;
            UiEventHandler.PopupManager.PopUpTextAtTransfrom(damageModel.Target, Vector3.one, damageText,
                spriteAsset,size);
        }

        void HandlePlayerWin()
        {
            UiEventHandler.SummaryMenu.Open();
        }

        void UpdateEnemiesCounter(int enemiesLeft)
        {
            UiEventHandler.GameMenu.UpdateEnemyCountText(enemiesLeft);
        }

        void HandleCharacterDamaged(DamageModel damageModel)
        {
            var damageString = damageModel.DamageType == DamageType.NoneCritical
                ? $"{(int)damageModel.Amount}"
                : $"!!{(int)damageModel.Amount}!!";
            UiEventHandler.PopupManager.PopUpTextAtTransfrom(damageModel.Target, Vector3.one, damageString,
                Color.red);
        }

        void HandleCharacterHeal(float amount, Transform pos)
        {
            //var damageString = $"{(int)amount}";
            //UiEventHandler.PopupManager.PopUpTextAtTransfrom(pos, Vector3.one, damageString,
            //    Color.green);
        }


        private void HandleBoosterActivated(BoosterSO boosterSO, float arg2, Transform transform)
        {
            PopUpTextAtPos($"+{boosterSO.Abreviation} %{arg2}", new Vector2(transform.position.x, transform.position.y + 2) , boosterSO.BoosterColor);
        }

        void PopUpTextAtPos(string info, Vector2 pos, Color color)
        {
            UiEventHandler.PopupManager.PopUpAtTextPosition(pos, new Vector2(0, 1), info,
               color);
        }

        void UpdateComboUI(int count)
        {
            UiEventHandler.GameMenu.UpdateComboCounterText(count);
        }
    }
}