using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface AbilityInterface
    {
        float CoolDown { get; }
        bool StopMovementOnUse { get; }
        bool ReadyToUse { get; }
        void UseAbility(Action onComplete = null);
    }
}