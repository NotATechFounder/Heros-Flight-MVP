using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.Dice;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.Stats.Traits.Model;
using HeroesFlight.System.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Stats.Handlers
{
    public class TraitsSystem : TraitSystemInterface
    {
        public TraitsSystem(DataSystemInterface dataSystem, IUISystem uiSystem, DiceSystemInterface diceSystemInterface)
        {
            data = dataSystem;
            data.OnApplicationQuit += SaveTraitData;
            this.uiSystem = uiSystem;
            diceSystem = diceSystemInterface;
            traitHandler = new TraitHandler(new Vector2Int(4, 8), dataSystem);
            LoadData();
        }


        public event Action<Dictionary<StatAttributeType, int>> OnTraitsStateChange;
        private DataSystemInterface data;
        private DiceSystemInterface diceSystem;
        private IUISystem uiSystem;

        private TraitHandler traitHandler;
        private const string SaveKey = "Traits";


        public void Init(Scene scene = default, Action onComplete = null)
        {
            uiSystem.UiEventHandler.MainMenu.OnNavigationButtonClicked += MainMenu_OnNavigationButtonClicked;
            uiSystem.UiEventHandler.TraitTreeMenu.OnTraitModificationRequest += HandleRequest;
            uiSystem.UiEventHandler.TraitTreeMenu.DiceInfoRequest += () =>
            {
                uiSystem.UiEventHandler.DiceMenu.ShowDiceInfo(
                    "Roll the 12-sided dice to further enhance any of your traits between 1-12 bonus points. " +
                    "Each roll will reset the previous bonus");
            };
        }

        private void MainMenu_OnNavigationButtonClicked(UISystem.MenuNavigationButtonType obj)
        {
            if (obj == UISystem.MenuNavigationButtonType.Traits)
            {
                HandleTraitButtonPressed();
            }
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
                        NotifyTraitStateChanged();
                    }

                    break;
                case TraitModificationType.Reroll:
                    uiSystem.UiEventHandler.DiceMenu.ShowDiceMenu(request.Model.CurrentValue,
                        data.CurrencyManager.GetCurrecy(CurrencyKeys.Gem).GetCurrencyAmount >= diceSystem.DiceCost,
                        () =>
                        {
                            diceSystem.RollDice((rolledValue) =>
                            {
                                if (traitHandler.TryModifyTraitValue(request.Model.Id, rolledValue))
                                {
                                    uiSystem.UiEventHandler.TraitTreeMenu.UpdateTreeView(
                                        traitHandler.GetTraitTreeData());
                                    uiSystem.UiEventHandler.DiceMenu.ModifyDiceRollResultUi($"+{rolledValue}");
                                    NotifyTraitStateChanged();
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

        public Dictionary<StatAttributeType, int> GetUnlockedEffects()
        {
            var unlockedTraits = traitHandler.GetUnlockedTraits();
            var modifiedStatsMap = new Dictionary<StatAttributeType, int>();
            foreach (var trait in unlockedTraits)
            {
                if (trait.TargetTrait.Effect.TraitType == TraitType.StatBoost)
                {
                    var effect = trait.TargetTrait.Effect as StatBoostEffect;
                    var modificationValue = effect.Value + trait.Value.Value;
                    Debug.Log($"Gona update {effect.TargetStat} with value {effect.Value + trait.Value.Value}");
                    if (modifiedStatsMap.TryGetValue(effect.TargetStat, out var currentValue))
                    {
                        modifiedStatsMap[effect.TargetStat] += modificationValue;
                    }
                    else
                    {
                        modifiedStatsMap.Add(effect.TargetStat, modificationValue);
                    }
                }
            }

            return modifiedStatsMap;
        }

        public void UnlockAllTraits()
        {
           traitHandler.UnlockAllTraits();
        }

        public void LoadData()
        {
            var saveData = FileManager.FileManager.Load<TraitsMapSaveModel>(SaveKey);
            if (saveData != null)
            {
                traitHandler.LoadData(saveData.savedModels);
            }
        }


        void NotifyTraitStateChanged()
        {
            Debug.Log("Notifying traits changed");
            OnTraitsStateChange?.Invoke(GetUnlockedEffects());
        }

        void SaveTraitData()
        {
            var unlockedTraits = traitHandler.GetUnlockedTraits();

            var saveData = new TraitsMapSaveModel();
            foreach (var model in unlockedTraits)
            {
                saveData.savedModels.Add(new TraitSaveModel(model.TargetTrait.Id, model.Value.Value));
            }

            FileManager.FileManager.Save(SaveKey, saveData);
        }
    }
}