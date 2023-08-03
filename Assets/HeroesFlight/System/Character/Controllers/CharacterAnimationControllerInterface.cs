using System;

namespace HeroesFlight.System.Character
{
    public interface CharacterAnimationControllerInterface
    {
        event Action<string> OnDealDamageRequest;
        void PlayDeathAnimation(Action onComplete=null);
        void PlayIdleAnimation();
        void PlayAttackSequence();
        void StopAttackSequence();
    }
}