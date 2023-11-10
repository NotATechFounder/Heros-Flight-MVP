using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;


namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class UiInitState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.UiInitialization;

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
                    var uiScene = $"{SceneType.UIScene}";
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, uiScene);
                    m_SceneActionsQueue.Start(null, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(uiScene);
                        IUISystem uiSystem = GetService<IUISystem>();
                        EnvironmentSystemInterface environmentSystem = GetService<EnvironmentSystemInterface>();
                        Debug.Log("Initing environment system");
                        environmentSystem.Init(loadedScene);
                        uiSystem.Init(loadedScene);
                        AppStateStack.State.Set(ApplicationState.MainMenu);
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