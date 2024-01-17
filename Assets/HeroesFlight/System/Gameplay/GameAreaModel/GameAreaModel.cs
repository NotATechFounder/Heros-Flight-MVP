
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
        [SerializeField] private MobDropTableHolder mobDropTable;

        [SerializeField] private CustomAnimationCurve levelComplectionExpCurve;
        [SerializeField] private CustomAnimationCurve runComplectionExpCurve;
        [SerializeField] private CustomAnimationCurve reRunComplectionExpCurve;

        [Header("Time Stop Testing")]
        [SerializeField] float timeStopRestoreSpeed;
        [SerializeField] float timeStopDuration;

        [Header("Drop values")] 
        [SerializeField] private int runeShardsPerEnemy = 5;
       
        public WorldType WorldType => worldType;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public Level ShrineLevel => angelsGambitLevel;
        public BoosterDropSO BossDrop => bossDrop;
        public string WorldBossMusicKey => bossMusicKey;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
        public Environment.Objects.Crystal CrystalPrefab => crystalPrefab;
        public MobDifficultyHolder MobDifficulty => mobDifficulty;
        public MobDropTableHolder MobDropTableHolder => mobDropTable;

        public CustomAnimationCurve LevelComplectionExpCurve => levelComplectionExpCurve;

        public CustomAnimationCurve InRunComplectionExpCurve => runComplectionExpCurve;

        public float TimeStopRestoreSpeed => timeStopRestoreSpeed;
        public float TimeStopDuration => timeStopDuration;

        public int RuneShardsPerEnemy => runeShardsPerEnemy;
    }
}