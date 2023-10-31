using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.State.AIStates
{
    public class AiWanderingState : AiStateBase
    {
        private AiMoverInterface mover;

        public AiWanderingState(AiControllerBase aiController, AiAnimationController animatorController,
            IFSM stateMachine) : base(aiController, animatorController, stateMachine)
        {
          
        }

        public override void Enter()
        {
            Debug.Log("Entered wandering");
            if (aiController.TryGetController<AiMoverInterface>(out mover))
            {
              mover.SetMovementState(true);
            }
            base.Enter();
        }

        protected override void Update()
        {
            if (!aiController.IsAggravated())
            {
                Debug.Log("Moving");
                mover.MoveToRandomPosition();
                base.Update();
            }
            else
            {
                Exit();
            }

        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}