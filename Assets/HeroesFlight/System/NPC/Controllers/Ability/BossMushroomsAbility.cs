using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossMushroomsAbility : BossAttackAbilityBase
    {
        [SerializeField] AreaDamageEntity[] mushrooms;
       
        protected override void Awake()
        {
            timeSincelastUse = 0;
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
            foreach (var mushroom in mushrooms)
            {
                mushroom.StartDetection();
            }
        }
    }
}