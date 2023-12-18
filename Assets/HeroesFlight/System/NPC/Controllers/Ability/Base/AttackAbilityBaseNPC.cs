using HeroesFlight.System.Combat.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackAbilityBaseNPC : AbilityBaseNPC
    {
        [Header(" % of npc damage that this ability will deal")]
        [SerializeField] protected float damageMultiplier = 100;
        [SerializeField] protected CalculationType damageType = CalculationType.Flat;
        [Header("Crit parameters")]
        [SerializeField] protected bool canCrit;
        [SerializeField] protected float critModifier = 1f;
        [Header("This values will be added to crit chance")]
        [SerializeField] protected float critChanceModifier;
        protected float damage;
        protected float critChance;

        public float SkillDamage => damage / 100 * damageMultiplier;
        public float SkillCritChance => critChance + critChanceModifier;

        public void SetStats(float newDamage, float newCritChance)
        {
            damage = newDamage;
            critChance = newCritChance;
        }
    }
}