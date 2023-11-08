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
            aiController.TryGetController(out mover);
            aiController.TryGetController(out attackController);
        }

        private AiMoverInterface mover;
        private EnemyAttackControllerBase attackController;
        
      
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
          base.Exit();
        }
    }
}