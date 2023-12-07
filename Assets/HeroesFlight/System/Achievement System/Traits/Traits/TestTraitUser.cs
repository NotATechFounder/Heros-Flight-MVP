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
            traitHandler = new TraitHandler(new Vector2Int(4,8));
            menu.OnTraitModificationRequest += HandleRequest;
          
            menu.UpdateTreeView(traitHandler.GetTraitTreeData());
            menu.Open();
        }

        private void HandleRequest(TraitModificationEventModel request)
        {
            Debug.Log($"Got event for {request.Model.Id} and {request.ModificationType}");
            switch (request.ModificationType)
            {
                case TraitModificationType.Unlock:
                    if (traitHandler.TryUnlockTrait(request.Model.Id))
                    {
                        menu.UpdateTreeView(traitHandler.GetTraitTreeData());
                    }
                   
                    break;
                case TraitModificationType.Reroll:
                    var effect = traitHandler.GetTraitEffect(request.Model.Id);
                    var rng = Random.Range(effect.ValueRange.x, effect.ValueRange.y);
                    if (traitHandler.TryModifyTraitValue(request.Model.Id, rng))
                    {
                        menu.UpdateTreeView(traitHandler.GetTraitTreeData());
                    }
                    break;
               
            }
        }
    }
}