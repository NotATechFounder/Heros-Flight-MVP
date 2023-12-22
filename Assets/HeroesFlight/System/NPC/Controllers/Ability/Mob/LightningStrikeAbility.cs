using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class LightningStrikeAbility : AttackAbilityBaseNPC
    {
        [Header("Ligtning strike parameters")]
        [SerializeField] private int amountOfStrikes = 1;
        [SerializeField] private float delayBetweenStrikes = 0.25f;
        [SerializeField] private ZoneDetectorWithIndicator detector;
        private List<ZoneDetectorWithIndicator>  runtimeDetectors= new ();
        JuicerRuntime juicerRuntime;
        private Transform target;
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < amountOfStrikes; i++)
            {
                var runtimeDetector =Instantiate(detector, transform.position, Quaternion.identity);
                runtimeDetector.OnTargetsDetected += HandleTargetsDetected;
                    runtimeDetectors.Add(runtimeDetector);
            }

            target = GameObject.FindWithTag("Player").transform;

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

          //  ActivateDetectors();
            currentCooldown = CoolDown;
        }

        private void ActivateDetectors()
        {
            StartCoroutine(ActivateDetectorsRoutine());
           
        }

        void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            if (abilityParticle != null)
            {
                abilityParticle.Play();
            }

            ActivateDetectors();
        }

        IEnumerator ActivateDetectorsRoutine()
        {
            for (int i = 0; i < amountOfStrikes; i++)
            {
                var position =target.position;
                runtimeDetectors[i].transform.position = position;
                runtimeDetectors[i].StartDetection();
                yield return new WaitForSeconds(delayBetweenStrikes);
            }
        }
    }
}