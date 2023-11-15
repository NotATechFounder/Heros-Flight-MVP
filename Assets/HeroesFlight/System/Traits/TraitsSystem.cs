using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
using HeroesFlight.System.Dice;
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
        public TraitsSystem(DataSystemInterface dataSystem, IUISystem uiSystem, DiceSystemInterface diceSystemInterface)
        {
            data = dataSystem;
            this.uiSystem = uiSystem;
            diceSystem = diceSystemInterface;
            traitHandler = new TraitHandler(new Vector2Int(4, 8));
        }

        private DataSystemInterface data;
        private DiceSystemInterface diceSystem;
        private IUISystem uiSystem;

        private TraitHandler traitHandler;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            uiSystem.UiEventHandler.MainMenu.OnTraitButtonPressed += HandleTraitButtonPressed;
            uiSystem.UiEventHandler.TraitTreeMenu.OnTraitModificationRequest += HandleRequest;
            uiSystem.UiEventHandler.TraitTreeMenu.DiceInfoRequest += () =>
            {
                uiSystem.UiEventHandler.DiceMenu.ShowDiceInfo(
                    "Roll the 12-sided dice to further enhance any of your traits between 1-12 bonus points. " +
                    "Each roll will reset the previous bonus");
            };
        }

        public void Reset()
        {
        }

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
                    if (data.CurrencyManager.GetCurrecy(request.Model.TargetCurrency.GetKey).GetCurrencyAmount <
                        request.Model.Cost)
                    {
                        uiSystem.UiEventHandler.TraitTreeMenu.ShowErrorMessage(
                            $"Not enough {request.Model.TargetCurrency.GetCurrencyName}");
                        return;
                    }


                    if (traitHandler.TryUnlockTrait(request.Model.Id))
                    {
                        data.CurrencyManager.GetCurrecy(request.Model.TargetCurrency.GetKey)
                            .ReduceCurrency(request.Model.Cost);
                        uiSystem.UiEventHandler.MainMenu.UpdateGoldText(data.CurrencyManager
                            .GetCurrecy(request.Model.TargetCurrency.GetKey).GetCurrencyAmount);
                        uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetTraitTreeData());
                    }

                    break;
                case TraitModificationType.Reroll:
                    uiSystem.UiEventHandler.DiceMenu.ShowDiceMenu(request.Model.CurrentValue, () =>
                    {
                        diceSystem.RollDice((rolledValue) =>
                        {
                            if (traitHandler.TryModifyTraitValue(request.Model.Id, rolledValue))
                            {
                                uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(traitHandler.GetTraitTreeData());
                                uiSystem.UiEventHandler.DiceMenu.ModifyDiceRollResultUi($"+{rolledValue}");
                            }
                        });
                    });


                    break;
            }
        }


        public bool HasTraitOfType(TraitType targetType, out List<TraitStateModel> models)
        {
            return traitHandler.HasTraitOfType(targetType, out models);
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