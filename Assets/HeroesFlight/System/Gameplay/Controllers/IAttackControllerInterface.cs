using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        event Action<IHealthController> OnDealDamageRequest;

        void AttackTarget();

    }
}