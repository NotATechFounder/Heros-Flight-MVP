using System;
using System.Collections;
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
                    uiSystem.OnSpecialButtonClicked += HandleSpecialButtonCLicked;

                    uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete += gamePlaySystem.HandleSingleLevelUp;

                    uiSystem.UiEventHandler.PuzzleMenu.OnMenuClosed += ContinueGameLoop;

                    uiSystem.UiEventHandler.AngelPermanetCardMenu.OnMenuClosed += ShowLevelPortal;
                    uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened += gamePlaySystem.StoreRunReward;
                    gamePlaySystem.OnNextLvlLoadRequest += HandleContinueGameLoop;
                    gamePlaySystem.OnGameStateChange += HandleGameStateChanged;

                    void HandleSpecialButtonCLicked()
                    {
                        gamePlaySystem.UseCharacterSpecial();
                    }
                    
                    void HandleGameStateChanged(GameState newState)
                    {
                        switch (newState)
                        {
                            case GameState.Ongoing:
                                break;
                            case GameState.Won:
                                uiSystem.UiEventHandler.SummaryMenu.Open();
                                break;
                            case GameState.Lost:
                                uiSystem.UiEventHandler.ReviveMenu.Open();
                                break;
                            case GameState.Ended:
                                break;
                            case GameState.WaitingPortal:

                                CoroutineUtility.Start(WaitingPortalRoutine());

                                break;
                        }

                        // if (gamePlaySystem.CurrentLvlIndex % 2 == 0)
                        // {
                        //     CoroutineUtility.WaitForSeconds(1f, () =>
                        //     {
                        //         gamePlaySystem.EffectManager.CompletedLevel();
                        //     });
                        // }
                        // else
                        // {
                        //     ShowLevelPortal();
                        // }
                    }

                    IEnumerator WaitingPortalRoutine()
                    {
                        uiSystem.UiEventHandler.GameMenu.DisplayLevelMessage("COMPLETED");

                        yield return new WaitForSeconds(2f);

                        gamePlaySystem.HandleHeroProgression();

                        yield return new WaitUntil(() => uiSystem.UiEventHandler.GameMenu.IsExpComplete && uiSystem.UiEventHandler.HeroProgressionMenu.MenuStatus == UISystem.Menu.Status.Closed);

                        if (!gamePlaySystem.EffectManager.CompletedLevel())
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

                    void ContinueGameLoop()
                    {
                        CoroutineUtility.WaitForSeconds(0.5f, () =>
                        {
                            gamePlaySystem.ResetLogic();
                            npcSystem.Reset();
                            characterSystem.ResetCharacter();
                            characterSystem.SetCharacterControllerState(false);
                            var data = gamePlaySystem.PreloadLvl();
                            gamePlaySystem.StartGameLoop(data);
                        });
                    }

                    void ShowGodBenevolencePrompt()
                    {
                        uiSystem.UiEventHandler.ConfirmationMenu.Display(uiSystem.UiEventHandler.PuzzleConfirmation, uiSystem.UiEventHandler.PuzzleMenu.Open,
                          ContinueGameLoop);
                    }


                    void HandleReturnToMainMenu()
                    {
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.OnReturnToMainMenuRequest -= HandleReturnToMainMenu;
                        uiSystem.OnRestartLvlRequest -= HandleLvlRestart;
                        uiSystem.OnReviveCharacterRequest -= HandleCharacterRevive;

                        uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete -= gamePlaySystem.HandleSingleLevelUp;   

                        uiSystem.UiEventHandler.PuzzleMenu.OnMenuClosed -= ContinueGameLoop;

                        uiSystem.UiEventHandler.AngelPermanetCardMenu.OnMenuClosed -= ShowLevelPortal;
                        uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened -= gamePlaySystem.StoreRunReward;
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

                        uiSystem.UiEventHandler.HeroProgressionMenu.OnUpButtonClickedEvent -= gamePlaySystem.HeroProgression.DecrementAttributeSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnDownButtonClickedEvent -= gamePlaySystem.HeroProgression.IncrementAttributeSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.GetHeroAttributes -= () => gamePlaySystem.HeroProgression.HeroProgressionAttributeInfos;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed -= gamePlaySystem.HeroProgression.Confirm;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnResetButtonPressed -= gamePlaySystem.HeroProgression.ResetSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed -= gamePlaySystem.HeroProgressionCompleted;

                        gamePlaySystem.HeroProgression.OnEXPAdded -= uiSystem.UiEventHandler.GameMenu.UpdateExpBar;
                        gamePlaySystem.HeroProgression.OnLevelUp -= uiSystem.UiEventHandler.GameMenu.UpdateExpBarLevelUp;
                        gamePlaySystem.HeroProgression.OnSpChanged -= uiSystem.UiEventHandler.HeroProgressionMenu.OnSpChanged;
                        uiSystem.UiEventHandler.HeroProgressionMenu.ResetMenu();


                        uiSystem.UiEventHandler.PuzzleMenu.OnPuzzleSolved -= gamePlaySystem.GodsBenevolence.ActivateGodsBenevolence;

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
                        gamePlaySystem.CreateCharacter();
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

                            CoroutineUtility.WaitForSeconds(0.5f, () =>
                            {
                                CoroutineUtility.Start(ContinueGameLoopRoutine());
                            });
                        });
                    }

                    IEnumerator ContinueGameLoopRoutine()
                    {
                        if ((gamePlaySystem.CurrentLvlIndex + 1) % 2 == 0) // Open every second lvl
                        {
                            uiSystem.UiEventHandler.AngelGambitMenu.Open();

                            yield return new WaitUntil(() => uiSystem.UiEventHandler.AngelGambitMenu.MenuStatus == UISystem.Menu.Status.Closed);
                        }

                        if ((gamePlaySystem.CurrentLvlIndex + 1) != gamePlaySystem.MaxLvlIndex) // Open every second lvl
                        {
                            ShowGodBenevolencePrompt();
                        }
                        else
                        {
                            ContinueGameLoop();
                        }
                    }

                    uiSystem.UiEventHandler.MainMenu.Close();
                    uiSystem.UiEventHandler.LoadingMenu.Open();
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, gameScene);
                    m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(gameScene);
                        SceneManager.SetActiveScene(loadedScene);
                        characterSystem.Init(loadedScene);
                        characterSystem.SetCurrentCharacterType(uiSystem.UiEventHandler.CharacterSelectionMenu.selectedType);
                        npcSystem.Init(loadedScene);
                        gamePlaySystem.Init(loadedScene, () =>
                        {
                            gamePlaySystem.EffectManager.OnPermanetCard += uiSystem.UiEventHandler.AngelPermanetCardMenu
                                .AcivateCardPermanetEffect;
                            uiSystem.UiEventHandler.AngelGambitMenu.CardExit += gamePlaySystem.EffectManager.Exists;
                            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += gamePlaySystem.EffectManager.AddAngelCardSO;

                            uiSystem.UiEventHandler.HeroProgressionMenu.GetHeroAttributes += () => gamePlaySystem.HeroProgression.HeroProgressionAttributeInfos;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnUpButtonClickedEvent += gamePlaySystem.HeroProgression.DecrementAttributeSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnDownButtonClickedEvent += gamePlaySystem.HeroProgression.IncrementAttributeSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed += gamePlaySystem.HeroProgression.Confirm;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnResetButtonPressed += gamePlaySystem.HeroProgression.ResetSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed += gamePlaySystem.HeroProgressionCompleted;

                            gamePlaySystem.HeroProgression.OnEXPAdded += uiSystem.UiEventHandler.GameMenu.UpdateExpBar;
                            gamePlaySystem.HeroProgression.OnLevelUp += uiSystem.UiEventHandler.GameMenu.UpdateExpBarLevelUp;
                            gamePlaySystem.HeroProgression.OnSpChanged += uiSystem.UiEventHandler.HeroProgressionMenu.OnSpChanged;
        

                           uiSystem.UiEventHandler.PuzzleMenu.OnPuzzleSolved += gamePlaySystem.GodsBenevolence.ActivateGodsBenevolence;
                        });
                        uiSystem.UiEventHandler.GameMenu.Open();
                        CoroutineUtility.WaitForSeconds(1f, () => // Run the first time the game is loaded
                        {
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            gamePlaySystem.CreateCharacter();
                            ContinueGameLoop();

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