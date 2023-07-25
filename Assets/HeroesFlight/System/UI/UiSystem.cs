using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using StansAssets.Foundation.Extensions;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.UI
{
    public class UiSystem : IUISystem
    {
        public UIEventHandler UiEventHandler { get; private set; }
        public CountDownTimer GameTimer { get; private set; }

        public const string MainMenuMusicID = "MainMenu";
        public const string GameMusicID = "ForestStart";
        public const string GameMusicLoopID = "ForestLoop";

        public void Init(Scene scene = default, Action onComplete = null)
        {
            UiEventHandler = scene.GetComponent<UIEventHandler>();

            GameTimer = new CountDownTimer(UiEventHandler);

            UiEventHandler.Init(onComplete);

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
                UiEventHandler.ConfirmationMenu.Display(UiEventHandler.BackToMenuConfirmation, ReturnToMainMenu, null);
            };


            UiEventHandler.ReviveMenu.OnWatchAdsButtonClicked += () =>
            {
                return true;
            };

            UiEventHandler.ReviveMenu.OnGemButtonClicked += () =>
            {
                return true;
            };

            UiEventHandler.SummaryMenu.OnContinueButtonClicked += () =>
            {

            };
        }

        public void Reset()
        {

        }

        private void OnPlayButtonPressed()
        {
            UiEventHandler.MainMenu.Close();
            UiEventHandler.GameMenu.Open();

            GameTimer.Start(3, (time) =>
            {
                UiEventHandler.GameMenu.UpdateTimerText(time);
            },
            () =>
            {
                GameTimer.Start(200, (time) =>
                {
                    UiEventHandler.GameMenu.UpdateTimerText(time);
                },
              () =>
              {
                  Debug.Log("Game Time Lapse");
              });
            });

        }

        public void ReturnToMainMenu()
        {
            UiEventHandler.GameMenu.Close();
            UiEventHandler.PauseMenu.Close();
            UiEventHandler.MainMenu.Open();
        }

        public void OpenPuzzleConfirmation()
        {
            UiEventHandler.ConfirmationMenu.Display(UiEventHandler.PuzzleConfirmation, UiEventHandler.PuzzleMenu.Open, null);
        }
    }
}