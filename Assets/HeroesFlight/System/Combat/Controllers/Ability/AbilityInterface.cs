using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface AbilityInterface
    {
        bool StopMovementOnUse { get; }
        bool ReadyToUse { get; }
        void UseAbility(float damage, IHealthController target = null, Action onComplete = null);
    }
}