using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.NPC;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class GameplayState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.Gameplay;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:
                    Debug.Log(ApplicationState);

                    progressReporter.SetDone();
                    var uiSystem = GetService<IUISystem>();
                    var gamePlaySystem = GetService<GamePlaySystemInterface>();
                    var npcSystem = GetService<NpcSystemInterface>();
                    var gameScene = $"{SceneType.GameScene}";
                    var characterSystem = GetService<CharacterSystemInterface>();
                    uiSystem.OnReturnToMainMenuRequest += HandleReturnToMainMenu;
                    uiSystem.OnRestartLvlRequest += HandleLvlRestart;
                    uiSystem.OnReviveCharacterRequest += HandleCharacterRevive;
                    uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked += HandleReturnToMainMenu;
                    uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += HandleAngelsGambitClosed;
                    uiSystem.UiEventHandler.AngelPermanetCardMenu.OnMenuClosed += ShowLevelPortal;
                    gamePlaySystem.OnNextLvlLoadRequest += HandleContinueGameLoop;
                    gamePlaySystem.OnGameStateChange += HandleGameStateChanged;


                    void HandleGameStateChanged(GameState newState)
                    {
                        if (newState != GameState.WaitingPortal)
                            return;

                        if (gamePlaySystem.CurrentLvlIndex % 2 == 0)
                        {
                            CoroutineUtility.WaitForSeconds(1f, () =>
                            {
                                gamePlaySystem.EffectManager.CompletedLevel();
                            });
                        }
                        else
                        {
                            ShowLevelPortal();
                        }
                    }

                    void ShowLevelPortal()
                    {
                        CoroutineUtility.WaitForSeconds(1f, () =>
                        {
                            gamePlaySystem.EnablePortal();
                        });
                    }

                    void HandleAngelsGambitClosed()
                    {
                        CoroutineUtility.WaitForSeconds(0.5f, () =>
                        {
                            gamePlaySystem.ResetLogic();
                            npcSystem.Reset();
                            characterSystem.ResetCharacter();
                            characterSystem.SetCharacterControllerState(false);
                            var data = gamePlaySystem.PreloadLvl();
                            gamePlaySystem.ContinueGameLoop(data);
                        });
                    }


                    void HandleReturnToMainMenu()
                    {
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.OnReturnToMainMenuRequest -= HandleReturnToMainMenu;
                        uiSystem.OnRestartLvlRequest -= HandleLvlRestart;
                        uiSystem.OnReviveCharacterRequest -= HandleCharacterRevive;
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= HandleAngelsGambitClosed;
                        uiSystem.UiEventHandler.AngelPermanetCardMenu.OnMenuClosed -= ShowLevelPortal;
                        gamePlaySystem.OnNextLvlLoadRequest -= HandleContinueGameLoop;
                        gamePlaySystem.OnGameStateChange -= HandleGameStateChanged;
                        uiSystem.UiEventHandler.LoadingMenu.Open();
                        uiSystem.UiEventHandler.GameMenu.Close();
                        m_SceneActionsQueue.AddAction(SceneActionType.Unload, gameScene);
                        gamePlaySystem.Reset();
                        gamePlaySystem.EffectManager.OnPermanetCard -= uiSystem.UiEventHandler.AngelPermanetCardMenu
                            .AcivateCardPermanetEffect;
                        uiSystem.UiEventHandler.AngelGambitMenu.CardExit -= gamePlaySystem.EffectManager.Exists;
                        uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -=
                            gamePlaySystem.EffectManager.AddAngelCardSO;
                        uiSystem.UiEventHandler.AngelPermanetCardMenu.ResetMenu();
                        m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                        {
                            uiSystem.UiEventHandler.MainMenu.Open();
                            uiSystem.UiEventHandler.GameMenu.Close();
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            AppStateStack.State.Set(ApplicationState.MainMenu);
                        });
                    }

                    void HandleLvlRestart()
                    {
                        characterSystem.Reset();
                        npcSystem.Reset();
                        gamePlaySystem.Reset();
                        uiSystem.UiEventHandler.ReviveMenu.Close();
                        uiSystem.UiEventHandler.AngelPermanetCardMenu.ResetMenu();
                        gamePlaySystem.StartGameLoop(gamePlaySystem.PreloadLvl());
                    }

                    void HandleCharacterRevive()
                    {
                        gamePlaySystem.ReviveCharacter();
                        uiSystem.UiEventHandler.ReviveMenu.Close();
                    }

                    void HandleContinueGameLoop()
                    {
                        CoroutineUtility.WaitForSeconds(0.5f, () =>
                        {
                            gamePlaySystem.ResetLogic();
                            npcSystem.Reset();
                            characterSystem.ResetCharacter();
                            characterSystem.SetCharacterControllerState(false);
                            if (gamePlaySystem.CurrentLvlIndex % 2 != 0)
                            {
                                uiSystem.UiEventHandler.AngelGambitMenu.Open();
                            }
                            else
                            {
                                var data = gamePlaySystem.PreloadLvl();
                                gamePlaySystem.ContinueGameLoop(data);
                            }
                        });
                    }


                    uiSystem.UiEventHandler.MainMenu.Close();
                    uiSystem.UiEventHandler.LoadingMenu.Open();
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, gameScene);
                    m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(gameScene);
                        SceneManager.SetActiveScene(loadedScene);
                        characterSystem.Init(loadedScene);
                        npcSystem.Init(loadedScene);
                        gamePlaySystem.Init(loadedScene, () =>
                        {
                            gamePlaySystem.EffectManager.OnPermanetCard += uiSystem.UiEventHandler.AngelPermanetCardMenu
                                .AcivateCardPermanetEffect;
                            uiSystem.UiEventHandler.AngelGambitMenu.CardExit += gamePlaySystem.EffectManager.Exists;
                            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected +=
                                gamePlaySystem.EffectManager.AddAngelCardSO;
                        });
                        uiSystem.UiEventHandler.GameMenu.Open();
                        CoroutineUtility.WaitForSeconds(1f, () =>
                        {
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            var data = gamePlaySystem.PreloadLvl();

                            gamePlaySystem.StartGameLoop(data);
                        });
                    });

                    break;
                case StackAction.Paused:
                    break;
                case StackAction.Resumed:
                    break;
                case StackAction.Removed:
                    progressReporter.SetDone();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evt.Action), evt.Action, null);
            }
        }
    }
}