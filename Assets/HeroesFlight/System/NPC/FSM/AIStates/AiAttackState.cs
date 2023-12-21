using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiAttackState : AiStateBase
    {
        public AiAttackState(AiControllerBase aiController, AiAnimationController animatorController, IFSM stateMachine)
            : base(aiController, animatorController, stateMachine)
        {
            aiController.TryGetController(out attackController);
            aiController.TryGetController(out mover);
        }

        private IAttackControllerInterface attackController;
        private AiMoverInterface mover;

        public override void Enter()
        {
            aiController.FaceTarget();
            attackController.AttackTargets(() => { Exit(); });
            aiController.SetMovementState(false);
            base.Enter();
        }

        protected override void Update() { }


        public override void Exit()
        {
            m_StateMachine.SetState(typeof(AiWanderingState));
            mover.SetMovementState(true);
            base.Exit();
        }
    }
}