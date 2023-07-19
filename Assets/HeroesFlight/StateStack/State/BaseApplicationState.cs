using HeroesFlight.Core.StateStack.Enum;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;

namespace HeroesFlight.StateStack.State
{
    public abstract class BaseApplicationState : IApplicationState<ApplicationState>
    {
        ServiceLocator m_Locator;

        protected void InitLocator(ServiceLocator locator)
        {
            m_Locator = locator;
        }

        protected T GetService<T>() => m_Locator.Get<T>();

        public abstract void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter);
    }
}