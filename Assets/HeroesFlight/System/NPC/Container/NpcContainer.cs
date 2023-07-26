using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlight.System.NPC.Container
{
    public class NpcContainer : MonoBehaviour
    {
        [SerializeField] AiControllerBase[] aiPrefabs;
        int spawnAmount = 10;
        GameObject player;
        
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            GenerateCache();
            player = GameObject.FindWithTag("Player");
           
        }

        public void SpawnEnemies(int amount, Action<AiControllerBase> OnOnEnemySpawned)
        {
            StartCoroutine(SpawnEnemiesRoutine(amount, OnOnEnemySpawned));

            // spawnAmount = amount;
            // for (var i = 0; i <= spawnAmount; i++)
            // {
            //     var rng = Random.Range(0, aiPrefabs.Length);
            //
            //     var targetPrefab = aiPrefabs[rng];
            //     var targetPoints = spanwPointsCache[targetPrefab.AgentModel.EnemySpawmType];
            //     var rngPoint=  Random.Range(0, targetPoints.Count);
            //     var resultEnemy = Instantiate(targetPrefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition()
            //         , Quaternion.identity);
            //     resultEnemy.transform.parent = transform;
            //     resultEnemy.Init(player.transform);
            //     OnOnEnemySpawned?.Invoke(targetPrefab);
            // }
        }

        void GenerateCache()
        {
            var spawnPoints =FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            foreach (var point in spawnPoints)
            {
                if (spanwPointsCache.TryGetValue(point.TargetEnemySpawmType, out var list))
                {
                    list.Add(point);
                }
                else
                {
                    spanwPointsCache.Add(point.TargetEnemySpawmType,new List<ISpawnPointInterface>(){point});
                }
            }
        }

        IEnumerator SpawnEnemiesRoutine(int amount, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawnAmount = amount;
            for (var i = 0; i < spawnAmount; i++)
            {
                var rng = Random.Range(0, aiPrefabs.Length);

                var targetPrefab = aiPrefabs[rng];
                var targetPoints = spanwPointsCache[targetPrefab.AgentModel.EnemySpawmType];
                var rngPoint=  Random.Range(0, targetPoints.Count);
                var resultEnemy = Instantiate(targetPrefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition()
                    , Quaternion.identity);
                resultEnemy.transform.parent = transform;
                resultEnemy.Init(player.transform);
                OnOnEnemySpawned?.Invoke(resultEnemy);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}