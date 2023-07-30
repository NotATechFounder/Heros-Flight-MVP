using System;
using Spine.Unity;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiAnimatorInterface
    { 
        void StartAttackAnimation(Action onCompleteAction);
        void StopAttackAnimation();
        void PlayDeathAnimation(Action onCompleteAction);
        void PlayAnimation(AnimationReferenceAsset animationReference, Action onCompleteAction = null);
    }
}