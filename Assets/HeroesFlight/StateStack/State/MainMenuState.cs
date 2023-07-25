using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;

namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class MainMenuState : BaseApplicationLoadSceneState, IAppState
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
                    Debug.Log(ApplicationState);
                    progressReporter.SetDone();
                    var uiSystem = GetService<IUISystem>();

                    uiSystem.UiEventHandler.MainMenu.Open();

                    void HandleGameStartRequest()
                    {
                        uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed -= HandleGameStartRequest;
                        AppStateStack.State.Set(ApplicationState.Gameplay);
                    }

                    uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed += HandleGameStartRequest;
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