using System;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.System.Character.Enum;
using Spine.Unity;

namespace HeroesFlight.System.Character
{
    public interface CharacterAnimationControllerInterface
    {
        event Action<AnimationEventInterface> OnAnimationEvent;
        event Action<bool> OnAttackAnimationStateChange;
        void Init(CharacterAnimations data);
        void AnimateCharacterMovement(CharacterState newState,bool isfacingLeft);
        void PlayDeathAnimation(Action onComplete=null);
        void PlayIdleAnimation();
        float PlayAttackSequence(float speedMultiplier);
        void StopAttackSequence();
        void PlayAnimationSequence(List<AnimationReferenceAsset> animations,Action onCompleteAction=null);
    }
}