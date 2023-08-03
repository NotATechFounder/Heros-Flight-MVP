using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AngelCard", menuName = "Angel Gambit/Angel Card")]
public class AngelCardSO : ScriptableObject
{
    [SerializeField] string cardName;
    [TextArea(3, 10)]
    [SerializeField] string[] cardDescriptions;
    [SerializeField] Sprite cardImage;
    [SerializeField] AngelCardType cardType;
    [SerializeField] StatEffect affterBonusEffect;
    [SerializeField] StatEffect[] effects;
    [Range(0, 1)]
    [SerializeField] float chance;

    public string CardName => cardName;
    public string[] CardDescriptions => cardDescriptions;
    public Sprite CardImage => cardImage;

    public AngelCardType CardType => cardType;
    public StatEffect AffterBonusEffect => affterBonusEffect;

    public StatEffect[] Effects => effects;

    public float Chance => chance;

    public string GetDescription(AngelCardTier tier)
    {
       return cardDescriptions[(int)tier];
    }  
}

[System.Serializable]
public class AngelCard
{
    public AngelCardTier tier;
    public AngelCardSO angelCardSO;

    public AngelCard(AngelCardSO angelCardSO)
    {
        this.angelCardSO = angelCardSO;
        tier = AngelCardTier.One;
    }

    public float GetValueDifference()
    {
        return angelCardSO.AffterBonusEffect.GetValueDifference(tier);
    }
}

[System.Serializable]
public class StatEffect
{
    public BuffDebuff effect;
    public TargetType targetType;
    public float[] tierValues;

    [HideInInspector] public bool showStatEffect;
    [HideInInspector] public bool showTierValues;

    public float GetValue(AngelCardTier tier)
    {
        return tierValues[(int)tier];
    }

    public float GetValueDifference(AngelCardTier tier)
    {
        if (tier == AngelCardTier.One)
        {
            return tierValues[(int)tier];
        }
        return tierValues[(int)tier] - tierValues[(int)tier - 1];
    }
}

[System.Serializable]
public class TierEffect
{
    public AngelCardTier tier;
    public float value;
}

public enum AngelCardType
{
    Buff,
    Debuff,
}

public enum AngelCardTier
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
}