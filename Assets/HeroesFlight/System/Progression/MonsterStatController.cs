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
        return StatCalc.ModifyValueByPercentage(baseAttack, baseAttack, Mathf.Abs(AttackModifier), AttackModifier > 0);
    }

    public float CalculateDefence(float baseDefence)
    {
        return StatCalc.ModifyValueByPercentage(baseDefence, baseDefence, Mathf.Abs(DefenceModifier), DefenceModifier > 0);
    }

    public float CalculateAttackSpeed(float baseAttackSpeed)// Attack speed need to be smaller to be positive
    {
        return StatCalc.ModifyValueByPercentage(baseAttackSpeed, baseAttackSpeed, Mathf.Abs(AttackSpeedUpModifier), AttackSpeedUpModifier < 0);
    }
}

public class MonsterStatController : MonoBehaviour
{
    [SerializeField] private float attackModifier;
    [SerializeField] private float defenceModifier;
    [SerializeField] private float attackSpeedUpModifier;
    [SerializeField] private Sprite currentCardIcon;

    public MonsterStatModifier GetMonsterStatModifier => new MonsterStatModifier
    {
        AttackModifier = attackModifier,
        DefenceModifier = defenceModifier,
        AttackSpeedUpModifier = attackSpeedUpModifier
    };

    public Sprite CurrentCardIcon => currentCardIcon;

    public void SetCurrentCardIcon(Sprite sprite)
    {
        currentCardIcon = sprite;
    }

    public void ModifyAttackModifier(float modifier, bool increase)
    {
        attackModifier = StatCalc.ModifyValue(attackModifier, modifier, increase);
    }

    public void ModifyDefenseModifier(float modifier, bool increase)
    {
        defenceModifier = StatCalc.ModifyValue(defenceModifier, modifier, increase);
    }

    public void ModifyAttackSpeedModifier(float modifier, bool increase)
    {
        attackSpeedUpModifier = StatCalc.ModifyValue(attackSpeedUpModifier, modifier, increase);
    }

    public void ResetStats()
    {
        attackModifier = 0;
        defenceModifier = 0;
        attackSpeedUpModifier = 0;
    }
}
