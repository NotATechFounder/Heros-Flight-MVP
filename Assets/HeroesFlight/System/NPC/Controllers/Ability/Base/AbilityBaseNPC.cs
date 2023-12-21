using System;
using HeroesFlightProject.System.NPC.Controllers;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public abstract class AbilityBaseNPC : MonoBehaviour,AbilityInterface
    {
        [SerializeField] protected AnimationReferenceAsset targetAnimation;
        [SerializeField] protected Vector2 coolDown;
        [Range(0, 100)][SerializeField] float useChance;
        [SerializeField] protected bool stopOnUse = true;
        [SerializeField] protected ParticleSystem abilityParticle;
        [SerializeField] protected bool abilityInteraptuble;
        protected AiAnimatorInterface animator;
        protected EnemyAttackControllerBase attackController;
        protected CameraShakerInterface cameraShaker;
        protected float currentCooldown;
        public float UseChance => useChance;

        protected virtual void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            currentCooldown = 0;
        }

        protected virtual void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
          
        }

        public float CoolDown => Random.Range(coolDown.x,coolDown.y);
        public bool StopMovementOnUse => stopOnUse;
        public virtual bool ReadyToUse => currentCooldown <= 0;


        public virtual void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation,onComplete);
            }
            

            currentCooldown = CoolDown;
        }


        public void InjectShaker(CameraShakerInterface shaker)
        {
            cameraShaker = shaker;
        }
        
        public virtual void StopAbility()
        {
            animator.StopDynamicAnimation();
        }

        public virtual void ResetAbility()
        {
            animator.StopDynamicAnimation();
        }

        public void SetCoolDown(float cd)
        {
            currentCooldown = cd;
        }
    }
}