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
        int spawnAmount ;
        int waves;
        GameObject player;
        
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();

        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;
        public void Init()
        {
            GenerateCache();
            player = GameObject.FindWithTag("Player");
            timeBetweenEnemySpawn = new WaitForSeconds(1f);
            timeBeweenWaves = new WaitForSeconds(10f);

        }

        public void SpawnEnemies(int enemiesToKill, int waves, Action<AiControllerBase> OnOnEnemySpawned)
        {
            StartCoroutine(SpawnEnemiesRoutine(enemiesToKill,waves, OnOnEnemySpawned));
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

        IEnumerator SpawnEnemiesRoutine(int enemiesToKill, int wavesNumber, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawnAmount = enemiesToKill;
            waves = wavesNumber;

            var amountToSpawnPerWave = enemiesToKill / wavesNumber;
            while (spawnAmount>0)
            {
                yield return StartCoroutine(SpawnWave(amountToSpawnPerWave, OnOnEnemySpawned));
                spawnAmount -= amountToSpawnPerWave;
                yield return timeBeweenWaves;
            }

           
        }

        IEnumerator SpawnWave(int enemiesToSpawn, Action<AiControllerBase> OnOnEnemySpawned)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
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
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }
    }
}