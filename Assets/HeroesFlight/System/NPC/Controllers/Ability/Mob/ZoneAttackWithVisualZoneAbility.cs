using System;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

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

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, onComplete);
            }

            TriggerAbilityZones();
        }

        private void TriggerAbilityZones()
        {
            foreach (var zone in abilityZones)
            {
                zone.ZoneVisual.Trigger(() => { zone.ZoneChecker.DetectOverlap(); }, 
                    preDamageDelay, zone.Width);
            }
        }

        void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                if (targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    TryDealDamage(health);
                }
            }
        }
    }
}