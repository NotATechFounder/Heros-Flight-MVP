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
        public Action OnLevelEnded;

        GameObject player;

        List<AiControllerBase> spawnedEnemies = new();
        Dictionary<EnemySpawmType, List<ISpawnPointInterface>> spanwPointsCache = new();
        MonsterStatController monsterStatController;
        Coroutine spawningWaveRoutine;
        Coroutine spawningRoutine;
        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;

        private int currentLevelIndex = 0;
        private int currentWaveIndex = 0;
        private int spawnAmount;
        private bool spawningEnded = false;

        public void Init()
        {
            GenerateCache();
        }

        public void SpawnEnemies(Level level, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawningRoutine = StartCoroutine(SpawnNewLevelRoutine(level, OnOnEnemySpawned));
        }

        IEnumerator SpawnNewLevelRoutine(Level currentLevel, Action<AiControllerBase> OnOnEnemySpawned)
        {
            timeBeweenWaves = new WaitForSeconds(currentLevel.TimeBetweenWaves);

            timeBetweenEnemySpawn = new WaitForSeconds(currentLevel.Waves[currentWaveIndex].TimeBetweenMobs);

            while (!spawningEnded)
            {
                foreach (Wave wave in currentLevel.Waves)
                {
                    timeBetweenEnemySpawn = new WaitForSeconds(wave.TimeBetweenMobs);
                    yield return spawningWaveRoutine = StartCoroutine(NewSpawnWave(currentLevel, wave.TotalMobsToSpawn, OnOnEnemySpawned));
                    yield return timeBeweenWaves;
                }
                OnLevelEnded?.Invoke();
                spawningEnded = true;
            }
        }

        IEnumerator NewSpawnWave(Level currentLevel, int enemiesToSpawn, Action<AiControllerBase> OnOnEnemySpawned)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                SpawnModelEntry spawnModelEntry = PickRandomTrashMob(currentLevel.Waves[currentWaveIndex].AvaliableTrashMobs);
                SpawnSingleEnemy(spawnModelEntry, OnOnEnemySpawned);
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }

        public void SpawnSingleEnemy(SpawnModelEntry spawnModelEntry, Action<AiControllerBase> OnOnEnemySpawned)
        {
            var targetPoints = spanwPointsCache[spawnModelEntry.Prefab.AgentModel.EnemySpawmType];
            var rngPoint = Random.Range(0, targetPoints.Count);
            AiControllerBase resultEnemy = Instantiate(spawnModelEntry.Prefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition(), Quaternion.identity);
            resultEnemy.transform.parent = transform;
            resultEnemy.Init(player.transform, monsterStatController.GetMonsterStatModifier, monsterStatController.CurrentCardIcon);
            spawnedEnemies.Add(resultEnemy);
            OnOnEnemySpawned?.Invoke(resultEnemy);
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

        public AiControllerBase SpawnMiniBoss(Level currentLevel)
        {
            var rng = Random.Range(0, currentLevel.Waves[currentWaveIndex].AvaliableMiniBosses.Count);
            var targetEntry = currentLevel.Waves[currentWaveIndex].AvaliableMiniBosses[rng];
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