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
        [SerializeField] protected bool stopOnUse = true;
        protected AiAnimatorInterface animator;
       [SerializeField] protected float currentCooldown;
       protected virtual void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
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
    }
}