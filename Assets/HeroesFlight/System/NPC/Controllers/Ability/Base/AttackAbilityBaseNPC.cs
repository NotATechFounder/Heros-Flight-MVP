using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackAbilityBaseNPC : AbilityBaseNPC
    {
        [Header(" % of npc damage that this ability will deal")] [SerializeField]
        protected float damageMultiplier = 100;
        [SerializeField] protected CalculationType damageType = CalculationType.Flat;
        [Header("Crit parameters")] [SerializeField]
        protected bool canCrit;
        [SerializeField] protected float critModifier = 1f;
        [Header("This values will be added to crit chance")] [SerializeField]
        protected float critChanceModifier;
        [Header("Amount of instances damage will be split between")] [SerializeField]
        protected int damageInstances = 1;
        [SerializeField] protected float betweenInstancesDelay = 0.25f;

        protected float damage;
        protected float critChance;
        private IHealthController healthController;
        
        
        public float SkillDamage => (damage / 100 * damageMultiplier)/damageInstances;
        public float SkillCritChance => critChance + critChanceModifier;

        public void SetStats(float newDamage, float newCritChance)
        {
            Debug.Log($"Damage is {newDamage}");
            damage = newDamage;
            critChance = newCritChance;
            healthController = GetComponentInParent<AiHealthController>();
        }
        protected bool DetermineIfCritical()
        {
            return Random.Range(0, 100) <= SkillCritChance && canCrit;
        }

        protected float CalculateDamage(bool isCritical)
        {
            return isCritical ? SkillDamage * critModifier : SkillDamage;
        }

        protected DamageCritType DetermineCritType(bool isCritical)
        {
            return isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
        }

        protected HealthModificationIntentModel CreateHealthModificationIntentModel(float damageToDeal,
            DamageCritType critType)
        {
            return new HealthModificationIntentModel(damageToDeal, critType, AttackType.Regular, damageType, healthController,damageInstances,betweenInstancesDelay);
        }
        protected void TryDealDamage(IHealthController health)
        {
            var isCritical = DetermineIfCritical();
            var damageToDeal = CalculateDamage(isCritical);
            var damageModel = CreateHealthModificationIntentModel(damageToDeal, DetermineCritType(isCritical));
            // if (damageInstances <= 1)
            // {
            //   
            //     health.TryDealDamage(damageModel);
            // }
            // else
            // {
            //     health.TryDealLineDamage(damageInstances, betweenInstancesDelay, damageModel);
            // }
            health.TryDealDamage(damageModel);
        }
    }
}