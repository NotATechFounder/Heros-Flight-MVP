using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common;
using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using Spine.Unity;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityBaseCharacter : MonoBehaviour, CharacterAbilityInterface
    {
        [SerializeField] float coolDown;
        [SerializeField] bool stopOnUse;
        protected List<AnimationReferenceAsset>  targetAnimations = new ();
        protected  CharacterAnimationControllerInterface animator;
        int targetCharges = 0;
        protected int currentCharges = 0;

        protected virtual void Awake()
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
     
        public void Init(List<AnimationData> animations, int charges)
        {
            foreach (var data in animations)
            {
                targetAnimations.Add(data.Aniamtion);
            }
            targetCharges = charges;
            currentCharges = 0;
        }

        public virtual void UpdateAbilityCharges(int value)
        {
            currentCharges += value;
        }
    }
}