using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiChaseState : AiStateBase
    {
        public AiChaseState(AiControllerBase aiController, AiAnimationController animatorController,
            IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
            this.aiController = aiController;
            this.aiController.TryGetController(out mover);
            this.aiController.TryGetController(out attackController);
        }

        private AiMoverInterface mover;
        private EnemyAttackControllerBase attackController;
        private AiControllerBase aiController;

        public override void Enter()
        {
            mover.SetMovementSpeed(aiController.AgentModel.AiData.MoveSpeed *
                                   aiController.AgentModel.AiData.SpeedModifier);
            base.Enter();
        }

        protected override void Update()
        {
            if (!aiController.IsAggravated())
            {
                Exit();
                m_StateMachine.SetState(typeof(AiWanderingState));
                return;
            }

            if (!attackController.InAttackRange())
            {
                mover.MoveToTarget(aiController.CurrentTarget);
                animator.SetMovementDirection(mover.GetVelocity());
            }
            else
            {
                Exit();
                m_StateMachine.SetState(typeof(AiAttackState));
            }
        }

        public override void Exit()
        {
            mover.MoveToTarget(null);
            mover.SetMovementSpeed(aiController.AgentModel.AiData.MoveSpeed);
            base.Exit();
        }
    }
}