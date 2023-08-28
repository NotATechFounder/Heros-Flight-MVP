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
        Dictionary<SpawnType, List<ISpawnPointInterface>> spawnPointsCache = new();
        MonsterStatController monsterStatController;
        Coroutine spawningWaveRoutine;
        Coroutine spawningRoutine;
        WaitForSeconds timeBetweenEnemySpawn;
        WaitForSeconds timeBeweenWaves;

        private bool spawningEnded = false;

        public void Init()
        {
            // To be removed
            monsterStatController = FindObjectOfType<MonsterStatController>();
        }

        public void SetSpawnPoints(Dictionary<SpawnType, List<ISpawnPointInterface>>  valuePairs)
        {
            spawnPointsCache = valuePairs;
        }

        public void SpawnEnemies(Level level, Action<AiControllerBase> OnOnEnemySpawned)
        {
            spawningEnded = false;
            spawningRoutine = StartCoroutine(SpawnNewLevelRoutine(level, OnOnEnemySpawned));
        }

        IEnumerator SpawnNewLevelRoutine(Level currentLevel, Action<AiControllerBase> OnOnEnemySpawned)
        {
            timeBeweenWaves = new WaitForSeconds(currentLevel.TimeBetweenWaves);

            while (!spawningEnded)
            {
                foreach (Wave wave in currentLevel.Waves)
                {
                    timeBetweenEnemySpawn = new WaitForSeconds(wave.TimeBetweenMobs);
                    yield return spawningWaveRoutine = StartCoroutine(NewSpawnWave(wave, wave.TotalMobsToSpawn, OnOnEnemySpawned));
                    yield return timeBeweenWaves;
                }
                OnLevelEnded?.Invoke();
                spawningEnded = true;
            }
        }

        IEnumerator NewSpawnWave(Wave wave, int enemiesToSpawn, Action<AiControllerBase> OnOnEnemySpawned)
        {
            if (wave.AvaliableMiniBosses.Count > 0)
            {
                yield return timeBetweenEnemySpawn;
                OnOnEnemySpawned?.Invoke(SpawnMiniBoss(wave));
            }

            for (var i = 0; i < enemiesToSpawn; i++)
            {
                SpawnModelEntry spawnModelEntry = PickRandomTrashMob(wave.AvaliableTrashMobs);
                SpawnSingleEnemy(spawnModelEntry, OnOnEnemySpawned);
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }

        public void SpawnSingleEnemy(SpawnModelEntry spawnModelEntry, Action<AiControllerBase> OnOnEnemySpawned)
        {
            var targetPoints = spawnPointsCache[spawnModelEntry.Prefab.AgentModel.EnemySpawmType];
            var rngPoint = Random.Range(0, targetPoints.Count);
            AiControllerBase resultEnemy = Instantiate(spawnModelEntry.Prefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition(), Quaternion.identity);
            resultEnemy.transform.parent = transform;
            resultEnemy.Init(player.transform, monsterStatController.GetMonsterStatModifier, monsterStatController.CurrentCardIcon);
            spawnedEnemies.Add(resultEnemy);
            OnOnEnemySpawned?.Invoke(resultEnemy);
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

        public AiControllerBase SpawnMiniBoss(Wave currentWave)
        {
            var rng = Random.Range(0, currentWave.AvaliableMiniBosses.Count);
            var targetEntry = currentWave.AvaliableMiniBosses[rng];
            var targetPoints = spawnPointsCache[targetEntry.Prefab.AgentModel.EnemySpawmType];
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