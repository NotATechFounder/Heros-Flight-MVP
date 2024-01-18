
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    [CreateAssetMenu(fileName = "GameAreaModel", menuName = "Model/GameAreaModel", order = 0)]
    public class GameAreaModel : ScriptableObject
    {
        [SerializeField] private WorldType worldType;   
        [SerializeField] float heroProgressionExpEarnedPerKill = 20f;
        [SerializeField] LevelPortal portalPrefab;
        [SerializeField] private Environment.Objects.Crystal crystalPrefab;
        [SerializeField] Level angelsGambitLevel;
        [SerializeField] BoosterDropSO bossDrop;
        [SerializeField] private string bossMusicKey;

        [SerializeField] private SpawnModel spawnModel;
        [SerializeField] private MobDifficultyHolder mobDifficulty;

        [Header("Xp")]
        [SerializeField] private InRunXp inRunXp;
        [SerializeField] private int permanentXpPerRoom;

        [Header("Rewards")]
        [SerializeField] private CustomAnimationCurve runShardCurve;
       
        public WorldType WorldType => worldType;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public Level ShrineLevel => angelsGambitLevel;
        public BoosterDropSO BossDrop => bossDrop;
        public string WorldBossMusicKey => bossMusicKey;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
        public Environment.Objects.Crystal CrystalPrefab => crystalPrefab;
        public MobDifficultyHolder MobDifficulty => mobDifficulty;

        public InRunXp InRunXp => inRunXp;

        public CustomAnimationCurve RunShardCurve => runShardCurve;

        public int PermanentXpPerRoom => permanentXpPerRoom;
        public float TimeStopRestoreSpeed => timeStopRestoreSpeed;
        public float TimeStopDuration => timeStopDuration;
        public int RuneShardsPerEnemy => runeShardsPerEnemy;
    }
}


[System.Serializable]
public class InRunXp
{
    public InRunXpEntry[] inRunXpEntries;
}

[System.Serializable]
public struct InRunXpEntry
{
    public LevelType LevelType;
    public int xp;
}