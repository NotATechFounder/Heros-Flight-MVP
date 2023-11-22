using System;
using System.Collections;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossEntanglingRootsAbilityStatic : BossAttackAbilityBase
    {
        
        [SerializeField] AreaDamageEntity[] mushrooms;
        [SerializeField] float preDamageDelay = 2f;

        [Header("References and parameters")]
        [SerializeField] float rootDuration = 5f;
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset startAnimation;
        [SerializeField] AnimationReferenceAsset endAnimation;
        [SerializeField] SkeletonAnimation entanglingRootView;
        Rigidbody2D character;
        Coroutine rootsRoutine;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            character = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
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
                    health.TryDealDamage(new HealthModificationIntentModel(CalculateDamage(),
                        DamageCritType.NoneCritical,AttackType.Regular,CalculationType.Flat,null));
                }
            }
        }


        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            CoroutineUtility.WaitForSeconds(preDamageDelay,() =>
            {
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                if(rootsRoutine!=null)
                    StopCoroutine(rootsRoutine);
                rootsRoutine = StartCoroutine(RootsRoutine());
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

        IEnumerator RootsRoutine()
        {
            character.bodyType = RigidbodyType2D.Static;
            entanglingRootView.AnimationState.SetAnimation(0, idleAniamtion, true);
            entanglingRootView.transform.position = character.transform.position + new Vector3(0,-1f,0);
            entanglingRootView.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            entanglingRootView.AnimationState.SetAnimation(0, startAnimation, false);
            entanglingRootView.AnimationState.AddAnimation(0, endAnimation, false, 0.2f);
            yield return new WaitForSeconds(rootDuration);
            entanglingRootView.AnimationState.SetAnimation(0, idleAniamtion, true);
            entanglingRootView.gameObject.SetActive(false);
            character.bodyType = RigidbodyType2D.Dynamic;
            
        }
    }
}