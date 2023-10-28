using System;
using System.Collections.Generic;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlight.System.NPC.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class SpawnModel
    {
        [SerializeField] private Level[] levels;
        public Level[] Levels => levels;
    }

    [Serializable]
    public class Level
    {
        [SerializeField] private LevelType levelType = LevelType.NormalCombat;
        [SerializeField] float lvlDuration = 120f;
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private float timeBetweenWaves = 3f;
        [SerializeField] private Wave[] waves;

        public LevelType LevelType => levelType;
        public GameObject LevelPrefab => levelPrefab;
        public float TimeBetweenWaves => timeBetweenWaves;
        public Wave[] Waves => waves;
        public float LevelDuration => lvlDuration;

        public int TotalMobsToSpawn
        {
            get
            {
                int totalMobs = 0;
                foreach (var wave in waves)
                {
                    totalMobs += wave.TotalMobsToSpawn;
                }
                return totalMobs;
            }
        }

        public bool MiniHasBoss
        {
            get
            {
                foreach (var wave in waves)
                {
                    if (wave.AvaliableMiniBosses.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public bool HasBoss
        {
            get
            {
                foreach (var wave in waves)
                {
                    if (wave.Boss!=null)
                        return true;
                }
                return false;
            }
        }
    }

    [Serializable]
    public class Wave
    {
        [SerializeField] int totalMobsToSpawn;
        [SerializeField] float timeBetweenMobs = .5f;
        [SerializeField] List<SpawnModelEntry> avaliableTrashMobs = new List<SpawnModelEntry>();
        [SerializeField] List<SpawnModelEntry> avaliableMiniBosses = new List<SpawnModelEntry>();
        [SerializeField] BossControllerBase boss ;

        public int TotalMobsToSpawn => totalMobsToSpawn;
        public float TimeBetweenMobs => timeBetweenMobs;
        public List<SpawnModelEntry> AvaliableTrashMobs => avaliableTrashMobs;
        public List<SpawnModelEntry> AvaliableMiniBosses => avaliableMiniBosses;
        public BossControllerBase Boss => boss;
    }

    public enum LevelType
    {
        NormalCombat,
        Intermission,
        WorldBoss
    }
}
