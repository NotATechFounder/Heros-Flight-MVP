using System;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Ability
{
    public class SpawnEnemyAbility : AbilityBaseNPC
    {
        [Header("Spawn data")] [SerializeField]
        protected float preSpawnDelay = 1.5f;

        [SerializeField] protected int amountToSpawn;
        [SerializeField] List<AiControllerBase> targetToSpawn = new();
        protected List<AiControllerBase> spawnedMobs = new();
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
            SpawnRandomEnemies(targetToSpawn);
        }

        protected void SpawnRandomEnemies(List<AiControllerBase> aiControllerBases)
        {
            CoroutineUtility.WaitForSeconds(preSpawnDelay, () =>
            {
                for (int i = 0; i < amountToSpawn; i++)
                {
                    var targetMobIndex = Random.Range(0, targetToSpawn.Count);
                    var newMob = Instantiate(aiControllerBases[targetMobIndex], transform.position,
                        Quaternion.identity);
                    spawnedMobs.Add(newMob);
                    OnEnemySpawned?.Invoke(newMob.GetComponent<IHealthController>());
                }
            });
        }

        public override void StopAbility()
        {
            base.StopAbility();
            foreach (var mob in spawnedMobs)
            {
                if (mob != null && mob.gameObject.activeSelf)
                    mob.Disable();
            }

            spawnedMobs.Clear();
        }

        void OnDisable()
        {
            StopAbility();
        }

        protected void NotifyEnemySpawned(IHealthController newEnemy)
        {
            OnEnemySpawned?.Invoke(newEnemy);
        }
    }
}