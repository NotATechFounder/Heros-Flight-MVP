using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAttackAbilityBase : BossAbilityBase
    {
        [SerializeField] protected float damage;
        

        protected float CalculateDamage()
        {
            return damage + (damage / 100) * modifier;
        }
    }
}

