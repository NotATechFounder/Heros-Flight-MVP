using System;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAreaDamageAbility : BossAttackAbilityBase
    {
        [SerializeField] AbilityZone[] abilityZones;
        [SerializeField] float preDamageDelay;
        [SerializeField] float zoneWidth;
        public event Action<int, Collider2D[]> OnDetected; 
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
            OnDetected?.Invoke(count,targets);
            for (int i = 0; i < count; i++)
            {
                if(targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    health.DealDamage(new DamageModel(CalculateDamage(),DamageType.NoneCritical,AttackType.Regular));
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
                    cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                    zone.ZoneChecker.Detect();
                },preDamageDelay,zoneWidth);
            }

            currentCooldown = coolDown;
        }


        public override void StopAbility()
        {
            foreach (var zone in abilityZones)
            {
                zone.ZoneVisual.gameObject.SetActive(false);
                zone.ZoneChecker.gameObject.SetActive(false);
            }
        }
    }
}