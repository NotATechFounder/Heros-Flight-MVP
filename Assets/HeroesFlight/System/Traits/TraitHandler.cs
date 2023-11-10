using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.Stats.Traits;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
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

        public bool TryUnlockTrait(string id)
        {
            if (traitMap.TryGetValue(id, out var targetFeat) && !unlockedTraits.ContainsKey(targetFeat.Id))
            {
                Debug.Log($"Unlocking trait{targetFeat.Id}");
                unlockedTraits.Add(targetFeat.Id, new TraitStateModel(targetFeat, new IntValue(0)));
                return true;
            }

            return false;
        }

        public bool TryModifyTraitValue(string traitId, int value)
        {
            if (unlockedTraits.TryGetValue(traitId, out var stateValue))
            {
                stateValue.ModifyValue(new IntValue(value));
                return true;
            }

            Debug.LogError($"{traitId} is not unlocked");
            return false;
        }

        public bool HasTraitOfType(TraitType targetType, out List<TraitStateModel> models)
        {
            var hasTrait = false;
            models = new List<TraitStateModel>();
            foreach (var traitEntry in unlockedTraits)
            {
                if (traitEntry.Value.TargetTrait.Effect.TraitType == targetType)
                {
                    models.Add(traitEntry.Value);
                    
                }
            }


            return models.Count>0;
        }

        public TraitEffect GetTraitEffect(string id)
        {
            if (traitMap.TryGetValue(id, out var model))
            {
                return model.Effect;
            }

            Debug.LogError($"Trait with id {id} not found");
            return null;
        }

        IntValue GetTraitModificationValue(string traitId)
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


        public List<TraitStateModel> GetUnlockedTraits() => unlockedTraits.Values.ToList();


        public TraitTreeModel GetTraitTreeData()
        {
            var featTreeData = new Dictionary<string, TraitModel>();
            foreach (var pair in traitMap)
            {
                featTreeData.Add(pair.Key, GenerateFeatModel(pair.Value));
            }

            return new TraitTreeModel(size.y, size.x, featTreeData);
        }

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
            TraitModelState state;
            if (IsFeatUnlocked(targetTrait.DependantId))
            {
                state = TraitModelState.UnlockPossible;
            }
            else
            {
                state = TraitModelState.UnlockBlocked;
            }

            if (IsFeatUnlocked(targetTrait.Id))
            {
                state = TraitModelState.Unlocked;
            }


            return new TraitModel(targetTrait.Id, targetTrait.Tier, targetTrait.Slot, targetTrait.RequiredLvl,
                targetTrait.DependantId, targetTrait.Cost, targetTrait.Currency, state, targetTrait.Icon,
                targetTrait.Effect.Value,
                GetTraitModificationValue(targetTrait.Id).Value, targetTrait.Description,
                targetTrait.Effect.CanBeRerolled);
        }
    }
}