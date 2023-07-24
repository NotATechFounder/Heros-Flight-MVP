using StansAssets.Foundation.Patterns;

namespace HeroesFlight.Core.Bootstrapper
{
    public interface IBootstrapper
    {
        ServiceLocator ResolveServices();
    }   
}

