using System.Collections.Generic;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    [CreateAssetMenu(fileName = "GameLoopModel", menuName = "Model/GameLoop", order = 0)]
    public class GameLoopModel : ScriptableObject
    {
        [SerializeField] Vector2 portalPosition;
        [SerializeField] List<SpawnModel> avaibleLvls=new ();

        void OnValidate()
        {
            if (Models.Count == 0 && avaibleLvls.Count != 0)
            {
                GenerateCache();
            }
        }

        void GenerateCache()
        {
            for (int i = 0; i < avaibleLvls.Count; i++)
            {
                Models.Add(i,avaibleLvls[i]);
            }
        }

        public Dictionary<int, SpawnModel> Models = new();
        public Vector2 PortalSpawnPosition => portalPosition;
    }
}