using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
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
                    var dataSystem = GetService<DataSystemInterface>();
                    uiSystem.OnReturnToMainMenuRequest += HandleReturnToMainMenu;
                    uiSystem.OnRestartLvlRequest += HandleLvlRestart;
                    uiSystem.OnReviveCharacterRequest += HandleCharacterRevive;
                    uiSystem.OnSpecialButtonClicked += HandleSpecialButtonCLicked;

                    uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete += gamePlaySystem.HandleSingleLevelUp;

                    uiSystem.UiEventHandler.PuzzleMenu.OnMenuClosed += ContinueGameLoop;

                    uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened += gamePlaySystem.StoreRunReward;
                    uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked += HandleGameLoopFinish;
                    uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted += HandleGameLoopFinish;
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

                                dataSystem.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,
                                    CharacterType.Storm));
                                dataSystem.UnlockHero(CharacterType.Storm);
                                Debug.Log("Granting STORM");

                                HandleGameLoopFinish();
                                break;
                            case GameState.Died:
                               uiSystem.UiEventHandler.ReviveMenu.Open();
                                break;
                            case GameState.Ended:
                                break;
                            case GameState.WaitingPortal:

                                Debug.Log(gamePlaySystem.CurrentLvlIndex);
                                if (gamePlaySystem.CurrentLvlIndex == 3)
                                {
                                    dataSystem.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,
                                        CharacterType.Lancer));
                                    dataSystem.UnlockHero(CharacterType.Lancer);
                                    Debug.Log("Granting LANCER");
                                }

                                // if (gamePlaySystem.CurrentLvlIndex == 7)
                                // {
                                //     dataSystem.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,
                                //         CharacterType.Storm));
                                //     dataSystem.UnlockHero(CharacterType.Storm);
                                //     Debug.Log("Granting STORM");
                                // }


                                CoroutineUtility.Start(WaitingPortalRoutine());
                                break;

                            case GameState.TimeEnded:
                                uiSystem.UiEventHandler.GameMenu.DisplayLevelMessage("TIME ENDED");
                                CoroutineUtility.WaitForSeconds(1f, HandleGameLoopFinish);
                                break;
                        }
                    }

                    IEnumerator WaitingPortalRoutine()
                    {
                        uiSystem.UiEventHandler.GameMenu.DisplayLevelMessage("COMPLETED");

                        yield return new WaitForSeconds(2f);

                        gamePlaySystem.HandleHeroProgression();

                        yield return new WaitUntil(() =>
                            uiSystem.UiEventHandler.GameMenu.IsExpComplete &&
                            uiSystem.UiEventHandler.HeroProgressionMenu.MenuStatus == UISystem.Menu.Status.Closed);

                        yield return new WaitForSeconds(1f);

                        if (gamePlaySystem.EffectManager.CompletedLevel())
                        {
                            yield return new WaitUntil(() =>
                                uiSystem.UiEventHandler.AngelPermanetCardMenu.MenuStatus ==
                                UISystem.Menu.Status.Closed);
                        }

                        ShowLevelPortal();
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
                            npcSystem.Reset();
                            characterSystem.ResetCharacter(gamePlaySystem.GetPlayerSpawnPosition);
                            characterSystem.SetCharacterControllerState(false);
                            gamePlaySystem.StartGameLoop();
                        });
                    }

                    void ShowGodBenevolencePrompt()
                    {
                        uiSystem.UiEventHandler.ConfirmationMenu.Display(uiSystem.UiEventHandler.PuzzleConfirmation,
                            uiSystem.UiEventHandler.PuzzleMenu.Open,
                            ContinueGameLoop);
                    }
                    
                    void HandleGameLoopFinish()
                    {
                        if (dataSystem.RewardHandler.RewardPending)
                        {
                            var pendingRewards = dataSystem.RewardHandler.GetPendingRewards();
                            var rewardsToConsume = new List<RewardModel>();

                            if (pendingRewards.TryGetValue(RewardType.Hero, out var rewards))
                            {
                                foreach (var reward in rewards)
                                {
                                    if (reward.RewardType == RewardType.Hero)
                                    {
                                        var heroReward = reward as HeroRewardModel;
                                        rewardsToConsume.Add(reward);
                                        uiSystem.UiEventHandler.SummaryMenu.AddRewardEntry(
                                            $"Unlocked new Hero - {heroReward.HeroType}");
                                    }
                                }
                            }


                            foreach (var reward in rewardsToConsume)
                            {
                                dataSystem.RewardHandler.ConsumeReward(reward);
                            }
                        }

                        uiSystem.UiEventHandler.SummaryMenu.Open();
                    }
                    void HandleReturnToMainMenu()
                    {
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.OnReturnToMainMenuRequest -= HandleReturnToMainMenu;
                        uiSystem.OnRestartLvlRequest -= HandleLvlRestart;
                        uiSystem.OnReviveCharacterRequest -= HandleCharacterRevive;

                        uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete -= gamePlaySystem.HandleSingleLevelUp;

                        uiSystem.UiEventHandler.PuzzleMenu.OnMenuClosed -= ContinueGameLoop;

                        uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened -= gamePlaySystem.StoreRunReward;

                        uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked -= HandleGameLoopFinish;
                        uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted -= HandleGameLoopFinish;

                        gamePlaySystem.OnNextLvlLoadRequest -= HandleContinueGameLoop;
                        gamePlaySystem.OnGameStateChange -= HandleGameStateChanged;
                        uiSystem.UiEventHandler.LoadingMenu.Open();
                        uiSystem.UiEventHandler.GameMenu.Close();
                        m_SceneActionsQueue.AddAction(SceneActionType.Unload, gameScene);
                        gamePlaySystem.Reset();

                        gamePlaySystem.EffectManager.OnTrigger -= uiSystem.UiEventHandler.AngelGambitMenu.Open;
                        gamePlaySystem.EffectManager.OnPermanetCard -= uiSystem.UiEventHandler.AngelPermanetCardMenu .AcivateCardPermanetEffect;
                        uiSystem.UiEventHandler.AngelGambitMenu.CardExit -= gamePlaySystem.EffectManager.Exists;
                        uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -=  gamePlaySystem.EffectManager.AddAngelCardSO;
                        uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= EnableMovement;
                        uiSystem.UiEventHandler.AngelPermanetCardMenu.ResetMenu();

                        uiSystem.UiEventHandler.HeroProgressionMenu.OnUpButtonClickedEvent -=
                            gamePlaySystem.HeroProgression.DecrementAttributeSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnDownButtonClickedEvent -=
                            gamePlaySystem.HeroProgression.IncrementAttributeSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.GetHeroAttributes -= () =>
                            gamePlaySystem.HeroProgression.HeroProgressionAttributeInfos;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed -=
                            gamePlaySystem.HeroProgression.Confirm;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnResetButtonPressed -=
                            gamePlaySystem.HeroProgression.ResetSP;
                        uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed -=
                            gamePlaySystem.HeroProgressionCompleted;

                        gamePlaySystem.HeroProgression.OnEXPAdded -= uiSystem.UiEventHandler.GameMenu.UpdateExpBar;
                        gamePlaySystem.HeroProgression.OnLevelUp -=
                            uiSystem.UiEventHandler.GameMenu.UpdateExpBarLevelUp;
                        gamePlaySystem.HeroProgression.OnSpChanged -=
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnSpChanged;
                        uiSystem.UiEventHandler.HeroProgressionMenu.ResetMenu();


                        uiSystem.UiEventHandler.PuzzleMenu.OnPuzzleSolved -=
                            gamePlaySystem.GodsBenevolence.ActivateGodsBenevolence;

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
                        gamePlaySystem.StartGameLoop();
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
                            System.NPC.Model.Level newLevel = null;
                            uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level to level transition
                                {
                                    gamePlaySystem.ResetLogic();
                                    npcSystem.Reset();

                                    newLevel = gamePlaySystem.PreloadLvl();
                                    Debug.Log(newLevel==null);
                                    characterSystem.ResetCharacter(gamePlaySystem.GetPlayerSpawnPosition);
                                    characterSystem.SetCharacterControllerState(false);
                                },
                                () =>
                                {
                                    Debug.Log(newLevel==null);
                                    if (newLevel.LevelType == System.NPC.Model.LevelType.Combat)
                                    {
                                        CoroutineUtility.Start(ContinueGameLoopRoutine());
                                    }
                                    else
                                    {
                                        characterSystem.SetCharacterControllerState(true);
                                    }
                                });
                        });
                    }

                    IEnumerator ContinueGameLoopRoutine()
                    {
                        yield return new WaitForSeconds(0.5f);

                        if (gamePlaySystem.CurrentLvlIndex  != gamePlaySystem.MaxLvlIndex) // Open every second lvl
                        {
                            ShowGodBenevolencePrompt();
                        }
                        else
                        {
                            ContinueGameLoop();
                        }
                    }

                    void EnableMovement()
                    {
                        characterSystem.SetCharacterControllerState(true);
                    }

                    uiSystem.UiEventHandler.MainMenu.Close();
                    uiSystem.UiEventHandler.LoadingMenu.Open();
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, gameScene);
                    m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(gameScene);
                        SceneManager.SetActiveScene(loadedScene);
                        characterSystem.Init(loadedScene);
                        characterSystem.SetCurrentCharacterType(uiSystem.UiEventHandler.CharacterSelectionMenu
                            .selectedType);
                        npcSystem.Init(loadedScene);
                        gamePlaySystem.Init(loadedScene, () =>
                        {
                            gamePlaySystem.EffectManager.OnTrigger += uiSystem.UiEventHandler.AngelGambitMenu.Open;
                            gamePlaySystem.EffectManager.OnPermanetCard += uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
                            uiSystem.UiEventHandler.AngelGambitMenu.CardExit += gamePlaySystem.EffectManager.Exists;
                            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += gamePlaySystem.EffectManager.AddAngelCardSO;
                            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += EnableMovement;

                            uiSystem.UiEventHandler.HeroProgressionMenu.GetHeroAttributes += () =>
                                gamePlaySystem.HeroProgression.HeroProgressionAttributeInfos;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnUpButtonClickedEvent +=
                                gamePlaySystem.HeroProgression.DecrementAttributeSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnDownButtonClickedEvent +=
                                gamePlaySystem.HeroProgression.IncrementAttributeSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed +=
                                gamePlaySystem.HeroProgression.Confirm;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnResetButtonPressed +=
                                gamePlaySystem.HeroProgression.ResetSP;
                            uiSystem.UiEventHandler.HeroProgressionMenu.OnCloseButtonPressed +=
                                gamePlaySystem.HeroProgressionCompleted;

                            gamePlaySystem.HeroProgression.OnEXPAdded += uiSystem.UiEventHandler.GameMenu.UpdateExpBar;
                            gamePlaySystem.HeroProgression.OnLevelUp +=
                                uiSystem.UiEventHandler.GameMenu.UpdateExpBarLevelUp;
                            gamePlaySystem.HeroProgression.OnSpChanged +=
                                uiSystem.UiEventHandler.HeroProgressionMenu.OnSpChanged;


                            uiSystem.UiEventHandler.PuzzleMenu.OnPuzzleSolved +=
                                gamePlaySystem.GodsBenevolence.ActivateGodsBenevolence;
                        });
                        uiSystem.UiEventHandler.GameMenu.Open();
                        CoroutineUtility.WaitForSeconds(1f, () => // Run the first time the game is loaded
                        {
                            uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level transition
                            {
                                uiSystem.UiEventHandler.LoadingMenu.Close();
                                gamePlaySystem.ResetLogic();
                                gamePlaySystem.PreloadLvl();
                                gamePlaySystem.CreateCharacter();                             
                            }
                            ,ContinueGameLoop);          
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