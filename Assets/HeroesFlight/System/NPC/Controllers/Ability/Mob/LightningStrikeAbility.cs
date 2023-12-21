using System;
using HeroesFlight.Common.Animation;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class LightningStrikeAbility : AttackAbilityBaseNPC
    {
        [SerializeField] private ZoneDetectorWithIndicator detector;
        private ZoneDetectorWithIndicator runtimeDetector;
        JuicerRuntime juicerRuntime;
        protected override void Awake()
        {
            base.Awake();
            runtimeDetector = Instantiate(detector, transform.position, Quaternion.identity);
            runtimeDetector.OnTargetsDetected += HandleTargetsDetected;
        }

        private void HandleTargetsDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                if (targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    TryDealDamage(health);
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

            var position = attackController.Target.HealthTransform.position;
            runtimeDetector.transform.position = position;
            runtimeDetector.StartDetection();
            currentCooldown = CoolDown;
        }

        void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            if (abilityParticle != null)
            {
                abilityParticle.Play();
            }

           // runtimeDetector.StartDetection();
        }
    }
}