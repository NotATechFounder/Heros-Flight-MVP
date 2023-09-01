using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnvironment : MonoBehaviour
{
    [SerializeField] PolygonCollider2D boundsCollider;
    [SerializeField] Transform spawnpointHolder;
    [SerializeField] InteractiveNPC interactiveNPC;
    Dictionary<SpawnType, List<ISpawnPointInterface>> spawnPointsCache = new();
    SpawnPoint[] spawnPoints;

    public PolygonCollider2D BoundsCollider => boundsCollider;

    public Dictionary<SpawnType, List<ISpawnPointInterface>> SpawnPointsCache => spawnPointsCache;

    public InteractiveNPC InteractiveNPC => interactiveNPC;

    private void Awake()
    {
        spawnPoints = spawnpointHolder.GetComponentsInChildren<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!spawnPointsCache.ContainsKey(spawnPoint.SpawnType))
            {
                spawnPointsCache.Add(spawnPoint.SpawnType, new List<ISpawnPointInterface>());
            }
            spawnPointsCache[spawnPoint.SpawnType].Add(spawnPoint);
        }
    }

    public List<GameObject> SpawnCrystals(GameObject cyrtsalPrefab)
    {
        List<GameObject> spawnedCrystals = new List<GameObject>();
        foreach (ISpawnPointInterface spawnPoint in spawnPointsCache[SpawnType.Crystal])
        {
            spawnedCrystals.Add(ObjectPoolManager.SpawnObject(cyrtsalPrefab, spawnPoint.GetSpawnPosition(), Quaternion.identity));
        }
        return spawnedCrystals;
    }

    public ISpawnPointInterface GetSpawnpoint(SpawnType spawnType)
    {
        return spawnPointsCache[spawnType][Random.Range(0, spawnPointsCache[spawnType].Count)];
    }

    public List<ISpawnPointInterface> GetSpawnpoints(SpawnType spawnType)
    {
        return spawnPointsCache[spawnType];
    }
}
