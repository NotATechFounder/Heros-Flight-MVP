using System;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Ability
{
    public class ZoneAttackAbility : AttackAbilityBaseNPC
    {
        [SerializeField] OverlapChecker[] abilityZones;
        private IHealthController healthController;
        private IAttackControllerInterface attackController;

        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            healthController = GetComponentInParent<AiHealthController>();
            currentCooldown = 0;
            foreach (var zone in abilityZones)
            {
                zone.OnDetect += NotifyTargetDetected;
            }
        }

        void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                if (targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    if (canCrit)
                    {
                        bool isCritical = Random.Range(0, 100) <= critChance;

                        float damageToDeal = isCritical
                            ? damage * critModifier
                            : damage;

                        var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                        var damageModel = new HealthModificationIntentModel(damageToDeal,
                            type, AttackType.Regular, CalculationType.Flat, healthController);
                        health.TryDealDamage(damageModel);
                    }
                    else
                    {
                        health.TryDealDamage(new HealthModificationIntentModel(damage,
                            DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat, healthController));
                    }
                }
            }
        }

        public override void UseAbility(Action onComplete = null)
        {
            animator.OnAnimationEvent += HandleAnimationEvents;
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, () =>
                {
                    animator.OnAnimationEvent -= HandleAnimationEvents;
                    onComplete?.Invoke();
                });
                currentCooldown = CoolDown;
            }
        }

        void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            foreach (var zone in abilityZones)
            {
                zone.DetectOverlap();
            }
        }
    }
}