using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Stats.Feats;
using UnityEngine;

namespace HeroesFlight.System.Stats.Handlers
{
    public class FeatHandler
    {

        public FeatHandler()
        {
            LoadCache();
        }
        private const string LOAD_FOLDER = "Feats/";
        private Dictionary<string, Feat> unlockedFeats = new();
        private Dictionary<string, Feat> featMap = new();
      
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

         void LoadCache()
         {
             Debug.Log("Loading cache");
             var availableCache= Resources.LoadAll<Feat>( LOAD_FOLDER);
             Debug.Log(availableCache.Length);
             foreach (var feat in availableCache)
             {
                 Debug.Log($"adding feat with id {feat.Id}");
                 featMap.Add(feat.Id,feat);
             }
         }
    }
}