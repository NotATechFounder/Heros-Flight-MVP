using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlight.System.NPC.Data;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlight.System.NPC.Container
{
    public class NpcContainer : MonoBehaviour
    {
        int spawnAmount;
        GameObject player;

        List<AiControllerBase> spawnedEnemies = new();
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();
        MonsterStatController monsterStatController;
        Coroutine spawningWaveRoutine;
        Coroutine spawningRoutine;
        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;

        public void Init()
        {
            GenerateCache();
        }

        public void SpawnEnemies(SpawnModel model, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawningRoutine = StartCoroutine(SpawnEnemiesRoutine(model, OnOnEnemySpawned));
        }

        void GenerateCache()
        {
            var spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var point in spawnPoints)
            {
                if (spanwPointsCache.TryGetValue(point.TargetEnemySpawmType, out var list))
                {
                    list.Add(point);
                }
                else
                {
                    spanwPointsCache.Add(point.TargetEnemySpawmType, new List<ISpawnPointInterface>() { point });
                }
            }

            // To be removed
            monsterStatController = FindObjectOfType<MonsterStatController>();
        }

        IEnumerator SpawnEnemiesRoutine(SpawnModel model, Action<AiControllerBase> OnOnEnemySpawned)
        {
            timeBetweenEnemySpawn = new WaitForSeconds(model.TimeBetweenMobs);
            timeBeweenWaves = new WaitForSeconds(model.TimeBetweenWaves);
            spawnAmount = model.MobsAmount;
            var amountToSpawnPerWave = model.MobsAmount / model.WavesAmount;
            while (spawnAmount > 0)
            {
                yield return spawningWaveRoutine =
                    StartCoroutine(SpawnWave(model, amountToSpawnPerWave, OnOnEnemySpawned));
                spawnAmount -= amountToSpawnPerWave;
                yield return timeBeweenWaves;
            }
        }

        IEnumerator SpawnWave(SpawnModel spawnModel, int enemiesToSpawn, Action<AiControllerBase> OnOnEnemySpawned)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                var targetEntry = PickRandomTrashMob(spawnModel.TrashMobs);
                var targetPoints = spanwPointsCache[targetEntry.Prefab.AgentModel.EnemySpawmType];
                var rngPoint = Random.Range(0, targetPoints.Count);
                AiControllerBase resultEnemy = Instantiate(targetEntry.Prefab,
                    targetPoints.ElementAt(rngPoint).GetSpawnPosition()
                    , Quaternion.identity);
                resultEnemy.transform.parent = transform;
                resultEnemy.Init(player.transform, monsterStatController.GetMonsterStatModifier,
                    monsterStatController.CurrentCardIcon);
                spawnedEnemies.Add(resultEnemy);
                OnOnEnemySpawned?.Invoke(resultEnemy);
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }

        SpawnModelEntry PickRandomTrashMob(List<SpawnModelEntry> spawnModel)
        {
            float totalChance = 0;
            foreach (var t in spawnModel)
            {
                totalChance += t.ChanceToSpawn;
            }

            float currentChance = 0;
            var rng = Random.Range(0, totalChance);
            foreach (var t in spawnModel)
            {
                currentChance += t.ChanceToSpawn;
                if (rng <= currentChance)
                {
                    return t;
                }
            }


            return spawnModel[0];
        }

        public AiControllerBase SpawnMiniBoss(SpawnModel currentLvlModel)
        {
            var rng = Random.Range(0, currentLvlModel.MiniBosses.Count);
            var targetEntry = currentLvlModel.MiniBosses[rng];
            var targetPoints = spanwPointsCache[targetEntry.Prefab.AgentModel.EnemySpawmType];
            var rngPoint = Random.Range(0, targetPoints.Count);
            AiControllerBase resultEnemy = Instantiate(targetEntry.Prefab,
                targetPoints.ElementAt(rngPoint).GetSpawnPosition()
                , Quaternion.identity);
            resultEnemy.transform.parent = transform;
            spawnedEnemies.Add(resultEnemy);
            resultEnemy.Init(player.transform, monsterStatController.GetMonsterStatModifier, null);
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