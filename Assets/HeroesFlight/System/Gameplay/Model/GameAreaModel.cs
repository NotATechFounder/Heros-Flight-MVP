using System;
using System.Collections.Generic;
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
        [SerializeField] Level angelsGambitLevel;
        [SerializeField] private SpawnModel spawnModel;
        [SerializeField] private MobDifficulty[] mobDifficulty;

        public string AreaName => areaName;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public Level AngelsGambitLevel => angelsGambitLevel;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
    }
}

[Serializable]
public class MobDifficulty
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] StatSO healthStat;
    [SerializeField] StatSO damageStat;

    public EnemyType EnemyType => enemyType;
    public StatSO HealthStat => healthStat;
    public StatSO DamageStat => damageStat;
}