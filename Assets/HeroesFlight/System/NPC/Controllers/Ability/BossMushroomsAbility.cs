using System;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossMushroomsAbility : BossAttackAbilityBase
    {
        [SerializeField] AreaDamageEntity[] mushrooms;
        [SerializeField] float preDamageDelay = 2f;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            foreach (var mushroom in mushrooms)
            {
                mushroom.Init();
                mushroom.OnTargetsDetected += HandleTargetsDetected;
            }
        }

        void HandleTargetsDetected(int count, Collider2D[] targets)
        {
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
            base.UseAbility(onComplete);
            CoroutineUtility.WaitForSeconds(preDamageDelay,() =>
            {
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                foreach (var mushroom in mushrooms)
                {
                    mushroom.StartDetection();
                }
            });
            
        }
        
        
        public override void StopAbility()
        {
            foreach (var mushroom in mushrooms)
            {
                mushroom.gameObject.SetActive(false);
            }
        }
    }
}