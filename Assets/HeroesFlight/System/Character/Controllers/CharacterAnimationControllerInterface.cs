using System;

namespace HeroesFlight.System.Character
{
    public interface CharacterAnimationControllerInterface
    {
        event Action<string> OnDealDamageRequest; 
        void PlayAttackSequence();
        void StopAttackSequence();
    }
}