using System;
using System.Linq;
using HeroesFlight.System.Character;
using Spine.Unity;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityBaseCharacter : MonoBehaviour,CharacterAbilityInterface
    {
        [SerializeField] int targetCharges = 100;
        [SerializeField] protected AnimationReferenceAsset[] targetAnimations;
        protected  CharacterAnimationControllerInterface animator;
      
      
        int currentCharges = 0;

        void Awake()
        {
            animator = GetComponent<CharacterAnimationControllerInterface>();
        }


        public bool ReadyToUse=> currentCharges >= targetCharges;

        public virtual void UseAbility(IHealthController target = null, Action onComplete = null)
        {
            currentCharges = 0;
            animator.PlayAnimationSequence(targetAnimations.ToList(), () =>
            {
                onComplete?.Invoke();
            });
        }

        public float CurrentCharge => (float)currentCharges/targetCharges;
     
        public void Init(AnimationReferenceAsset[] animations)
        {
            targetAnimations = animations;
        }

        public virtual void UpdateAbilityCharges(int value)
        {
            currentCharges += value;
        }
    }
}