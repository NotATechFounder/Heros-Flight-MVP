using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiAggravatedState : AiStateBase
    {
        
        protected AiAggravatedState(AiControllerBase aiController, AiAnimationController animatorController, IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
        }
    }
}