using System;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    [SerializeField] AngelEffectManager angelEffectManager;
    [SerializeField] Healer healer;
    [SerializeField] private ShrineNPC[] shrineNPCs;
    private CurrencyManager currencyManager;

    public void Initialized(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
        AssignShrineEffects();
    }

    public void AssignShrineEffects()
    {
        foreach (ShrineNPC shrineNPC in shrineNPCs)
        {
            switch (shrineNPC.GetShrineNPCType())
            {
                case ShrineNPCType.AngelsGambit:
                    shrineNPC.OnPurchaseSuccessful = () => angelEffectManager.TriggerAngelsGambit();
                    break;
                case ShrineNPCType.ActiveAbilityReRoller:

                    break;
                case ShrineNPCType.PassiveAbilityReRoller:
                   
                    break;
                case ShrineNPCType.HealingMagicRune:
                    shrineNPC.OnPurchaseSuccessful = () => healer.Heal();
                    break;
                default: break;
            }
        }
    }

    public void UnlockNpc(ShrineNPCType shrineNPCFeeType)
    {
        ShrineNPC shrineNPC = GetShrineNPC(shrineNPCFeeType);
        shrineNPC.Unlock();
    }

    public void Purchase(ShrineNPCType shrineNPCFeeType, ShrineNPCFeeType shrineNPCFeeTypeFeeType = ShrineNPCFeeType.RuneShards)
    {
        ShrineNPC shrineNPC = GetShrineNPC(shrineNPCFeeType);

        if (shrineNPC.GetShrineNPCFee.IsFree)
        {
            shrineNPC.OnPurchased();
            return;
        }

        switch (shrineNPCFeeTypeFeeType)
        {
            case ShrineNPCFeeType.RuneShards:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.RuneShard) >= shrineNPC.GetShrineNPCFee.CurrentRuneShards)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.RuneShard, shrineNPC.GetShrineNPCFee.CurrentRuneShards);
                    shrineNPC.OnPurchased();
                }

                break;
            case ShrineNPCFeeType.Gems:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= shrineNPC.GetShrineNPCFee.CurrentGems)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.Gem, shrineNPC.GetShrineNPCFee.CurrentGems);
                    shrineNPC.OnPurchased();
                }

                break;
            case ShrineNPCFeeType.AdsCount:

                if (shrineNPC.GetShrineNPCFee.CurrentMaxAdsCount > 0)
                {
                    shrineNPC.GetShrineNPCFee.AdsWatched();
                    shrineNPC.OnPurchased();
                }

                break;
            default: break;
        }
    }

    public void OnOpened()
    {
        foreach (ShrineNPC shrineNPC in shrineNPCs)
        {
            shrineNPC.ResetInteractivity(); 
            shrineNPC.GetShrineNPCFee.VisitIncrement();
        }
    }

    private ShrineNPC GetShrineNPC(ShrineNPCType shrineNPCFeeType)
    {
        foreach (ShrineNPC shrineNPC in shrineNPCs)
        {
            if (shrineNPC.GetShrineNPCType() == shrineNPCFeeType)
            {
                return shrineNPC;
            }
        }
        return null;
    }
}

public enum ShrineNPCType
{
    AngelsGambit,
    ActiveAbilityReRoller,
    PassiveAbilityReRoller,
    HealingMagicRune
}

public enum ShrineNPCFeeType
{
    RuneShards,
    Gems,
    AdsCount
}

[Serializable]
public class ShrineNPCFee
{
    [Header("Shrine NPC Fee")]
    [SerializeField] private bool unlocked;
    [SerializeField] private int startingRuneShards;
    [SerializeField] private int startingGems;
    [SerializeField] private int adsCount;

    [Header("Increment")]
    public int visitPerIncrement;
    public float pricePecentageIncPerVisit;
    public float pricePecentageIncPerLevel;

    private int currentRuneShards;
    private int currentGems;
    private int currentMaxAdsCount;

    public bool Unlocked => unlocked;
    public int CurrentRuneShards => currentRuneShards;
    public int CurrentGems => currentGems;
    public int CurrentMaxAdsCount => currentMaxAdsCount;

    public bool IsFree => currentRuneShards == 0 && currentGems == 0 && currentMaxAdsCount == 0;    

    public ShrineNPCFee()
    {
        currentRuneShards = startingRuneShards;
        currentGems = startingGems;
        currentMaxAdsCount = adsCount;
    }

    public void VisitIncrement()
    {
        int runShardIncrement = (int)StatCalc.GetPercentage(startingRuneShards, pricePecentageIncPerVisit);
        currentRuneShards += runShardIncrement;

        int gemIncrement = (int)StatCalc.GetPercentage(startingGems, pricePecentageIncPerVisit);
        currentGems += gemIncrement;
    }

    public void LevelIncrement(int level)
    {
        int runShardIncrement = (int)StatCalc.GetPercentage(startingRuneShards, pricePecentageIncPerLevel * level);
        currentRuneShards += runShardIncrement;

        int gemIncrement = (int)StatCalc.GetPercentage(startingGems, pricePecentageIncPerLevel * level);
        currentGems += gemIncrement;
    }

    public void  Unlock()
    {
        unlocked = true;
    }

    public void AdsWatched()
    {
        currentMaxAdsCount--;
    }
}
