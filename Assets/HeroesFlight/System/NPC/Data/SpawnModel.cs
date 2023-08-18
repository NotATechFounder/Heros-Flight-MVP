using System;
using System.Collections.Generic;
using HeroesFlight.System.NPC.Data;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class SpawnModel
    {
        [SerializeField] int wavesAmount;
        [SerializeField] int mobsToSpawn;
        [SerializeField] float timeBetweenWaves = 3f;
        [SerializeField] float timeBetweenMobs = .5f;
        [SerializeField] List<SpawnModelEntry> trashMobs = new List<SpawnModelEntry>();
        [SerializeField] List<SpawnModelEntry> miniBosses = new List<SpawnModelEntry>();
        [SerializeField] List<SpawnModelEntry> bosses = new List<SpawnModelEntry>();

        public int MobsAmount => mobsToSpawn;
        public int WavesAmount => wavesAmount;
        public float TimeBetweenWaves => timeBetweenWaves;
        public float TimeBetweenMobs => timeBetweenMobs;
        public List<SpawnModelEntry> TrashMobs => trashMobs;
        public List<SpawnModelEntry> MiniBosses => miniBosses;
        public List<SpawnModelEntry> Bosses => bosses;
    }
}