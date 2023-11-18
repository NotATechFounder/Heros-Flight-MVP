using UnityEngine;
using System;


[Serializable]
public class CharacterStatData
{
    [SerializeField] float attackSpeed;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] float defense;
    [SerializeField] float criticalHitChance;
    [SerializeField] RangeValue criticalHitDamage;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float Defense => defense;
    public float CriticalHitChance => criticalHitChance;
    public RangeValue CriticalHitDamage => criticalHitDamage;
}

[Serializable]
public class MonsterStatData : CharacterStatData
{
   
}

[Serializable]
public class PlayerStatData: CharacterStatData
{
    [SerializeField] HeroType heroType;
    [Header("Player Combat")]
    [SerializeField] protected int health;
    [SerializeField] float vitality;
    [SerializeField] float agility;
    [SerializeField] float resilience;
    [SerializeField] float dodgeChance;
    [SerializeField] RangeValue magicDamage;
    [SerializeField] RangeValue physicalDamage;
   

    public HeroType HeroType => heroType;
    public float Vitality => vitality;
    public float Agility => agility;
    public float Resilience => resilience;
    public float DodgeChance => dodgeChance;
    public int Health => health;
    public RangeValue MagicDamage => magicDamage;
    public RangeValue PhysicalDamage => physicalDamage;
   
}

public enum HeroType
{
    Melee,
    Archer,
    Mage
}