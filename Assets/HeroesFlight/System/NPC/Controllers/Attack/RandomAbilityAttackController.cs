using System;
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
            if (CanAttack())
            {
              InitAttack(null);
            }
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
          //  rangeCheck.Detect();
        }

        protected override void InitAttack(Action onComplete=null)
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
                    abilities[possibleAbilities.ElementAt(Random.Range(0, possibleAbilities.Count ))];
                Debug.Log(targetAbility.name);
                if (targetAbility.StopMovementOnUse)
                {
                    targetAbility.UseAbility(() =>
                    {
                       onComplete?.Invoke();
                    });
                }
                else
                {
                    targetAbility.UseAbility();     
                    onComplete?.Invoke();
                }

               
            }
        }

        protected override void HandleAnimationEvents(AttackAnimationEvent obj) { }
    }
}