using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AngelCard", menuName = "Angel Gambit/Angel Card")]
public class AngelCardSO : ScriptableObject
{
    [SerializeField] string cardName;
    [TextArea(3, 10)]
    [SerializeField] string fullCardDescription;
    [TextArea(1, 3)]
    [SerializeField] string shortCardDescription;
    [SerializeField] Sprite cardImage;
    [SerializeField] AngelCardType cardType;
    [SerializeField] AngelCardTier cardTier;
    [SerializeField] CardEffect affterEffectBonus;
    [SerializeField] CardEffect[] effects;

    public string CardName => cardName;
    public string CardDescription => fullCardDescription;
    public string ShortCardDescription => shortCardDescription;
    public Sprite CardImage => cardImage;

    public AngelCardType CardType => cardType;
    public AngelCardTier CardTier => cardTier;
    public CardEffect AffterEffectBonus => affterEffectBonus;
}


[System.Serializable]
public class CardEffect
{
    public BuffDebuff effect;
    public TargetType targetType;
    public float value;
}