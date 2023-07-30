using System;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiAnimatorInterface
    {
        event Action OnDynamicAnimationEnded;
        void StartAttackAnimation();
        void StopAttackAnimation();
        void PlayDeathAnimation(Action onCompleteAction);
    }
}