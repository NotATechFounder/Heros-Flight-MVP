using UnityEngine;
using HeroesFlight.Core.StateStack.Enum;
using StansAssets.Foundation.Patterns;
using JetBrains.Annotations;
using StansAssets.SceneManagement;
using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Environment;


namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class EnvironmentInitState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.EnvironmentInitialization;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:

                    string environmentScene = $"{SceneType.EnvironmentScene}";
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, environmentScene);
                    m_SceneActionsQueue.Start(null, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(environmentScene);
                        EnvironmentSystemInterface environmentSystem = GetService<EnvironmentSystemInterface>();
                        environmentSystem.Init(loadedScene);

                        DataSystemInterface dataSystem = GetService<DataSystemInterface>();

                        if (dataSystem.TutorialMode)
                        {
                            AppStateStack.State.Set(ApplicationState.Tutorial);
                            GetService<RewardSystemInterface>().SetCurrentState(GameStateType.Tutorial);
                        }
                        else
                        {
                            AppStateStack.State.Set(ApplicationState.MainMenu);
                            GetService<RewardSystemInterface>().SetCurrentState(GameStateType.MainMenu);
                        }

                    });

                    progressReporter.SetDone();
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
