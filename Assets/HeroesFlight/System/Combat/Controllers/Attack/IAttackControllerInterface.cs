using System;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        public event Action OnHitTarget;
        float Damage { get; }
        float TimeSinceLastAttack { get; }
        void AttackTargets();
        void Init();
        void ToggleControllerState(bool isEnabled);
    }
}