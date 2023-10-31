using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiIdleState : AiStateBase
    {
        private AiAnimationController aniamtorController;

        protected AiIdleState(AiControllerBase aiController, AiAnimationController animatorController, IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
        }
    }
}