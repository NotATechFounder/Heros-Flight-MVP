using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackAbilityBaseNPC : AbilityBaseNPC
    {
        [SerializeField] protected bool canCrit;
        [SerializeField] protected float critModifier = 1f;
        protected float damage;
        protected float critChance;

        public void SetStats(float newDamage, float newCritChance)
        {
            damage = newDamage;
            critChance = newCritChance;
        }
    }
}