using System;
using System.Collections.Generic;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class ThresholdSpawnModel
    {
        [SerializeField] private float healthThreshhold;
        [SerializeField] private List<AiControllerBase> mobsToSpawn = new();
        public float HealthThreshhold => healthThreshhold;
        public List<AiControllerBase> TargetsToSpawn => mobsToSpawn;
    }
}