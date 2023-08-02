using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlight.System.NPC.Container
{
    public class NpcContainer : MonoBehaviour
    {
        int spawnAmount ;
        GameObject player;

        List<AiControllerBase> spawnedEnemies = new();
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();

        Coroutine spawningWaveRoutine;
        Coroutine spawningRoutine;
        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;
        public void Init()
        {
            GenerateCache();
            timeBetweenEnemySpawn = new WaitForSeconds(1f);
            timeBeweenWaves = new WaitForSeconds(10f);

        }

        public void SpawnEnemies(SpawnModel model, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawningRoutine= StartCoroutine(SpawnEnemiesRoutine(model, OnOnEnemySpawned));
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

        IEnumerator SpawnEnemiesRoutine(SpawnModel model, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawnAmount = model.MobsAmount;
            var amountToSpawnPerWave =model.MobsAmount / model.WavesAmount ;
            while (spawnAmount>0)
            {
                yield return spawningWaveRoutine =StartCoroutine(SpawnWave(model,amountToSpawnPerWave, OnOnEnemySpawned));
                spawnAmount -= amountToSpawnPerWave;
                yield return timeBeweenWaves;
            }

          
        }

        IEnumerator SpawnWave(SpawnModel spawnModel, int enemiesToSpawn, Action<AiControllerBase> OnOnEnemySpawned)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                var rng = Random.Range(0, spawnModel.TrashMobs.Count);

                var targetPrefab = spawnModel.TrashMobs[rng];
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

        public AiControllerBase SpawnMiniBoss(SpawnModel currentLvlModel)
        {
            var rng = Random.Range(0, currentLvlModel.MiniBosses.Count);
            var targetPrefab = currentLvlModel.MiniBosses[rng];
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
            spawnAmount = 0;
            if (spawningWaveRoutine != null)
            {
                StopCoroutine(spawningWaveRoutine);
            }
            if (spawningRoutine != null)
            {
                StopCoroutine(spawningRoutine);
            }
        }
    }
}