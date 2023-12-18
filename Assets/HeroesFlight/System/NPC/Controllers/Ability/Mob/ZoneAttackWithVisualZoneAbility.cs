using System;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class ZoneAttackWithVisualZoneAbility : AttackAbilityBaseNPC
    {
        
        [SerializeField] AbilityZone[] abilityZones;
        [SerializeField] float preDamageDelay;
       
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            currentCooldown = 0;
            foreach (var zone in abilityZones)
            {
                zone.ZoneChecker.OnDetect += NotifyTargetDetected;
            }
        }

        void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            
            for (int i = 0; i < count; i++)
            {
                if(targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    if (canCrit)
                    {
                        bool isCritical = Random.Range(0, 100) <= SkillCritChance;

                        float damageToDeal = isCritical
                            ? SkillDamage * critModifier
                            : SkillDamage;

                        var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                        var damageModel = new HealthModificationIntentModel(damageToDeal, 
                            type, AttackType.Regular,damageType,null);
                        health.TryDealDamage(damageModel);
                    }
                    else
                    {
                        health.TryDealDamage(new HealthModificationIntentModel(SkillDamage,
                            DamageCritType.NoneCritical,AttackType.Regular,damageType,null));      
                    }
                  
                }
            }
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation,onComplete);
            }

            foreach (var zone in abilityZones)
            {
                zone.ZoneVisual.Trigger(() =>
                {
                   zone.ZoneChecker.DetectOverlap();
                },preDamageDelay,zone.Width);
            }

        }


        
    }
}