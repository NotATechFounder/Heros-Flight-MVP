using System;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    [SerializeField] AngelEffectManager angelEffectManager;
    [SerializeField] Healer healer;

    [SerializeField] private ShrineNPCFee[] shrineNPCFees;

    private CurrencyManager currencyManager;
    private CharacterStatController characterStatController;

    public AngelEffectManager GetAngelEffectManager => angelEffectManager;
    public Healer GetHealer => healer;

    public void Initialize(CurrencyManager currencyManager, CharacterStatController characterStatController)
    {
        this.currencyManager = currencyManager;
        this.characterStatController = characterStatController;

        angelEffectManager.Initialize(characterStatController);
        healer.Initialize(characterStatController);

        AssignShrineEffects();
    }

    public void AssignShrineEffects()
    {
        //switch (shrineNPC.GetShrineNPCType())
        //{
        //    case ShrineNPCType.AngelsGambit:
        //        shrineNPC.OnPurchaseSuccessful = () => angelEffectManager.TriggerAngelsGambit();
        //        break;
        //    case ShrineNPCType.ActiveAbilityReRoller:

        //        break;
        //    case ShrineNPCType.PassiveAbilityReRoller:

        //        break;
        //    case ShrineNPCType.HealingMagicRune:
        //        shrineNPC.OnPurchaseSuccessful = () => healer.Heal();
        //        break;
        //    default: break;
        //}
    }

    public void UnlockNpc(ShrineNPCType shrineNPCFeeType)
    {
        ShrineNPCFee shrineNPCFee = GetShrineNPCFee(shrineNPCFeeType);
        shrineNPCFee.Unlock();
    }

    public void Purchase(ShrineNPCType shrineNPCFeeType, ShrineNPCCurrencyType shrineNPCFeeTypeFeeType = ShrineNPCCurrencyType.RuneShards)
    {
        ShrineNPCFee shrineNPCFee = GetShrineNPCFee(shrineNPCFeeType);

        if (shrineNPCFee.IsFree)
        {
            // proceed;
            return;
        }

        switch (shrineNPCFeeTypeFeeType)
        {
            case ShrineNPCCurrencyType.RuneShards:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.RuneShard) >= shrineNPCFee.CurrentRuneShards)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.RuneShard, shrineNPCFee.CurrentRuneShards);
                   // shrineNPC.OnPurchased();
                }

                break;
            case ShrineNPCCurrencyType.Gems:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= shrineNPCFee.CurrentGems)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.Gem, shrineNPCFee.CurrentGems);
                    //shrineNPC.OnPurchased();
                }

                break;
            case ShrineNPCCurrencyType.AdsCount:

                if (shrineNPCFee.CurrentMaxAdsCount > 0)
                {
                    shrineNPCFee.AdsWatched();
                    //shrineNPC.OnPurchased();
                }

                break;
            default: break;
        }
    }

    public void OnOpened()
    {
        foreach (ShrineNPCFee shrineNPCFee in shrineNPCFees)
        {
            shrineNPCFee.VisitIncrement();
        }
    }

    private ShrineNPCFee GetShrineNPCFee(ShrineNPCType shrineNPCFeeType)
    {
        foreach (ShrineNPCFee shrineNPCFee in shrineNPCFees)
        {
            if (shrineNPCFee.GetShrineNPCType == shrineNPCFeeType)
            {
                return shrineNPCFee;
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

public enum ShrineNPCCurrencyType
{
    RuneShards,
    Gems,
    AdsCount
}

[Serializable]
public class ShrineNPCFee
{
    [Header("Shrine NPC Fee")]
    [SerializeField] private ShrineNPCType shrineNPCType;
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

    public ShrineNPCType GetShrineNPCType => shrineNPCType;
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
