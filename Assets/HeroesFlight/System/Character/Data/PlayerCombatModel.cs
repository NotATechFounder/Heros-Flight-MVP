using HeroesFlight.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCombatModel", menuName = "Model/Combat/PlayerCombat", order = 0)]
public class PlayerCombatModel : CombatModel
{
    [Header("Player Combat Model")]
    [SerializeField] float vitality;
    [SerializeField] float agility;
    [SerializeField] float resilience;

    [SerializeField] ValueRange magicDamage;
    [SerializeField] ValueRange physicalDamage;
    [SerializeField] float criticalHitChance = 10f;
    [SerializeField] ValueRange criticalHitDamage;
    [SerializeField] float defense;
    [SerializeField] float attackSpeed;

    public float Vitality => vitality;
    public float Agility => agility;
    public float Resilience => resilience;
    public ValueRange MagicDamage => magicDamage;
    public ValueRange PhysicalDamage => physicalDamage;
    public float CriticalHitChance => criticalHitChance;
    public ValueRange CriticalHitDamage => criticalHitDamage;
    public float Defense => defense;
    public float AttackSpeed => attackSpeed;
}

[System.Serializable]
public struct ValueRange
{
    public float min;
    public float max;

    public ValueRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandomValue()
    {
        return Random.Range(min, max);
    }
}