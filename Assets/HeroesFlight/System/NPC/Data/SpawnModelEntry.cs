using System;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Data
{
    [Serializable]
    public class SpawnModelEntry
    {
        [SerializeField] AiControllerBase aiPrefab;
        [Range(0,100)]
        [SerializeField] int spawnChance;

        public AiControllerBase Prefab => aiPrefab;
        public int ChanceToSpawn => spawnChance;
    }
}