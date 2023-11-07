using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.Stats.Traits;
using HeroesFlight.System.Stats.Traits.Model;
using UnityEngine;

namespace HeroesFlight.System.Stats.Handlers
{
    public class TraitHandler
    {
        public TraitHandler(Vector2Int featTreeSize)
        {
            size = featTreeSize;
            LoadCache();
        }

        private const string LOAD_FOLDER = "Traits/";
        private Dictionary<string, TraitStateModel> unlockedTraits = new();
        private Dictionary<string, Trait> traitMap = new();

        private Vector2Int size;

        public bool TryUnlockFeat(string id)
        {
            if (traitMap.TryGetValue(id, out var targetFeat))
            {
                unlockedTraits.Add(targetFeat.Id, new TraitStateModel(targetFeat, new IntValue(0)));
                return true;
            }

            return false;
        }

        public void ModifyTraitValue(string traitId, int value)
        {
            if (unlockedTraits.TryGetValue(traitId, out var stateValue))
            {
                stateValue.ModifyValue(new IntValue(value));
            }
            else
            {
                Debug.LogError($"{traitId} is not unlocked");
            }
        }

        IntValue GetFeatModificationValue(string traitId)
        {
            if (unlockedTraits.TryGetValue(traitId, out var stateValue))
            {
                return stateValue.Value;
            }

            Debug.LogError($"{traitId} is not unlocked");
            return new IntValue(0);
        }

        bool IsFeatUnlocked(string id)
        {
            if (id.Equals(string.Empty))
                return true;

            return unlockedTraits.ContainsKey(id);
        }


        public List<TraitStateModel> GetUnlockedFeats() => unlockedTraits.Values.ToList();

        void LoadCache()
        {
            Debug.Log("Loading cache");
            var availableCache = Resources.LoadAll<Trait>(LOAD_FOLDER);
            Debug.Log(availableCache.Length);
            foreach (var feat in availableCache)
            {
                Debug.Log($"adding feat with id {feat.Id}");
                traitMap.Add(feat.Id, feat);
            }
        }


        TraitModel GenerateFeatModel(Trait targetTrait)
        {
            return new TraitModel(targetTrait.Id, targetTrait.Tier, targetTrait.Slot, targetTrait.RequiredLvl,
                targetTrait.DependantId, targetTrait.GoldCost, IsFeatUnlocked(targetTrait.Id),
                IsFeatUnlocked(targetTrait.DependantId), targetTrait.Icon, targetTrait.Effect.Value,
                GetFeatModificationValue(targetTrait.Id).Value);
        }

        public TraitTreeModel GetFeatTreeData()
        {
            var featTreeData = new Dictionary<string, TraitModel>();
            foreach (var pair in traitMap)
            {
                featTreeData.Add(pair.Key, GenerateFeatModel(pair.Value));
            }

            return new TraitTreeModel(size.x, size.y, featTreeData);
        }
    }
}