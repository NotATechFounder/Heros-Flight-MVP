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
        protected IAttackControllerInterface attackController;
        int currentCharges = 0;

        void Awake()
        {
            animator = GetComponent<CharacterAnimationControllerInterface>();
            attackController = GetComponent<CharacterAttackController>();
            //attackController.ToggleControllerState(false);
        }

        public virtual bool ReadyToUse => currentCharges >= targetCharges;
        public virtual void UseAbility(IHealthController target = null, Action onComplete = null)
        {
            currentCharges = 0;
            attackController.ToggleControllerState(false);
            animator.PlayAnimationSequence(targetAnimations.ToList(), () =>
            {
                attackController.ToggleControllerState(true);
            });
        }

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