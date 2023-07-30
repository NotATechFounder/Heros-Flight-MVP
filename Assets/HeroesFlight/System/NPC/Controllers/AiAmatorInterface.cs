using System;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiAnimatorInterface
    {
        event Action OnDynamicAnimationEnded;
        void StartAttackAnimation(Action onCompleteAction);
        void StopAttackAnimation();
        void PlayDeathAnimation(Action onCompleteAction);
    }
}