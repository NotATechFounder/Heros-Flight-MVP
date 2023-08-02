using System;
using System.Collections.Generic;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class SpawnModel
    {
        [SerializeField] int wavesAmount;
        [SerializeField] int mobsToSpawn;
        [SerializeField] List<AiControllerBase> trashMobs;
        [SerializeField] List<AiControllerBase> miniBosses;
        [SerializeField] List<AiControllerBase> bosses;

        public int MobsAmount => mobsToSpawn;
        public int WavesAmount => wavesAmount;
        public List<AiControllerBase> TrashMobs => trashMobs;
        public List<AiControllerBase> MiniBosses => miniBosses;
        public List<AiControllerBase> Bosses => bosses;
    }
}