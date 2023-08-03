using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatController : MonoBehaviour
{
    [SerializeField] private float attackModifier;
    [SerializeField] private float defenceModifier;
    [SerializeField] private float attackSpeedUpModifier;

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
