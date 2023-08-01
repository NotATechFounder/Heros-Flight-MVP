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

    [SerializeField] float magicDamage;
    [SerializeField] float physicalDamage;
    [SerializeField] float criticalHitChance = 10f;
    [SerializeField] float criticalHitDamage;
    [SerializeField] float defense;
    [SerializeField] float attackSpeed;

    public float Vitality => vitality;
    public float Agility => agility;
    public float Resilience => resilience;
    public float MagicDamage => magicDamage;
    public float PhysicalDamage => physicalDamage;
    public float CriticalHitChance => criticalHitChance;
    public float CriticalHitDamage => criticalHitDamage;
    public float Defense => defense;
    public float AttackSpeed => attackSpeed;

    //public float GetRandomMagicDamage()
    //{
    //    return magicDamage.GetRandomValue();
    //}

    //public float GetRandomPhysicalDamage()
    //{
    //    return physicalDamage.GetRandomValue();
    //}
}

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