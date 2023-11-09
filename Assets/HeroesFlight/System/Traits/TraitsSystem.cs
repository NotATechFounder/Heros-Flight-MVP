using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.Stats.Traits.Model;
using HeroesFlight.System.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Stats.Handlers
{
    public class TraitsSystem : TraitSystemInterface
    {

        public TraitsSystem(DataSystemInterface dataSystem, IUISystem uiSystem)
        {
            data = dataSystem;
            this.uiSystem = uiSystem;
            traitHandler = new TraitHandler(new Vector2Int(5,3));
           
        }

        private DataSystemInterface data;

        private IUISystem uiSystem;

        private TraitHandler traitHandler;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            uiSystem.UiEventHandler.MainMenu.OnTraitButtonPressed += HandleTraitButtonPressed;
            uiSystem.UiEventHandler.TraitTreeMenu.OnTraitModificationRequest += HandleRequest;
        }

        public void Reset() { }

         void HandleTraitButtonPressed()
        {
            uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetTraitTreeData());
            uiSystem.UiEventHandler.TraitTreeMenu.Open();
        }

        void HandleRequest(TraitModificationEventModel request)
        {
            Debug.Log($"Got event for {request.Model.Id} and {request.ModificationType}");
            switch (request.ModificationType)
            {
                case TraitModificationType.Unlock:
                    if (traitHandler.TryUnlockTrait(request.Model.Id))
                    {
                        uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetTraitTreeData());
                    }
                   
                    break;
                case TraitModificationType.Reroll:
                    var effect = traitHandler.GetTraitEffect(request.Model.Id);
                    var rng = Random.Range(effect.ValueRange.x, effect.ValueRange.y);
                    if (traitHandler.TryModifyTraitValue(request.Model.Id, rng))
                    {
                        uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetTraitTreeData());
                    }
                    break;
               
            }
        }

       
        public bool HasTraitOfType(TraitType targetType, out string id)
        {
            return traitHandler.HasTraitOfType(targetType, out id);
        }

        public TraitEffect GetTraitEffect(string id)
        {
            return traitHandler.GetTraitEffect(id);
        }

        public List<TraitStateModel> GetUnlockedEffects()
        {
            return traitHandler.GetUnlockedTraits();
        }
    }
}