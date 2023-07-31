using System;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.UI
{
    public class UiSystem : IUISystem
    {
        public UiSystem(GamePlaySystemInterface gamePlaySystem)
        {
            gameplaySystem = gamePlaySystem;
            gameplaySystem.OnGameStateChange += HandleGameplayStateChange;
            gameplaySystem.OnEnemyDamaged += HandleEnemyDamaged;
            gameplaySystem.OnCharacterHealthChanged += HandleCharacterHealthChanged;
            gameplaySystem.OnRemainingEnemiesLeft += UpdateEnemiesCounter;
            gameplaySystem.OnCharacterDamaged += HandleCharacterDamaged;
            gameplaySystem.OnCharacterComboChanged += UpdateComboUI;
            gameplaySystem.OnMinibossSpawned += HandleMiniboss;
            gameplaySystem.OnMinibossHealthChange += HandleMinibossHealthChange;
        }

        public event Action OnReturnToMainMenuRequest;

        public UIEventHandler UiEventHandler { get; private set; }

        public CountDownTimer GameTimer { get; private set; }

        public const string MainMenuMusicID = "MainMenu";

        public const string GameMusicID = "ForestStart";

        public const string GameMusicLoopID = "ForestLoop";

        GamePlaySystemInterface gameplaySystem;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            UiEventHandler = scene.GetComponent<UIEventHandler>();

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
                        ReturnToMainMenu);
                };


                UiEventHandler.ReviveMenu.OnMenuClosed += () =>
                {
                    UiEventHandler.SummaryMenu.Open();
                };

                UiEventHandler.ReviveMenu.OnWatchAdsButtonClicked += () =>
                {
                    UiEventHandler.ReviveMenu.Close();
                    return true;
                };
                UiEventHandler.ReviveMenu.OnCloseButtonClicked += () =>
                {
                    UiEventHandler.ReviveMenu.Close();
                };

                UiEventHandler.ReviveMenu.OnGemButtonClicked += () =>
                {
                    UiEventHandler.ReviveMenu.Close();

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

        void HandleMinibossHealthChange(float value)
        {
            UiEventHandler.GameMenu.UpdateBossHealthFill(value);
        }

        void HandleGameplayStateChange(GameplayState newState)
        {
            switch (newState)
            {
                case GameplayState.Ongoing:
                    gameplaySystem.GameTimer.OnTimeTick += UpdateGameTimeUI;
                    break;
                case GameplayState.Won:
                    gameplaySystem.GameTimer.OnTimeTick -= UpdateGameTimeUI;
                    HandlePlayerWin();
                    break;
                case GameplayState.Lost:
                    gameplaySystem.GameTimer.OnTimeTick -= UpdateGameTimeUI;
                    HandlePlayerDeath();
                    break;
                case GameplayState.Ended:
                    break;
            }
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

        void HandleEnemyDamaged(Transform transform, int damage)
        {
            UiEventHandler.PopupManager.PopUpTextAtTransfrom(transform, Vector3.one, damage.ToString(),
                Color.yellow);
        }

        void HandlePlayerWin()
        {
            UiEventHandler.SummaryMenu.Open();
        }

        void UpdateEnemiesCounter(int enemiesLeft)
        {
            UiEventHandler.GameMenu.UpdateEnemyCountText(enemiesLeft);
        }

        void HandleCharacterDamaged(Transform transform, int damage)
        {
            UiEventHandler.PopupManager.PopUpTextAtTransfrom(transform, Vector3.one, damage.ToString(),
                Color.red);
        }

        void UpdateComboUI(int count)
        {
            UiEventHandler.GameMenu.UpdateComboCounterText(count);
        }
    }
}