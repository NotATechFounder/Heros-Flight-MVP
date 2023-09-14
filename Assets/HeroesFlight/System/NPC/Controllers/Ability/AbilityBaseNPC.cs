using System;
using HeroesFlightProject.System.NPC.Controllers;
using Spine.Unity;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityBaseNPC : MonoBehaviour,AbilityInterface
    {
        [SerializeField] protected AnimationReferenceAsset targetAnimation;
        [SerializeField] protected float coolDown;
        [SerializeField] protected bool stopOnUse = true;
        protected AiAnimatorInterface animator;
       [SerializeField] protected float currentCooldown;
       protected virtual void Awake()
        {
            animator = GetComponent<AiAnimatorInterface>();
            currentCooldown = 0;
        }

        protected virtual void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
          
        }


        public float CoolDown => coolDown;
        public bool StopMovementOnUse => stopOnUse;
        public virtual bool ReadyToUse => currentCooldown <= 0;
        

        public virtual void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayAnimation(targetAnimation,onComplete);
            }
            

            currentCooldown = coolDown;
        }
    }
}