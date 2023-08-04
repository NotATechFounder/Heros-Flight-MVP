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

    [SerializeField] ValueRange magicDamage;
    [SerializeField] ValueRange physicalDamage;
    [SerializeField] float criticalHitChance = 10f;
    [SerializeField] ValueRange criticalHitDamage;

    public float Vitality => vitality;
    public float Agility => agility;
    public float Resilience => resilience;
    public ValueRange MagicDamage => magicDamage;
    public ValueRange PhysicalDamage => physicalDamage;
    public float CriticalHitChance => criticalHitChance;
    public ValueRange CriticalHitDamage => criticalHitDamage;

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
        return UnityEngine.Random.Range(min, max);
    }
}