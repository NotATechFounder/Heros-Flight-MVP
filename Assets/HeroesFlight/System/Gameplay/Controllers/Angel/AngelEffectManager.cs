using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelEffectManager : MonoBehaviour
{
    public Action<AngelCard> OnPermanetCard;

    [SerializeField] private List<AngelCard> collectedAngelCards = new List<AngelCard>();
    [SerializeField] private List<AngelCard> permanetStatEffect = new List<AngelCard>();
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private AngelCard currentAngelCard;

    public bool EffectActive => currentAngelCard != null && currentAngelCard.angelCardSO != null;

    public void ComplectedLevel()
    {
        if (currentAngelCard != null && currentAngelCard.angelCardSO != null)
        {
            foreach (StatEffect effect in currentAngelCard.angelCardSO.Effects)
            {
                RemoveEffect(currentAngelCard.tier, effect);
            }
            AddAfterBonusEffect(currentAngelCard.tier, currentAngelCard);
        }
    }

    private void AddAfterBonusEffect(AngelCardTier angelCardTier, AngelCard newAngelCard)
    {
        foreach (AngelCard angelCard in permanetStatEffect)
        {
            if (angelCard.angelCardSO == newAngelCard.angelCardSO)
            {
                ModifyPlayerStatRaw(angelCard.tier, angelCard.angelCardSO.AffterBonusEffect, false);
                permanetStatEffect.Remove(angelCard);
                break;
            }
        }

        permanetStatEffect.Add(newAngelCard);
        ModifyPlayerStatRaw(angelCardTier, newAngelCard.angelCardSO.AffterBonusEffect, true);
        OnPermanetCard?.Invoke(newAngelCard);
        currentAngelCard = null;
    }

    public void AddAngelCardSO(AngelCardSO angelCardSO)
    {
        if (collectedAngelCards.Count == 0 || !CardExists(angelCardSO))
        {
            AngelCard angelCard = new AngelCard(angelCardSO);
            collectedAngelCards.Add(angelCard);

            foreach (StatEffect effect in angelCardSO.Effects)
            {
                AddEffect(angelCard.tier, effect);
            }

            currentAngelCard = angelCard;
        }
    }

    private bool CardExists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in collectedAngelCards)
        {
            if (angelCard.angelCardSO == angelCardSO)
            {
                angelCard.tier++;
                foreach (StatEffect effect in angelCardSO.Effects)
                {
                    AddEffect(angelCard.tier, effect);
                }
                currentAngelCard = angelCard;
                return true;
            }
        }

        return false;
    }

    public AngelCard Exists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in collectedAngelCards)
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
                ModifyPlayerStatDifference(angelCardTier, effect, true);
                ModifyMonsterStatDifference(angelCardTier, effect, true);
                break;
            case TargetType.Player:
                ModifyPlayerStatDifference(angelCardTier, effect,true);
                break;
            case TargetType.Monster:
                ModifyPlayerStatDifference(angelCardTier, effect, true);
                break;
        }
    }

    private void RemoveEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                ModifyPlayerStatRaw(angelCardTier, effect,false);
                ModifyMonsterStatRaw(angelCardTier, effect,false);
                break;
            case TargetType.Player:
                ModifyPlayerStatRaw(angelCardTier, effect, false);
                break;
            case TargetType.Monster:
                ModifyPlayerStatRaw(angelCardTier, effect, false);
                break;
        }
    }

    private void ModifyPlayerStatDifference(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.ModifyPhysicalDamage(effect.GetValueDifference(angelCardTier), positive);
                characterStatController.ModifyMagicDamage(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.ModifyPhysicalDamage(effect.GetValueDifference(angelCardTier), !positive);
                characterStatController.ModifyMagicDamage(effect.GetValueDifference(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.ModifyDefense(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.ModifyDefense(effect.GetValueDifference(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.ModifyAttackSpeed(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.ModifyAttackSpeed(effect.GetValueDifference(angelCardTier), !positive);
                break;
        }
    }

    private void ModifyPlayerStatRaw(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.ModifyPhysicalDamage(effect.GetValue(angelCardTier), positive);
                characterStatController.ModifyMagicDamage(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.ModifyPhysicalDamage(effect.GetValue(angelCardTier), !positive);
                characterStatController.ModifyMagicDamage(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.ModifyDefense(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.ModifyDefense(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.ModifyAttackSpeed(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.ModifyAttackSpeed(effect.GetValue(angelCardTier), !positive);
                break;
        }
    }

    private void ModifyMonsterStatDifference(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:

                break;
            case BuffDebuff.AttackDown:

                break;
            case BuffDebuff.DefenseUp:

                break;
            case BuffDebuff.DefenseDown:

                break;
            case BuffDebuff.AttackSpeedUp:

                break;
            case BuffDebuff.AttackSpeedDown:

                break;
        }
    }

    private void ModifyMonsterStatRaw(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:

                break;
            case BuffDebuff.AttackDown:

                break;
            case BuffDebuff.DefenseUp:

                break;
            case BuffDebuff.DefenseDown:

                break;
            case BuffDebuff.AttackSpeedUp:

                break;
            case BuffDebuff.AttackSpeedDown:

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

    public AngelCard GetActiveAngelCard()
    {
        return currentAngelCard;
    }
}
