using System.Collections.Generic;
using HeroesFlightProject.System.NPC.State;
using HeroesFlightProject.System.NPC.State.AIStates;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class StationaryAiController : AiControllerBase

    {
        public override void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            stateMachine = new FSMachine();
            var animator = GetComponent<AiAnimationController>();
            stateMachine.AddStates(new List<FSMState>()
            {
                new AiWanderingState(this, animator, stateMachine),
                new AiChaseState(this, animator, stateMachine),
                new AiAttackState(this, animator, stateMachine),
                new AiDeathState(this, animator, stateMachine)
            });
            Debug.Log("Initied stationary");
            base.Init(player, health, damage, monsterStatModifier, currentCardIcon);
            stateMachine.SetState(typeof(AiWanderingState));
            
        }
    }
}