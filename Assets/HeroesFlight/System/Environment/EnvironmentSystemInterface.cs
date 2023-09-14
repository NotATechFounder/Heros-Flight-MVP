
namespace HeroesFlight.System.Environment
{
    public interface EnvironmentSystemInterface : SystemInterface
    {
        public ParticleManager ParticleManager { get; }

        public  BoosterSpawner BoosterSpawner { get;}

        public CurrencySpawner CurrencySpawner { get;}
    }
}