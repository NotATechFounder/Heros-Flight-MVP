using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface ProjecttileControllerInterface
    {
        event Action OnEnded;

        void SetupProjectile(int targetDamage,Vector2 targetDirection);
    }
}