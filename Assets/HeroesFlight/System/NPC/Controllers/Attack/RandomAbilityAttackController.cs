using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class RandomAbilityAttackController : EnemyAttackControllerBase
    {
        [SerializeField] AbilityBaseNPC[] abilities;


        void Awake()
        {
            abilities = GetComponents<AbilityBaseNPC>();
        }

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
            var distanceToPlayer = Vector2.Distance(transform.position, aiController.CurrentTarget.position);
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

            UseRandomAbility();
        }

        void UseRandomAbility()
        {
            var possibleAbilities = new List<int>();
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i].ReadyToUse)
                    possibleAbilities.Add(i);
            }

            if (possibleAbilities.Count > 0)
            {
                var targetAbility =
                    abilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count - 1))];
                if (targetAbility.StopMovementOnUse)
                {
                    aiController.SetMovementState(false);
                    targetAbility.UseAbility(() =>
                    {
                        aiController.SetMovementState(true);
                    });
                }
                else
                {
                    targetAbility.UseAbility();     
                }

               
            }
        }

      
    }
}