using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.NPC;
using HeroesFlight.System.Stats;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
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
                    var progressionSystem = GetService<ProgressionSystemInterface>();
                    var inputSystem = GetService<InputSystemInterface>();
                    var combatSystem = GetService<CombatSystemInterface>();
                    uiSystem.OnReturnToMainMenuRequest += HandleReturnToMainMenu;


                    void HandleReturnToMainMenu()
                    {
                        uiSystem.UiEventHandler.PauseMenu.OnQuitButtonClicked -= HandleReturnToMainMenu;
                        uiSystem.OnReturnToMainMenuRequest -= HandleReturnToMainMenu;

                        uiSystem.UiEventHandler.LoadingMenu.Open();
                        uiSystem.UiEventHandler.GameMenu.Close();
                        m_SceneActionsQueue.AddAction(SceneActionType.Unload, gameScene);
                        gamePlaySystem.Reset();
                        progressionSystem.Reset();
                        combatSystem.Reset();
                        uiSystem.UiEventHandler.HeroProgressionMenu.ResetMenu();


                        m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
                        {
                            uiSystem.UiEventHandler.MainMenu.Open();
                            uiSystem.UiEventHandler.GameMenu.Close();
                            uiSystem.UiEventHandler.LoadingMenu.Close();
                            AppStateStack.State.Set(ApplicationState.MainMenu);
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
                        progressionSystem.Init(loadedScene);
                        characterSystem.SetCurrentCharacterType(dataSystem.CharacterManager.SelectedCharacter
                            .CharacterType);
                        npcSystem.Init(loadedScene);
                        inputSystem.Init(loadedScene);
                        combatSystem.Init(loadedScene);
                        gamePlaySystem.Init(loadedScene);
                        gamePlaySystem.StartGameSession();
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