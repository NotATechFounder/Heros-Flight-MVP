using System;
using HeroesFlight.System.Gameplay.Enum;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        public event Action OnHitTarget;
        event Action<AttackControllerState> OnStateChange; 
        float Damage { get; }
        float TimeSinceLastAttack { get; }
        void AttackTargets();
        void Init();
        void ToggleControllerState(bool isEnabled);
    }
}