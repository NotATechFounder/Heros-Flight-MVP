using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Enum;
using JetBrains.Annotations;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;

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
                    var gameScene = $"{SceneType.GameScene}";
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, gameScene);
                    m_SceneActionsQueue.Start(null, () =>
                    {
                        var uiSystem = GetService<IUISystem>();
                        uiSystem.SetUiState(UiSystemState.Gameplay);

                        void ReturnTomMinMenu()
                        {
                            Debug.Log("Return to main menu request");
                            uiSystem.OnReturnToMainMenuRequest -= ReturnTomMinMenu;
                            uiSystem.SetLoaderState(true);
                            var sceneToRemove = $"{SceneType.GameScene}";
                            m_SceneActionsQueue.AddAction(SceneActionType.Unload, sceneToRemove);
                            m_SceneActionsQueue.Start(null, () =>
                            {
                                CoroutineUtility.WaitForSeconds(3f, () =>
                                {
                                    AppStateStack.State.Set(ApplicationState.MainMenu);
                                });
                            });
                        }

                        uiSystem.OnReturnToMainMenuRequest += ReturnTomMinMenu;
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