using System;
using HeroesFlight.Common.Animation;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiAnimatorInterface
    {
        event Action<AttackAnimationEvent> OnAnimationEvent;

        void SetMovementAnimation(bool isMoving);
        void StartAttackAnimation(AnimationReferenceAsset animationReference,Action onComplete=null);
        void StartAttackAnimation(Action onComplete=null);
        void StopAttackAnimation();
        void PlayDeathAnimation(Action onCompleteAction,float timeScale=1f);
        void PlayHitAnimation(bool interruptAttack,Action onCompleteAction=null);
        void PlayDynamicAnimation(AnimationReferenceAsset animationReference, Action onCompleteAction = null);
        void StopDynamicAnimation();
     
    }
}