using UnityEngine;
using System;


[Serializable]
public class CharacterStatData
{
    [SerializeField] protected int health;
    [SerializeField] float attackSpeed;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] float defense;

    public int Health => health;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float Defense => defense;
}

[Serializable]
public class MonsterStatData : CharacterStatData
{
    [Header("Monster Combat")]
    [SerializeField] float agroDistance;
    [SerializeField] float damage = 2;

    public float Damage => damage;
    public float AgroDistance => agroDistance;
}

[Serializable]
public class PlayerStatData: CharacterStatData
{
    [Header("Player Combat")]
    [SerializeField] float vitality;
    [SerializeField] float agility;
    [SerializeField] float resilience;

    [SerializeField] RangeValue magicDamage;
    [SerializeField] RangeValue physicalDamage;
    [SerializeField] float criticalHitChance = 10f;
    [SerializeField] RangeValue criticalHitDamage;

    public float Vitality => vitality;
    public float Agility => agility;
    public float Resilience => resilience;
    public RangeValue MagicDamage => magicDamage;
    public RangeValue PhysicalDamage => physicalDamage;
    public float CriticalHitChance => criticalHitChance;
    public RangeValue CriticalHitDamage => criticalHitDamage;

}


[System.Serializable]
public struct RangeValue
{
    public float min;
    public float max;

    public RangeValue(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}