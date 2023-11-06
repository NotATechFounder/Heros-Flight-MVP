using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.Stats.Traits;
using UnityEngine;

namespace HeroesFlight.System.Stats.Handlers
{
    public class TraitHandler
    {

        public TraitHandler()
        {
            LoadCache();
        }
        private const string LOAD_FOLDER = "Traits/";
        private Dictionary<string, Trait> unlockedFeats = new();
        private Dictionary<string, Trait> featMap = new();
      
        public bool TryUnlockFeat(string id)
        {
            if (featMap.TryGetValue(id, out var targetFeat))
            {
                unlockedFeats.Add(targetFeat.Id,targetFeat);
                return true;
            }

            return false;

        }

        public bool IsFeatUnlocked(string id)
        {
            if (id.Equals(string.Empty))
                return true;
            
            return unlockedFeats.ContainsKey(id);
        }


        public List<Trait> GetUnlockedFeats() => unlockedFeats.Values.ToList();

         void LoadCache()
         {
             Debug.Log("Loading cache");
             var availableCache= Resources.LoadAll<Trait>( LOAD_FOLDER);
             Debug.Log(availableCache.Length);
             foreach (var feat in availableCache)
             {
                 Debug.Log($"adding feat with id {feat.Id}");
                 featMap.Add(feat.Id,feat);
             }
         }
         
         
         TraitModel GenerateFeatModel(Trait targetTrait)
         {
             return new TraitModel(targetTrait.Id, targetTrait.Tier, targetTrait.Slot, targetTrait.RequiredLvl, 
                 targetTrait.DependantId,targetTrait.GoldCost,IsFeatUnlocked(targetTrait.Id),
                 IsFeatUnlocked(targetTrait.DependantId),targetTrait.Icon);
         }

         public TraitTreeModel GetFeatTreeData()
         {
             var featTreeData = new Dictionary<string, TraitModel>();
             foreach (var pair in featMap)
             {
                 featTreeData.Add(pair.Key,GenerateFeatModel(pair.Value));
             }

             return  new TraitTreeModel(3,3,featTreeData);
         }
    }
}