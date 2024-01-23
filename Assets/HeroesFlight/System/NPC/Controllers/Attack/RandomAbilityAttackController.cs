using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common.Animation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class RandomAbilityAttackController : EnemyAttackControllerBase
    {
        [SerializeField] AbilityBaseNPC[] abilities;

        CameraShakerInterface shaker;

        void Awake()
        {
            shaker = FindObjectOfType<CameraController>().CameraShaker;
        }

       protected override void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
                return;
            }


            timeSinceLastAttack += Time.deltaTime;
        }

        protected override void InitAttack(Action onComplete = null)
        {
            timeSinceLastAttack = 0;
            UseRandomAbility(onComplete);
        }

        void UseRandomAbility(Action onComplete)
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
                    abilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count))];
                if (targetAbility.StopMovementOnUse)
                {
                    targetAbility.UseAbility(() => { onComplete?.Invoke(); });
                }
                else
                {
                    targetAbility.UseAbility();
                    onComplete?.Invoke();
                }
            }
        }

        protected override void HandleAnimationEvents(AttackAnimationEvent obj) {
        }


        public override void SetAttackStats(float damage, float attackRange, float attackSpeed, float criticalChance)
        {
            base.SetAttackStats(damage, attackRange, attackSpeed, criticalChance);
            foreach (var ability in abilities)
            {
                if (ability.GetType().IsSubclassOf(typeof(AttackAbilityBaseNPC)))
                {
                    var attackAbility = ability as AttackAbilityBaseNPC;
                    attackAbility.SetStats(currentDamage,base.criticalChance);
                }
            }
        }
    }
}