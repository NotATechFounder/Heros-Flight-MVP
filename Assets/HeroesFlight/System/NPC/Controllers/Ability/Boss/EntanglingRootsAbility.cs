using System;
using Cinemachine;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.NPC.Controllers.Ability
{
    public class EntanglingRootsAbility : BossAbilityBase
    {
        [SerializeField] GameObject entityHolder;
         [SerializeField] float logicTriggerDelay = 2f;

        [Header("Parameters for optional visual zone usage")]
        [SerializeField] WarningLine zone;
        [SerializeField] float zoneWidth;
        [Header("Effect To Apply on Target")]
        [SerializeField] RootStatusEffect rootEffect;
        AreaDamageEntity[] mushrooms;
        GameObject runtimeEntityHolder;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            runtimeEntityHolder = Instantiate(entityHolder, transform.position, Quaternion.identity);
            mushrooms = runtimeEntityHolder.GetComponentsInChildren<AreaDamageEntity>();
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
               
                if(targets[i].TryGetComponent<CombatEffectsController>(out var controller))
                {
                    controller.ApplyStatusEffect(rootEffect,1);
                }
            }
        }


        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            runtimeEntityHolder.transform.position = transform.position;
            foreach (var mushroom in mushrooms)
            {
                mushroom.ToggleIndicator(true);
            }
            if (zone == null)
            {
                
                CoroutineUtility.WaitForSeconds(logicTriggerDelay,() =>
                {
                    cameraShaker?.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                    foreach (var mushroom in mushrooms)
                    {
                        mushroom.StartDetection();
                    }
                });
            }
            else
            {
                zone.Trigger(() =>
                {
                    cameraShaker?.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                    foreach (var mushroom in mushrooms)
                    {
                        mushroom.StartDetection();
                    }
                   
                },logicTriggerDelay,zoneWidth);
            }
           
            
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