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
        private MobDifficultyHolder mobDifficulty;
        private int levelIndex;

        public void Init()
        {
            // To be removed
            monsterStatController = FindObjectOfType<MonsterStatController>();
        }

        public void SetMobDifficultyHolder (MobDifficultyHolder mobDifficulty)
        {
            this.mobDifficulty = mobDifficulty;
        }

        public void SetSpawnPoints(Dictionary<SpawnType, List<ISpawnPointInterface>>  valuePairs)
        {
            spawnPointsCache = valuePairs;
        }

        public void SpawnEnemies(Level level, int levelIndex, Action<AiControllerBase> OnOnEnemySpawned)
        {
            if (level.Waves.Length == 0) return;
            spawningEnded = false;
            this.levelIndex = levelIndex;
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
                SpawnModelEntry targetEntry = PickRandomMob(wave.AvaliableMiniBosses);
                SpawnSingleEnemy(targetEntry, OnOnEnemySpawned);
                yield return timeBetweenEnemySpawn;
            }

            for (var i = 0; i < enemiesToSpawn; i++)
            {
                SpawnModelEntry spawnModelEntry = PickRandomMob(wave.AvaliableTrashMobs);
                SpawnSingleEnemy(spawnModelEntry, OnOnEnemySpawned);
                yield return timeBetweenEnemySpawn;
            }

            yield return true;
        }

        public void SpawnSingleEnemy(SpawnModelEntry spawnModelEntry, Action<AiControllerBase> OnOnEnemySpawned)
        {
            List<ISpawnPointInterface> targetPoints = spawnPointsCache[spawnModelEntry.Prefab.AgentModel.EnemySpawmType];

            if (targetPoints == null || targetPoints.Count == 0)
            {
                Debug.LogError("No spawn points for " + spawnModelEntry.Prefab.name);
                return;
            }

            var rngPoint = Random.Range(0, targetPoints.Count);
            AiControllerBase resultEnemy = Instantiate(spawnModelEntry.Prefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition(), Quaternion.identity);
            resultEnemy.transform.parent = transform;

            if(resultEnemy.EnemyType == EnemyType.MiniBoss)
            {
                resultEnemy.Init(player.transform,mobDifficulty.GetHealth(levelIndex, resultEnemy.EnemyType), mobDifficulty.GetDamage(levelIndex, resultEnemy.EnemyType), 
                    monsterStatController.GetMonsterStatModifier, null);
            }
            else
            {
                resultEnemy.Init(player.transform, mobDifficulty.GetHealth(levelIndex, resultEnemy.EnemyType), mobDifficulty.GetDamage(levelIndex, resultEnemy.EnemyType),
                    monsterStatController.GetMonsterStatModifier, monsterStatController.CurrentCardIcon);
            }

            spawnedEnemies.Add(resultEnemy);
            OnOnEnemySpawned?.Invoke(resultEnemy);
        }

        SpawnModelEntry PickRandomMob(List<SpawnModelEntry> spawnModel)
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

[Serializable]
public class MobDifficultyHolder
{
    [SerializeField] MobDifficulty[] mobDifficulties;

    public MobDifficulty[] GetMobDifficulties => mobDifficulties;

    public int GetHealth(int level, EnemyType enemyType)
    {
       return mobDifficulties.FirstOrDefault(x => x.EnemyType == enemyType).HealthStat.GetCurrentValue(level);
    }

    public int GetDamage(int level, EnemyType enemyType)
    {
        return mobDifficulties.FirstOrDefault(x => x.EnemyType == enemyType).DamageStat.GetCurrentValue(level);
    }
}

[Serializable]
public class MobDifficulty
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] CustomAnimationCurve healthStat;
    [SerializeField] CustomAnimationCurve damageStat;

    public EnemyType EnemyType => enemyType;
    public CustomAnimationCurve HealthStat => healthStat;
    public CustomAnimationCurve DamageStat => damageStat;
}