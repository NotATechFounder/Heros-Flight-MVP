using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Inventory;
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

                    IAuthenticationInterface authentication = GetService<IAuthenticationInterface>();
                    progressReporter.SetDone();

                    var authenticationScene = $"{SceneType.AuthenticationScene}";
                    m_SceneActionsQueue.AddAction(SceneActionType.Load, authenticationScene);

                    m_SceneActionsQueue.Start(null, () =>
                    {
                        var loadedScene = m_SceneActionsQueue.GetLoadedScene(authenticationScene);
                        authentication.Init(loadedScene);
                        authentication.LL_Authentication.OnLoginComplected += LoginSuccessFul;
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

        public void LoginSuccessFul(LoginMode loginMode)
        {
            Debug.Log("LoginSuccessFul");

            var dataScene = $"{SceneType.DataScene}";
            m_SceneActionsQueue.AddAction(SceneActionType.Load, dataScene);

            m_SceneActionsQueue.Start(null, () =>
            {
                var loadedScene = m_SceneActionsQueue.GetLoadedScene(dataScene);
                DataSystemInterface dataSystem = GetService<DataSystemInterface>();
                dataSystem.Init(loadedScene);
                InventorySystemInterface inventorySystem = GetService<InventorySystemInterface>();
                inventorySystem.Init(loadedScene);
                RewardSystemInterface rewardSystem = GetService<RewardSystemInterface>();
                rewardSystem.Init(loadedScene);
                IShopSystemInterface shopSystem = GetService<IShopSystemInterface>();
                shopSystem.Init(loadedScene);
                IAchievementSystemInterface achievementSystemInterface = GetService<IAchievementSystemInterface>();
                achievementSystemInterface.Init(loadedScene);
                AppStateStack.State.Set(ApplicationState.Initialization);

            });
        }
    }
}