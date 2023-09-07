using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAreaDamageAbility : AbilityBaseNPC
    {
        [SerializeField] AbilityZone[] abilityZones;
        [SerializeField] float preDamageDelay;
        [SerializeField] float zoneWidth;
        [SerializeField] float damage;
        public event Action<int, Collider2D[]> OnDetected; 
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            timeSincelastUse = 0;
            foreach (var zone in abilityZones)
            {
                zone.ZoneChecker.OnDetect += NotifyTargetDetected;
            }
        }

        void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            OnDetected?.Invoke(count,targets);
            for (int i = 0; i < count; i++)
            {
                if(targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    health.DealDamage(new DamageModel(damage,DamageType.NoneCritical,AttackType.Regular));
                }
            }
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayAnimation(targetAnimation,onComplete);
            }

            foreach (var zone in abilityZones)
            {
                zone.ZoneVisual.Trigger(() =>
                {
                    zone.ZoneChecker.Detect();
                },preDamageDelay,zoneWidth);
            }

            timeSincelastUse = coolDown;
        }
    }
}