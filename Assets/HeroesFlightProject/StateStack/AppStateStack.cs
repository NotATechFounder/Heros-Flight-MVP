using System;
using HeroesFlight.Core.StateStack.Enum;
using StansAssets.Foundation;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;

namespace HeroesFlight.StateStack
{
    public class AppStateStack
    {
        static readonly ApplicationStateStack<ApplicationState> s_State = new ApplicationStateStack<ApplicationState>();

        public static IReadOnlyApplicationStateStack<ApplicationState> State => s_State;


        public void Init(ServiceLocator locator, Action onComplete)
        {
            InitStatesStack(locator);
            onComplete?.Invoke();
        }

        void InitStatesStack(ServiceLocator locator)
        {
            var stateTypes = ReflectionUtility.FindImplementationsOf<IAppState>();
            foreach (var stateType in stateTypes)
            {
                var stateInstance = Activator.CreateInstance(stateType) as IAppState;

                stateInstance.Init(locator);
                s_State.RegisterState(stateInstance.ApplicationState, stateInstance);
            }
        }
    }
}