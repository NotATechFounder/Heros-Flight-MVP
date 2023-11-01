using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiChaseState : AiStateBase
    {
        public AiChaseState(AiControllerBase aiController, AiAnimationController animatorController,
            IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
            if (aiController.TryGetController<AiMoverInterface>(out mover)) { }
        }

        private AiMoverInterface mover;
        
        public override void Enter()
        {
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
            
            if (!aiController.InAttackRange())
            {
                mover.MoveToTarget(aiController.CurrentTarget);
            }
            else
            {
                Exit();
                m_StateMachine.SetState(typeof(AiAttackState));
            }
        }

        public override void Exit()
        {
          Debug.Log("Exiting Agrovated state");
            mover.MoveToTarget(null);
          
            base.Exit();
        }
    }
}