using System;
using HeroesFlight.Common.Animation;
using Spine.Unity;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiAnimatorInterface
    {
        event Action<AttackAnimationEvent> OnAnimationEvent;

        void SetMovementAnimation(bool isMoving);
        void StartAttackAnimation(Action onCompleteAction);
        void StopAttackAnimation();
        void PlayDeathAnimation(Action onCompleteAction);
        void PlayHitAnimation(bool interruptAttack,Action onCompleteAction=null);
        void PlayDynamicAnimation(AnimationReferenceAsset animationReference, Action onCompleteAction = null);
        void StopDynamicAnimation();
    }
}