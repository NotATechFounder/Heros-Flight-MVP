using System;
using System.Collections.Generic;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlight.System.NPC.Data;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    [CreateAssetMenu(fileName = "GameAreaModel", menuName = "Model/GameAreaModel", order = 0)]
    public class GameAreaModel : ScriptableObject
    {
        [SerializeField] private string areaName;
        [SerializeField] float heroProgressionExpEarnedPerKill = 20f;
        [SerializeField] LevelPortal portalPrefab;
        [SerializeField] private Crystal crystalPrefab;
        [SerializeField] Level angelsGambitLevel;
        [SerializeField] private BossControllerBase worldBoss;
        [SerializeField] private SpawnModel spawnModel;
        [SerializeField] private MobDifficultyHolder mobDifficulty;


        public string AreaName => areaName;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public Level AngelsGambitLevel => angelsGambitLevel;
        public BossControllerBase WorldBoss => worldBoss;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
        public Crystal CrystalPrefab => crystalPrefab;
        public MobDifficultyHolder MobDifficulty => mobDifficulty;
    }
}