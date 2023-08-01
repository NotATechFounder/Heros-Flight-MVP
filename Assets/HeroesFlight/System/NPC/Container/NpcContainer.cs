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
        [SerializeField] AiControllerBase[] miniBosses;
        int spawnAmount ;
        int waves;
        GameObject player;

        List<AiControllerBase> spawnedEnemies = new();
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();

        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;
        public void Init()
        {
            GenerateCache();
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
                spawnedEnemies.Add(resultEnemy);
                OnOnEnemySpawned?.Invoke(resultEnemy);
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }

        public AiControllerBase SpawnMiniBoss()
        {
            var rng = Random.Range(0, miniBosses.Length);
            var targetPrefab = miniBosses[rng];
            var targetPoints = spanwPointsCache[targetPrefab.AgentModel.EnemySpawmType];
            var rngPoint=  Random.Range(0, targetPoints.Count);
            var resultEnemy = Instantiate(targetPrefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition()
                , Quaternion.identity);
            resultEnemy.transform.parent = transform;
            spawnedEnemies.Add(resultEnemy);
            resultEnemy.Init(player.transform);
            return resultEnemy;

        }

        public void InjectPlayer(Transform playerTransform)
        {
            player = playerTransform.gameObject;
        }

        public void Reset()
        {
            foreach (var enemy in spawnedEnemies)
            {
               Destroy(enemy.gameObject);
            }
            spawnedEnemies.Clear();
        }
    }
}