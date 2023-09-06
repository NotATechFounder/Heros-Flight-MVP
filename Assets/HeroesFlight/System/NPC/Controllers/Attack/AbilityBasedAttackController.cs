using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityBasedAttackController :EnemyAttackControllerBase
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
        }
        protected override void InitAttack()
        {
            timeSinceLastAttack = 0;
            aiController.SetAttackState(false);
            var possibleAbilities = new List<int>();
            for (int i = 0; i < abilities.Length; i++)
            {
                if(abilities[i].ReadyToUse)
                    possibleAbilities.Add(i);
            }

            if (possibleAbilities.Count > 0)
            {
                var targetAbility =
                    abilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count - 1))];
                if (targetAbility.StopMovementOnUse)
                {
                    aiController.SetMovementState(false);
                    targetAbility.UseAbility(aiController.GetDamage, target, () =>
                    {
                        aiController.SetMovementState(true);
                    });
                }
                targetAbility.UseAbility(aiController.GetDamage, target);
            }
        }
    }
}