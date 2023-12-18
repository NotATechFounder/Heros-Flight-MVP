using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiStateBase : FSMState
    {
       protected  AiControllerBase aiController;
       protected  AiAnimationController animator;
        protected AiStateBase(AiControllerBase aiController,AiAnimationController animatorController,IFSM stateMachine) : base(stateMachine)
        {
            this.aiController = aiController;
            animator = animatorController;

        }
    }
}