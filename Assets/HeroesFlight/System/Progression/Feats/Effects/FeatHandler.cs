using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    public class FeatHandler
    {
        private const string LOAD_FOLDER = "/Feats/";
        private Dictionary<string, Feat> unlockedFeats = new();
        private Dictionary<string, Feat> featMap = new();

        public void LoadCache()
        {
            var availableCache= Resources.LoadAll<Feat>( LOAD_FOLDER);
            foreach (var feat in availableCache)
            {
                featMap.Add(feat.Id,feat);
            }
        }
        
        
        public void UnlockFeat(string id)
        {
            if (unlockedFeats.ContainsKey(id))
            {
               Debug.Log($"Feat with  id : {id} is already unlocked");
               return;
            }
            
            if (featMap.TryGetValue(id, out var targetFeat))
            {
                unlockedFeats.Add(targetFeat.Id,targetFeat);
            }
            
        }


        public List<Feat> GetUnlockedFeats() => unlockedFeats.Values.ToList();
    }
}