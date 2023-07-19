using HeroesFlight.Core.Bootstrapper;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.StateStack;

namespace HeroesFlight.Core.Application
{
    internal class HeroesFlightApplication : IApplication
    {
        public void Start(IBootstrapper monoBootstrapper)
        {
            var stateStack = new AppStateStack();
            stateStack.Init(monoBootstrapper.ResolveServices(), () =>
            {
                AppStateStack.State.Set(ApplicationState.Initialization);
            });
        }
    }
}