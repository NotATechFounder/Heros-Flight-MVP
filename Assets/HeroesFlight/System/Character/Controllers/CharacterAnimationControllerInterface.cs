using System;
using HeroesFlight.Common;
using HeroesFlight.System.Character.Enum;

namespace HeroesFlight.System.Character
{
    public interface CharacterAnimationControllerInterface
    {
        event Action<string> OnDealDamageRequest;
        void Init(AnimationData data);
        void AnimateCharacterMovement(CharacterState newState,bool isfacingLeft);
        void PlayDeathAnimation(Action onComplete=null);
        void PlayIdleAnimation();
        void PlayAttackSequence(float speedMultiplier);
        void StopAttackSequence();
    }
}