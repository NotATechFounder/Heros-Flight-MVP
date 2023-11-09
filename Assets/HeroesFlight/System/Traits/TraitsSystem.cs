using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
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
            Debug.Log("created");
            data = dataSystem;
            this.uiSystem = uiSystem;
            traitHandler = new TraitHandler(new Vector2Int(5,3));
           
        }

        private DataSystemInterface data;

        private IUISystem uiSystem;

        private TraitHandler traitHandler;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            Debug.Log("Inited");
            uiSystem.UiEventHandler.MainMenu.OnTraitButtonPressed += HandleTraitButtonPressed;
            uiSystem.UiEventHandler.TraitTreeMenu.OnTraitModificationRequest += HandleRequest;
        }

        public void Reset() { }

         void HandleTraitButtonPressed()
        {
            Debug.Log("TraitButtonPRessed");
            uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetFeatTreeData());
            uiSystem.UiEventHandler.TraitTreeMenu.Open();
        }

        void HandleRequest(TraitModificationEventModel request)
        {
            Debug.Log($"Got event for {request.Model.Id} and {request.ModificationType}");
            switch (request.ModificationType)
            {
                case TraitModificationType.Unlock:
                    if (traitHandler.TryUnlockFeat(request.Model.Id))
                    {
                        uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetFeatTreeData());
                    }
                   
                    break;
                case TraitModificationType.Reroll:
                    var effect = traitHandler.GetFeatEffect(request.Model.Id);
                    var rng = Random.Range(effect.ValueRange.x, effect.ValueRange.y);
                    if (traitHandler.TryModifyTraitValue(request.Model.Id, rng))
                    {
                        uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetFeatTreeData());
                    }
                    break;
               
            }
        }
    }
}