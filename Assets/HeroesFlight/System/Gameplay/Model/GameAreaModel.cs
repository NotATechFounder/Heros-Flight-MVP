using System.Collections.Generic;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    [CreateAssetMenu(fileName = "GameAreaModel", menuName = "Model/GameAreaModel", order = 0)]
    public class GameAreaModel : ScriptableObject
    {
        [SerializeField] Vector2 portalPosition;
        [SerializeField] List<SpawnModel> avaibleLvls=new ();

        public void Init()
        {
            OnValidate();
        }
        
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