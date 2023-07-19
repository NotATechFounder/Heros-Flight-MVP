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
    public class MainMenuState : BaseApplicationState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.MainMenu;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:
                    progressReporter.SetDone();
                    Debug.Log(ApplicationState);
                    var uiSystem = GetService<IUISystem>();
                    uiSystem.SetUiState(UiSystemState.MainMenu);

                    void StartGameSession()
                    {
                        uiSystem.OnStartGameSessionRequest -= StartGameSession;
                        uiSystem.SetLoaderState(true);
                        CoroutineUtility.WaitForSeconds(3f, () =>
                        {
                            AppStateStack.State.Set(ApplicationState.Gameplay);
                        });
                    }

                    uiSystem.OnStartGameSessionRequest += StartGameSession;

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