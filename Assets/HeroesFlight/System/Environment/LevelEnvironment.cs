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

    public ISpawnPointInterface GetSpawnpoint(SpawnType spawnType)
    {
        return spawnPointsCache[spawnType][Random.Range(0, spawnPointsCache[spawnType].Count)];
    }

    public List<ISpawnPointInterface> GetSpawnpoints(SpawnType spawnType)
    {
        if (!spawnPointsCache.ContainsKey(spawnType))
        {
          return  new();
        }
        return spawnPointsCache[spawnType];
    }
}
