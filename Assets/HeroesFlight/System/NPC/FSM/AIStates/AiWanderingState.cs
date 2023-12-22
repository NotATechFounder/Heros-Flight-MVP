using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiWanderingState : AiStateBase
    {
        public AiWanderingState(AiControllerBase aiController, AiAnimationController animatorController,
            IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
            aiController.TryGetController(out mover);
            aiController.TryGetController(out attackController);
            aiController.TryGetController(out healthController);
            this.aiController.TryGetController(out viewController);
        }

        private AiMoverInterface mover;
        private IAttackControllerInterface attackController;
        private IHealthController healthController;
        private AiViewController viewController;

        public override void Enter()
        {
            mover.MoveToTarget(null);
            aiController.SetMovementState(true);
            base.Enter();
        }

        protected override void Update()
        {
            if (aiController.IsAggravated() && attackController.CanAttack())
            {
                Exit();
            }
            else
            {
                mover.MoveToRandomPosition();
                viewController.UpdateAiRotation(mover.GetVelocity());
                base.Update();
            }
        }

        public override void Exit()
        {
            m_StateMachine.SetState(typeof(AiChaseState));
            base.Exit();
        }
    }
}