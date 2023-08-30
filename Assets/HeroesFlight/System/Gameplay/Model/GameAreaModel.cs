using System;
using System.Collections.Generic;
using HeroesFlight.System.NPC.Data;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    [CreateAssetMenu(fileName = "GameAreaModel", menuName = "Model/GameAreaModel", order = 0)]
    public class GameAreaModel : ScriptableObject
    {
        [SerializeField] private string areaName;
        [SerializeField] float heroProgressionExpEarnedPerKill = 20f;
        [SerializeField] LevelPortal portalPrefab;
        [SerializeField] private SpawnModel spawnModel;

        public string AreaName => areaName;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
    }
}