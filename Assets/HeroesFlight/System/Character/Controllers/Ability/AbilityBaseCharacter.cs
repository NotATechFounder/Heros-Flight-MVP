using System;
using System.Linq;
using HeroesFlight.System.Character;
using Spine.Unity;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityBaseCharacter : MonoBehaviour,CharacterAbilityInterface
    {
        [SerializeField] float coolDown;
        [SerializeField] bool stopOnUse;
        protected AnimationReferenceAsset[] targetAnimations;
        protected  CharacterAnimationControllerInterface animator;
        int targetCharges = 0;
        int currentCharges = 0;

        void Awake()
        {
            animator = GetComponent<CharacterAnimationControllerInterface>();
        }


        public float CoolDown => coolDown;
        public bool StopMovementOnUse => stopOnUse;
        public bool ReadyToUse=> currentCharges >= targetCharges;

        public virtual void UseAbility(Action onComplete = null)
        {
            currentCharges = 0;
            animator.PlayAnimationSequence(targetAnimations.ToList(), () =>
            {
                onComplete?.Invoke();
            });
        }

        public float CurrentCharge => (float)currentCharges/targetCharges;
     
        public void Init(AnimationReferenceAsset[] animations,int charges)
        {
            targetAnimations = animations;
            targetCharges = charges;
            currentCharges = 0;
        }

        public virtual void UpdateAbilityCharges(int value)
        {
            currentCharges += value;
        }
    }
}