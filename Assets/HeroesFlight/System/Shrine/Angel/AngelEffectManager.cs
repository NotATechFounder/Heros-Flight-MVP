using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatModel;

public class AngelEffectManager : MonoBehaviour
{
    public Action<AngelCard> OnPermanetCard;

    private List<AngelCard> collectedAngelCards = new List<AngelCard>();
    private List<AngelCard> permanetStatEffect = new List<AngelCard>();
    private CharacterStatController characterStatController;
    private MonsterStatController  monsterStatController;
    private AngelCard currentAngelCard;

    public bool EffectActive => currentAngelCard != null && currentAngelCard.angelCardSO != null;

    private void Awake()
    {
        monsterStatController = FindAnyObjectByType<MonsterStatController>();
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
    }

    public bool CompletedLevel()
    {
        if (currentAngelCard != null && currentAngelCard.angelCardSO != null)
        {
            foreach (StatEffect effect in currentAngelCard.angelCardSO.Effects)
            {
                RemoveEffect(currentAngelCard.tier, effect);
            }
            AddAfterBonusEffect(currentAngelCard.tier, currentAngelCard);
            return true;
        }

        return false;
    }

    private void AddAfterBonusEffect(AngelCardTier angelCardTier, AngelCard newAngelCard)
    {
        foreach (AngelCard angelCard in permanetStatEffect)
        {
            if (angelCard.angelCardSO == newAngelCard.angelCardSO)
            {
                ModifyPlayerStatRaw(angelCard.tier, angelCard.angelCardSO.AffterBonusEffect, StatModel.StatModificationType.Addition);
                permanetStatEffect.Remove(angelCard);
                break;
            }
        }

        permanetStatEffect.Add(newAngelCard);
        ModifyPlayerStatRaw(angelCardTier, newAngelCard.angelCardSO.AffterBonusEffect, StatModel.StatModificationType.Addition);
        OnPermanetCard?.Invoke(newAngelCard);
        currentAngelCard = null;
        characterStatController.SetCurrentCardIcon(null);
        monsterStatController.SetCurrentCardIcon(null);
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
            characterStatController.SetCurrentCardIcon(angelCardSO.CardImage);
            monsterStatController.SetCurrentCardIcon(angelCardSO.CardImage);
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
                ModifyPlayerStatDifference(angelCardTier, effect, StatModel.StatModificationType.Addition);
                ModifyMonsterStatRaw(angelCardTier, effect, true);
                break;
            case TargetType.Player:
                ModifyPlayerStatDifference(angelCardTier, effect, StatModel.StatModificationType.Addition);
                break;
            case TargetType.Monster:
                ModifyMonsterStatRaw(angelCardTier, effect, true);
                break;
        }
    }

    private void RemoveEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                ModifyPlayerStatRaw(angelCardTier, effect, StatModel.StatModificationType.Subtraction);
                ModifyMonsterStatRaw(angelCardTier, effect,false);
                break;
            case TargetType.Player:
                ModifyPlayerStatRaw(angelCardTier, effect, StatModel.StatModificationType.Subtraction);
                break;
            case TargetType.Monster:
                ModifyMonsterStatRaw(angelCardTier, effect, false);
                break;
        }
    }

    private void ModifyPlayerStatDifference(AngelCardTier angelCardTier, StatEffect effect, StatModel.StatModificationType statModificationType)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.PhysicalDamage, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.PhysicalDamage, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, effect.GetValueDifference(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
        }
    }

    private void ModifyPlayerStatRaw(AngelCardTier angelCardTier, StatEffect effect, StatModel.StatModificationType statModificationType)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.GetStatModel. ModifyCurrentStat(StatType.PhysicalDamage, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.PhysicalDamage, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, effect.GetValue(angelCardTier), statModificationType, StatModel.StatCalculationType.Percentage);
                break;
        }
    }

    private void ModifyMonsterStatRaw(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                monsterStatController.ModifyAttackModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                monsterStatController.ModifyAttackModifier(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                monsterStatController.ModifyDefenseModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:    
                monsterStatController.ModifyDefenseModifier(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                monsterStatController.ModifyAttackSpeedModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                monsterStatController.ModifyAttackSpeedModifier(effect.GetValue(angelCardTier), !positive);
                break;
        }
    }

    public AngelCard GetActiveAngelCard()
    {
        return currentAngelCard;
    }

    public void ResetAngelEffects()
    {
        //foreach (AngelCard angelCard in collectedAngelCards)
        //{
        //    foreach (StatEffect effect in angelCard.angelCardSO.Effects)
        //    {
        //        RemoveEffect(angelCard.tier, effect);
        //    }
        //}
        collectedAngelCards = new List<AngelCard>();
        permanetStatEffect = new List<AngelCard>();
        currentAngelCard = null;
        Debug.Log("Reset Angel Effects");
    }
}
