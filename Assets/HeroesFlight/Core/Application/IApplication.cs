using HeroesFlight.Core.Bootstrapper;

namespace HeroesFlight.Core.Application
{
    public interface IApplication
    {
        void Start(IBootstrapper monoBootstrapper);
    }
}