using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UnityEngine;

namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class ApplicationInitState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.Initialization;

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
                        CoroutineUtility.WaitForEndOfFrame(() =>
                        {
                            GetService<IUISystem>().Init(loadedScene, () =>
                            {
                                AppStateStack.State.Set(ApplicationState.MainMenu);
                            });
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