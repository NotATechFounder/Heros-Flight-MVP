using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class RangeBasedAbilityAttackController : EnemyAttackControllerBase
    {
        [Header("Long Range Abilities")]
        [SerializeField] AbilityBaseNPC[] longRangeAbilities;
        [Header("Close Range Abilities")]
        [SerializeField] AbilityBaseNPC[] closeRangeAbilities;

        bool usingAbility;
        float distanceToPlayer;

        protected override void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
                aiController.SetAttackState(false);
                return;
            }


            timeSinceLastAttack += Time.deltaTime;
            if(usingAbility)
                return;
            
            distanceToPlayer = Vector2.Distance(transform.position, aiController.CurrentTarget.position);
            if (distanceToPlayer <= attackRange && timeSinceLastAttack >= timeBetweenAttacks)
            {
                aiController.SetAttackState(true);
                InitAttack();
            }
            else
            {
                aiController.SetAttackState(false);
            }
        }

        protected override void InitAttack()
        {
            timeSinceLastAttack = 0;

            UseAbilityByRange();
        }

        void UseAbilityByRange()
        {
            var targetAbilities = distanceToPlayer > attackRange / 2 ? longRangeAbilities : closeRangeAbilities;
           
            
            
            var possibleAbilities = new List<int>();
            for (int i = 0; i < targetAbilities.Length; i++)
            {
                if (targetAbilities[i].ReadyToUse)
                    possibleAbilities.Add(i);
            }

            if (possibleAbilities.Count > 0)
            {
                var targetAbility =
                    targetAbilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count - 1))];
                if (targetAbility.StopMovementOnUse)
                {
                    aiController.SetMovementState(false);
                    targetAbility.UseAbility(() =>
                    {
                        aiController.SetMovementState(true);
                        usingAbility = false;
                    });
                }
                else
                {
                    targetAbility.UseAbility(() =>
                    {
                        usingAbility = false;
                    });
                }

              
            }
        }
    }
}