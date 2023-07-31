using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class StatEffectManager : MonoBehaviour
{
    public static StatEffectManager Instance { get; private set; }

    [SerializeField] private List<AngelCard> activeAngelCards = new List<AngelCard>();

    [Header("Base Stats")]
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseAttackSpeed;

    [Header("Current Stats")]
    [SerializeField] private float currentDamage;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentAttackSpeed;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentDamage = baseDamage;
        currentHealth = baseHealth;
        currentSpeed = baseSpeed;
        currentAttackSpeed = baseAttackSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AngelCard angelCard = activeAngelCards[0];
            foreach (StatEffect effect in angelCard.angelCardSO.Effects)
            {
                RemoveEffect(angelCard.tier, effect);
            }
            activeAngelCards.Remove(angelCard);
        }
    }

    public void AddAngelCardSO(AngelCardSO angelCardSO)
    {
        if (activeAngelCards.Count == 0 || !CardExists(angelCardSO))
        {
            AngelCard angelCard = new AngelCard(angelCardSO);
            activeAngelCards.Add(angelCard);

            foreach (StatEffect effect in angelCardSO.Effects)
            {
                AddEffect(angelCard.tier, effect);
            }
        }
    }

    private bool CardExists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in activeAngelCards)
        {
            if (angelCard.angelCardSO == angelCardSO)
            {
                angelCard.tier++;
                foreach (StatEffect effect in angelCardSO.Effects)
                {
                    AddEffect(angelCard.tier, effect);
                }
                return true;
            }
        }

        return false;
    }

    public AngelCard Exists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in activeAngelCards)
        {
            if (angelCard.angelCardSO == angelCardSO)
            {
                return angelCard;
            }
        }

        return null;
    }

    private void AddEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                HandlePlayerStatsActivation(angelCardTier, effect);
                break;
            case TargetType.Player:
                HandlePlayerStatsActivation(angelCardTier, effect);
                break;
            case TargetType.Monster:

                break;
        }
    }

    private void HandlePlayerStatsActivation(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                currentDamage += CalculatePercentage(baseDamage, effect.GetValueDifference(angelCardTier));
                break;
            case BuffDebuff.AttackDown:
                currentDamage -= CalculatePercentage(baseDamage, effect.GetValueDifference(angelCardTier));
                break;
            case BuffDebuff.DefenseUp:;
                currentHealth += CalculatePercentage(baseHealth, effect.GetValueDifference(angelCardTier));
                break;
            case BuffDebuff.DefenseDown:
                currentHealth -= CalculatePercentage(baseHealth, effect.GetValueDifference(angelCardTier));
                break;
            case BuffDebuff.AttackSpeedUp:
                currentAttackSpeed += CalculatePercentage(baseAttackSpeed, effect.GetValueDifference(angelCardTier));
                break;
            case BuffDebuff.AttackSpeedDown:
                currentAttackSpeed -= CalculatePercentage(baseAttackSpeed, effect.GetValueDifference(angelCardTier));
                break;
        }
    }

    private void RemoveEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                HandlePlayerStatsRemoval(angelCardTier, effect);
                break;
            case TargetType.Player:
                HandlePlayerStatsRemoval(angelCardTier, effect);
                break;
            case TargetType.Monster:

                break;
        }
    }

    private void HandlePlayerStatsRemoval(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                currentDamage -= CalculatePercentage(baseDamage, effect.GetValue(angelCardTier));
                break;
            case BuffDebuff.AttackDown:
                currentDamage += CalculatePercentage(baseDamage, effect.GetValue(angelCardTier));
                break;
            case BuffDebuff.DefenseUp:
                currentHealth -= CalculatePercentage(baseHealth, effect.GetValue(angelCardTier));
                break;
            case BuffDebuff.DefenseDown:
                currentHealth += CalculatePercentage(baseHealth, effect.GetValue(angelCardTier));
                break;
            case BuffDebuff.AttackSpeedUp:
                Debug.Log(effect.GetValue(angelCardTier));
                currentAttackSpeed -= CalculatePercentage(baseAttackSpeed, effect.GetValue(angelCardTier));
                break;
            case BuffDebuff.AttackSpeedDown:
                Debug.Log(effect.GetValue(angelCardTier));
                currentAttackSpeed += CalculatePercentage(baseAttackSpeed, effect.GetValue(angelCardTier));
                break;
        }
    }

    public float CalculateValueWithPercentage(float baseValue, float percentageAmount, bool increase)
    {
        float percentageValue = CalculatePercentage(baseValue, percentageAmount);
        return increase ? baseValue += percentageValue : baseValue -= percentageValue;
    }

    public float CalculatePercentage(float value, float percentageAmount)
    {
        float percentageValue = ((float)percentageAmount / 100) * value;
        return percentageValue;
    }
}
