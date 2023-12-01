using System;
using HeroesFlight.Core.StateStack.Enum;
using JetBrains.Annotations;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;


namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class AuthenticationState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.Authentication;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:
                    
                    var dataScene = $"{SceneType.DataScene}";
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, dataScene);
                    progressReporter.SetDone();
                    m_SceneActionsQueue.Start(null, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(dataScene);
                        DataSystemInterface dataSystem = GetService<DataSystemInterface>();
                        dataSystem.Init(loadedScene);
                        AppStateStack.State.Set(ApplicationState.Initialization);
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