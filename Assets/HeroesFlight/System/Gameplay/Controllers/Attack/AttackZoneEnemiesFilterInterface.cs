using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface AttackZoneEnemiesFilterInterface
    {
        void FilterEnemies(Vector2 attackPoint,bool characterFacingLeft,List<IHealthController> enemies, ref List<IHealthController> enemiesToUpdate,
            AttackAnimationEvent dataAttackType);
    }
}