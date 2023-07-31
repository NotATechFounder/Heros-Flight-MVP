using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface ProjecttileControllerInterface
    {
        event Action OnEnded;

        void SetupProjectile(int targetDamage,Transform currentTarget,Vector2 targetDirection);
    }
}