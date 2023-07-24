using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.NPC.Container
{
    public class NpcContainer : MonoBehaviour
    {
        [SerializeField] AiControllerBase[] aiPrefabs;
        [SerializeField] int spawnAmount = 10;
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
            SpawnEnemies();
        }

        void SpawnEnemies()
        {
            for (var i = 0; i <= spawnAmount; i++)
            {
                var rng = Random.Range(0, aiPrefabs.Length);

                var targetPrefab = aiPrefabs[rng];
                var targetPoints = spanwPointsCache[targetPrefab.AgentModel.EnemySpawmType];
                var rngPoint=  Random.Range(0, targetPoints.Count);
                var resultEnemy = Instantiate(targetPrefab, targetPoints.ElementAt(rngPoint).GetSpawnPosition()
                    , Quaternion.identity);
                resultEnemy.Init(player.transform);
            }
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
    }
}