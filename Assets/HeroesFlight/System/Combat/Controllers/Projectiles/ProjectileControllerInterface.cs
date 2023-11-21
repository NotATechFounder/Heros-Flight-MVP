using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface ProjectileControllerInterface
    {
        public event Action<ProjectileControllerInterface> OnDeactivate;
        event Action<ProjectileControllerInterface> OnHit;

        void SetupProjectile(float targetDamage,Vector2 targetDirection,IHealthController currentOwner=null);
    }
}