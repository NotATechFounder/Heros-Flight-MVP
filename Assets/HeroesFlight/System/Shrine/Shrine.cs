using System;
using System.Collections.Generic;
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
    public Dictionary<ShrineNPCType, ShrineNPCFee> ShrineNPCFeeCache  = new();

    private void Awake()
    {
        foreach (ShrineNPCFee shrineNPCFee in shrineNPCFees)
        {
            shrineNPCFee.Init();
            ShrineNPCFeeCache.Add(shrineNPCFee.GetShrineNPCType, shrineNPCFee);
        }
    }

    public void Initialize(CurrencyManager currencyManager, CharacterStatController characterStatController)
    {
        this.currencyManager = currencyManager;
        this.characterStatController = characterStatController;

        angelEffectManager.Initialize(characterStatController);
        healer.Initialize(characterStatController);
    }

    public void UnlockNpc(ShrineNPCType shrineNPCFeeType)
    {
        ShrineNPCFee shrineNPCFee = GetShrineNPCFee(shrineNPCFeeType);
        shrineNPCFee.Unlock();
    }

    public bool Purchase(ShrineNPCType shrineNPCFeeType, ShrineNPCCurrencyType shrineNPCFeeTypeFeeType = ShrineNPCCurrencyType.RuneShard)
    {
        ShrineNPCFee shrineNPCFee = GetShrineNPCFee(shrineNPCFeeType);

        if (shrineNPCFee.IsFree)
        {
            shrineNPCFee.PurchaseSuccessful();
            return true;
        }

        switch (shrineNPCFeeTypeFeeType)
        {
            case ShrineNPCCurrencyType.RuneShard:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.RuneShard) >= shrineNPCFee.CurrentRuneShards)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.RuneShard, shrineNPCFee.CurrentRuneShards);
                    shrineNPCFee.PurchaseSuccessful();
                    return true;
                }

                break;
            case ShrineNPCCurrencyType.Gem:

                if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= shrineNPCFee.CurrentGems)
                {
                    currencyManager.ReduceCurency(CurrencyKeys.Gem, shrineNPCFee.CurrentGems);
                    shrineNPCFee.PurchaseSuccessful();
                    return true;
                } 
                break;
            case ShrineNPCCurrencyType.Ad:

                if (shrineNPCFee.CurrentMaxAdsCount > 0)
                {
                    shrineNPCFee.AdsWatched();
                    shrineNPCFee.PurchaseSuccessful();
                    return true;
                }

                break;
            default: break;
        }

        shrineNPCFee.PurchaseFailed();
        return false;
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
    HealingMagicRune,
    Blacksmith
}

public enum ShrineNPCCurrencyType
{
    RuneShard,
    Gem,
    Ad
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
    private int currentAdsCount;

    public event Action OnPurchaseSuccessful;
    public event Action OnPurchaseFailed;
    public Action OnInteracted;

    public ShrineNPCType GetShrineNPCType => shrineNPCType;
    public bool Unlocked => unlocked;
    public int CurrentRuneShards => currentRuneShards;
    public int CurrentGems => currentGems;
    public int CurrentMaxAdsCount => currentAdsCount;

    public bool IsFree => currentRuneShards == 0 && currentGems == 0 && currentAdsCount == 0;    

    public void Init()
    {
        currentRuneShards = startingRuneShards;
        currentGems = startingGems;
        currentAdsCount = adsCount;
    }

    public int GetPrice(ShrineNPCCurrencyType shrineNPCCurrencyType)
    {
        return shrineNPCCurrencyType switch
        {
            ShrineNPCCurrencyType.RuneShard => currentRuneShards,
            ShrineNPCCurrencyType.Gem => currentGems,
            ShrineNPCCurrencyType.Ad => currentAdsCount,
            _ => 0,
        };
    }

    public void PurchaseSuccessful()
    {
        OnInteracted?.Invoke();
        OnPurchaseSuccessful?.Invoke(); 
    }

    public void PurchaseFailed()
    {
        OnPurchaseFailed?.Invoke();
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

    public void Unlock()
    {
        unlocked = true;
    }

    public void AdsWatched()
    {
        currentAdsCount--;
    }
}
