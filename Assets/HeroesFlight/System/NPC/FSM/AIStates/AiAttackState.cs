using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiAttackState : AiStateBase
    {
        protected AiAttackState(AiControllerBase aiController, AiAnimationController animatorController, IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
        }
    }
}