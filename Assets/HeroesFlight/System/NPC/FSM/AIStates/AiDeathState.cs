using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiDeathState : AiStateBase
    {
        public AiDeathState(AiControllerBase aiController, AiAnimationController animatorController,
            IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
           aiController.TryGetController(out mover);
        }

        private AiMoverInterface mover;
        public override void Enter()
        {
            mover.SetMovementState(false);
            aiController.Disable();
            base.Enter();
        }
    }
}