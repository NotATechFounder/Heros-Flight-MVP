using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterStatModifier
{
    public float AttackModifier { get; set; }
    public float DefenceModifier { get; set; }
    public float AttackSpeedUpModifier { get; set;}

    public float CalculateAttack(float baseAttack)
    {
        return ModifyValue(baseAttack, baseAttack, Mathf.Abs(AttackModifier), AttackModifier > 0);
    }

    public float CalculateDefence(float baseDefence)
    {
        return ModifyValue(baseDefence, baseDefence, Mathf.Abs(DefenceModifier), DefenceModifier > 0);
    }

    public float CalculateAttackSpeed(float baseAttackSpeed)
    {
        return ModifyValue(baseAttackSpeed, baseAttackSpeed, Mathf.Abs(AttackSpeedUpModifier), AttackSpeedUpModifier > 0);
    }

    private float ModifyValue(float baseValue, float currentValue, float percentageAmount, bool increase)
    {
        float percentageValue = ((float)percentageAmount / 100) * baseValue;
        return increase ? currentValue + percentageValue : currentValue - percentageValue;
    }
}

public class MonsterStatController : MonoBehaviour
{
    [SerializeField] private float attackModifier;
    [SerializeField] private float defenceModifier;
    [SerializeField] private float attackSpeedUpModifier;

    public MonsterStatModifier GetMonsterStatModifier => new MonsterStatModifier
    {
        AttackModifier = attackModifier,
        DefenceModifier = defenceModifier,
        AttackSpeedUpModifier = attackSpeedUpModifier
    };

    public void ModifyAttackModifier(float modifier, bool increase)
    {
        Debug.Log($"Modifying attack modifier with {modifier} and increase {increase}");
        attackModifier = ModifyValue(attackModifier, modifier, increase);
    }

    public void ModifyDefenseModifier(float modifier, bool increase)
    {
        Debug.Log($"Modifying defence modifier with {modifier} and increase {increase}");
        defenceModifier = ModifyValue(defenceModifier, modifier, increase);
    }

    public void ModifyAttackSpeedModifier(float modifier, bool increase)
    {
        Debug.Log($"Modifying attack speed modifier with {modifier} and increase {increase}");
        attackSpeedUpModifier = ModifyValue(attackSpeedUpModifier, modifier, increase);
    }

    public void ResetStats()
    {
        attackModifier = 0;
        defenceModifier = 0;
        attackSpeedUpModifier = 0;
    }

    private float ModifyValue(float currentValue, float modifier, bool increase)
    {
        return increase ? currentValue + modifier : currentValue - modifier;
    }
}
