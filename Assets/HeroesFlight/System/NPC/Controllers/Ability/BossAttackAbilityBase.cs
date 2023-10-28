using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAttackAbilityBase : BossAbilityBase
    {
        [SerializeField] protected float damage;
        [SerializeField] protected bool canCrit;
        [Range(0,100)]
        [SerializeField] protected float critChance;

        [SerializeField] protected float critModifier = 1f;
        

        protected float CalculateDamage()
        {
           return damage + (damage / 100) * modifier;
        }
    }
}

