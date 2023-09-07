using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface ProjectileControllerInterface
    {
        event Action<ProjectileControllerInterface> OnEnded;

        void SetupProjectile(float targetDamage,Vector2 targetDirection);
    }
}