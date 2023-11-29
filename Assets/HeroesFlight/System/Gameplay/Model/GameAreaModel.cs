
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
        [SerializeField] private Environment.Objects.Crystal crystalPrefab;
        [SerializeField] Level angelsGambitLevel;
        [SerializeField] BoosterDropSO bossDrop;
        [SerializeField] private string bossMusicKey;

        [SerializeField] private SpawnModel spawnModel;
        [SerializeField] private MobDifficultyHolder mobDifficulty;
        [SerializeField] private MobDropTableHolder mobDropTable;

        [SerializeField] private CustomAnimationCurve levelComplectionExpCurve;
        [SerializeField] private CustomAnimationCurve runComplectionExpCurve;

        [Header("Time Stop Testing")]
        [SerializeField] float timeStopRestoreSpeed;
        [SerializeField] float timeStopDuration;
       
        public string AreaName => areaName;
        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;
        public Level AngelsGambitLevel => angelsGambitLevel;
        public BoosterDropSO BossDrop => bossDrop;
        public string WorldBossMusicKey => bossMusicKey;
        public SpawnModel SpawnModel => spawnModel;
        public LevelPortal PortalPrefab => portalPrefab;
        public Environment.Objects.Crystal CrystalPrefab => crystalPrefab;
        public MobDifficultyHolder MobDifficulty => mobDifficulty;
        public MobDropTableHolder MobDropTableHolder => mobDropTable;

        public float TimeStopRestoreSpeed => timeStopRestoreSpeed;
        public float TimeStopDuration => timeStopDuration;
    }
}