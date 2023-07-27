using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Gameplay;
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
                    
                    void HandleReturnToMainMenu()
                    {
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.OnReturnToMainMenuRequest -= HandleReturnToMainMenu;
                        uiSystem.UiEventHandler.LoadingMenu.Open();
                        m_SceneActionsQueue.AddAction(SceneActionType.Unload, gameScene);
                        gamePlaySystem.Reset();
                        m_SceneActionsQueue.Start( uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                        {
                            uiSystem.UiEventHandler.MainMenu.Open();
                            uiSystem.UiEventHandler.GameMenu.Close();
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            AppStateStack.State.Set(ApplicationState.MainMenu);
                        });
                        
                    }

                    uiSystem.OnReturnToMainMenuRequest += HandleReturnToMainMenu;
                    uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked += HandleReturnToMainMenu;
                    
                    
                    uiSystem.UiEventHandler.MainMenu.Close();
                    uiSystem.UiEventHandler.LoadingMenu.Open();
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, gameScene);
                    m_SceneActionsQueue.Start( uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(gameScene);
                        SceneManager.SetActiveScene(loadedScene);
                        npcSystem.Init(loadedScene);
                        gamePlaySystem.Init(loadedScene);
                        uiSystem.UiEventHandler.GameMenu.Open();
                        CoroutineUtility.WaitForSeconds(1f,() =>
                        {
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            gamePlaySystem.StartGameLoop();     
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