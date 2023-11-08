using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.UI.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Stats.Traits
{
    public class TestTraitUser : MonoBehaviour
    {
        private TraitHandler traitHandler;
        [SerializeField] private TraitTreeMenu menu;
        private void Awake()
        {
            traitHandler = new TraitHandler(new Vector2Int(6,4));
            menu.OnTraitModificationRequest += HandleRequest;
          
            menu.UpdateTreeView(traitHandler.GetFeatTreeData());
            menu.Open();
        }

        private void HandleRequest(TraitModificationEventModel request)
        {
            Debug.Log($"Got event for {request.Model.Id} and {request.ModificationType}");
            switch (request.ModificationType)
            {
                case TraitModificationType.Unlock:
                    if (traitHandler.TryUnlockFeat(request.Model.Id))
                    {
                        menu.UpdateTreeView(traitHandler.GetFeatTreeData());
                    }
                   
                    break;
                case TraitModificationType.Reroll:
                    var range = traitHandler.GetFeatEffectRange(request.Model.Id);
                    var rng = Random.Range(range.x, range.y);
                    if (traitHandler.ModifyTraitValue(request.Model.Id, rng))
                    {
                        menu.UpdateTreeView(traitHandler.GetFeatTreeData());
                    }
                    break;
               
            }
        }
    }
}