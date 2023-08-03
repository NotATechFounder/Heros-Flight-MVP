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
        [SerializeField] List<SpawnModelEntry> trashMobs;
        [SerializeField] List<SpawnModelEntry> miniBosses;
        [SerializeField] List<SpawnModelEntry> bosses;

        public int MobsAmount => mobsToSpawn;
        public int WavesAmount => wavesAmount;
        public List<SpawnModelEntry> TrashMobs => trashMobs;
        public List<SpawnModelEntry> MiniBosses => miniBosses;
        public List<SpawnModelEntry> Bosses => bosses;
    }
}