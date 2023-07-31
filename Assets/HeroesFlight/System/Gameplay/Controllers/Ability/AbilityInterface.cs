using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface AbilityInterface
    {
        bool ReadyToUse { get; }
        void UseAbility(IHealthController target=null,Action onComplete=null);
    }
}