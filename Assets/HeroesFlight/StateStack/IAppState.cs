using HeroesFlight.Core.StateStack.Enum;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;

namespace HeroesFlight.StateStack
{
    public interface IAppState : IApplicationState<ApplicationState>
    {
        ApplicationState ApplicationState { get; }
        void Init(ServiceLocator serviceLocator);
    }
}