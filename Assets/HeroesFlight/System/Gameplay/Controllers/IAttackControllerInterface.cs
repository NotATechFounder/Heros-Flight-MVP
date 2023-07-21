using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        event Action<IAttackControllerInterface,IHealthController> OnDealDamageRequest;

        int Damage { get; }
        float TimeSinceLastAttack { get; }

        void AttackTarget();

    }
}