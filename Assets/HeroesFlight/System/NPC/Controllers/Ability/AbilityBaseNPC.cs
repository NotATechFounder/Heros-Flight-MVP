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
        protected float timeSincelastUse;
       protected virtual void Awake()
        {
            animator = GetComponent<AiAnimatorInterface>();
            timeSincelastUse = 0;
        }

        protected virtual void Update()
        {
            if (timeSincelastUse > 0)
            {
                timeSincelastUse -= Time.deltaTime;
            }
          
        }


        public float CoolDown => coolDown;
        public bool StopMovementOnUse => stopOnUse;
        public virtual bool ReadyToUse => timeSincelastUse <= 0;
        

        public virtual void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayAnimation(targetAnimation,onComplete);
            }

            timeSincelastUse = coolDown;
        }
    }
}