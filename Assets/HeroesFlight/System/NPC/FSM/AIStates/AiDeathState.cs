using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiDeathState : AiStateBase
    {
        protected AiDeathState(AiControllerBase aiController, AiAnimationController animatorController, IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
        }
    }
}