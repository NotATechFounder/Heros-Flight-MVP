using System;
using System.Linq;
using Cinemachine;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ValSpecialSkillController : AbilityBaseCharacter
    {
        CameraShakerInterface shaker;
        protected override void Awake()
        {
            base.Awake();
            shaker = FindObjectOfType<CameraController>().CameraShaker;
        }

        public override void UseAbility(Action onComplete = null)
        {
            currentCharges = 0;
            animator.OnAnimationEvent += HandleAnimationEvent;
            animator.PlayAnimationSequence(targetAnimations.ToList(), () =>
            {
                onComplete?.Invoke();
            });
        }

        void HandleAnimationEvent(AnimationEventInterface animationEvent)
        {
            if (animationEvent.Type != AniamtionEventType.Attack)
                return;
            
            Debug.Log("SHAKE");
            shaker.ShakeCamera(
                CinemachineImpulseDefinition.ImpulseShapes.Recoil,2f,5);
            animator.OnAnimationEvent -= HandleAnimationEvent;
        }
    }
}