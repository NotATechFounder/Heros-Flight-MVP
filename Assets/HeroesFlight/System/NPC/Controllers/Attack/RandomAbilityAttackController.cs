using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common.Animation;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class RandomAbilityAttackController : EnemyAttackControllerBase
    {
        [SerializeField] BossAbilityBase[] abilities;
        [SerializeField] OverlapChecker rangeCheck;
        CameraShakerInterface shaker;
        void Awake()
        {
          rangeCheck.OnDetect += HandlePlayerDetected;
          shaker = FindObjectOfType<CameraController>().CameraShaker;
          foreach (var ability in abilities)
          {
              ability.InjectShaker(shaker);
          }

        }
       
        void HandlePlayerDetected(int arg1, Collider2D[] arg2)
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                aiController.SetAttackState(true);
                InitAttack();
            }
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
            rangeCheck.Detect();
            if (timeSinceLastAttack < timeBetweenAttacks)
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
                Debug.Log(abilities[i].ReadyToUse);
                if (abilities[i].ReadyToUse)
                    possibleAbilities.Add(i);
                   
            }
            Debug.Log(possibleAbilities.Count);
            if (possibleAbilities.Count > 0)
            {
                var targetAbility =
                    abilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count ))];
                Debug.Log(targetAbility.name);
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

        protected override void HandleAnimationEvents(AttackAnimationEvent obj) { }
    }
}