using System;
using HeroesFlight.System.FileManager.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.FileManager.Model
{
    [Serializable]
    public class TraitModel
    {
        public TraitModel()
        {
            Id = string.Empty;
        }
        public TraitModel(string id, int tier, int slot, int requiredLvl, string blockingId, int cost,CurrencySO targetCurrency,
            TraitModelState currentState, Sprite visual, int baseValue,int currentValue,string description,bool canBeRerolled)
        {
            Id = id;
            Tier = tier;
            Slot = slot;
            RequiredLvl = requiredLvl;
            BlockingId = blockingId;
            Cost = cost;
            State = currentState;
            Visual = visual;
            BaseValue = baseValue;
            CurrentValue = currentValue;
            CanBeRerolled = canBeRerolled;
            TargetCurrency = targetCurrency;
        }

        public string Id { get; }
        public int BaseValue{ get; }
        public int CurrentValue{ get; }
        public int RequiredLvl{ get; }
        public string BlockingId{ get; }
        public int Tier{ get; }
        public int Slot{ get; }
        public int Cost{ get; }
        public TraitModelState State{ get; }
        public Sprite Visual{ get; }
        public string Description{ get; }
        public bool CanBeRerolled{ get; }
        public CurrencySO TargetCurrency { get; }
    }
}