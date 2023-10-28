using System;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability
{
    public class SpawnEnemyAbility : BossAbilityBase
    {
        [Header("Spawn data")]
        [SerializeField] int amountToSpawn;
        [SerializeField] AiControllerBase targetToSpawn;
        [Header("Mobs stats")]
        [SerializeField] int health;
        [SerializeField] float damage;
        List<AiControllerBase> spawnedMobs = new();
        public event Action<IHealthController> OnEnemySpawned;
        Transform player;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            player = GameObject.FindWithTag("Player").transform;
        }

        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            CoroutineUtility.WaitForSeconds(1.5f, () =>
            {
                for (int i = 0; i < amountToSpawn; i++)
                {
                    var newMob = Instantiate(targetToSpawn, transform.position, Quaternion.identity);
                    newMob.Init(player,health,damage,new MonsterStatModifier(),null);
                  //  newMob.GetComponent<IHealthController>().Init();
                    spawnedMobs.Add(newMob);
                    OnEnemySpawned?.Invoke( newMob.GetComponent<IHealthController>());
                }
            });
            
        }

        public override void StopAbility()
        {
            base.StopAbility();
            foreach (var mob in spawnedMobs)
            {
                if(mob.gameObject.activeSelf)
                    mob.Disable();
            }
            spawnedMobs.Clear();
        }

        void OnDisable()
        {
            StopAbility();
        }
    }
}