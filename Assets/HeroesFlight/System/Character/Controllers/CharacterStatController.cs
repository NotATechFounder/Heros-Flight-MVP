using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer currentCardIcon;

    [SerializeField] PlayerStatData playerCombatModel;

    public PlayerStatData PlayerStatData => playerCombatModel;

    public Action<float, bool> OnHealthModified;
    public Action<float> OnMaxHealthChanged;

    public Func<float> GetCurrentHealth;

    public bool debug = false;
    public HeroType GetHeroType => playerCombatModel.HeroType;
    public float CurrentMaxHealth => runtimeMaxHealth;
    [field: SerializeField] public float CurrentMoveSpeed { get; private set; }
    [field: SerializeField] public float CurrentVitality { get; private set; }
    [field: SerializeField] public float CurrentAgility { get; private set; }
    [field: SerializeField] public float CurrentResilience { get; private set; }
    [field: SerializeField] public float CurrentDodgeChance { get; private set; }
    [field: SerializeField] public float CurrentAttackRange { get; private set; }
    [field: SerializeField] public float CurrentLifeSteal { get; private set; }
    public float CurrentMagicDamage => playerCombatModel.MagicDamage.GetRandomValue() + runtimeMagicDamage;
    public float CurrentPhysicalDamage  => playerCombatModel.PhysicalDamage.GetRandomValue() + runtimePhysicalDamage;
    [field: SerializeField] public float CurrentCriticalHitChance { get; private set; }
    public float CurrentCriticalHitDamage => playerCombatModel.CriticalHitDamage.GetRandomValue() + runtimeCriticalHitDamage;
    [field: SerializeField] public float CurrentDefense { get; private set; }
    [field: SerializeField] public float CurrentAttackSpeed { get; private set; }

    [SerializeField] private float runtimeMaxHealth;
    [SerializeField] private float runtimeMagicDamage;
    [SerializeField] private float runtimePhysicalDamage;
    [SerializeField] private float runtimeCriticalHitDamage;


    private void Start()
    {
        if (debug)
        {
            Initialize(playerCombatModel);
        }
    }

    public void Initialize(PlayerStatData playerCombatModel)
    {
        this.playerCombatModel = playerCombatModel;
        ResetStats();
    }

    public void SetCurrentCardIcon(Sprite sprite)
    {
        if (sprite != null)
        {
            currentCardIcon.sprite = sprite;
            currentCardIcon.enabled = true;
        }
        else
        {
            currentCardIcon.enabled = false;
        }
    }

    public void ModifyMaxHealth(float amount, bool increase)
    {
        runtimeMaxHealth += increase ? amount : -amount;
        OnMaxHealthChanged?.Invoke(runtimeMaxHealth);
    }

    public void ModifyHealth(float percentageAmount, bool increase)
    {
        float value = StatCalc.GetPercentage(playerCombatModel.Health, percentageAmount);
        OnHealthModified?.Invoke(value, increase);
    }

    public void ModifyMoveSpeed(float percentageAmount, bool increase)
    {
        CurrentMoveSpeed = StatCalc.ModifyValueByPercentage(playerCombatModel.MoveSpeed, CurrentMoveSpeed, percentageAmount, increase);
    }

    public void ModifyVitality(float percentageAmount, bool increase)
    {
        CurrentMoveSpeed = StatCalc.ModifyValueByPercentage(playerCombatModel.Vitality, CurrentVitality, percentageAmount, increase);
    }

    public void ModifyAgility(float percentageAmount, bool increase)
    {
        CurrentAgility = StatCalc.ModifyValueByPercentage(playerCombatModel.Agility, CurrentAgility, percentageAmount, increase);
    }

    public void ModifyResilience(float percentageAmount, bool increase)
    {
        CurrentResilience = StatCalc.ModifyValueByPercentage(playerCombatModel.Resilience, CurrentResilience, percentageAmount, increase);
    }

    public void ModifyDodgeChance(float amount, bool increase, bool isPercentage = true)
    {
        if (isPercentage)
        {
            CurrentDodgeChance = StatCalc.ModifyValueByPercentage(playerCombatModel.DodgeChance, CurrentDodgeChance, amount, increase);
        }
        else
        {
            CurrentDodgeChance += increase ? amount : -amount;
        }
    }

    public void ModifyMagicDamage(float percentageAmount, bool increase)
    {
        runtimeMagicDamage = StatCalc.ModifyValueByPercentage(playerCombatModel.MagicDamage.max, runtimeMagicDamage, percentageAmount, increase);
    }

    public void ModifyPhysicalDamage(float percentageAmount, bool increase)
    {
        runtimePhysicalDamage = StatCalc.ModifyValueByPercentage(playerCombatModel.PhysicalDamage.max, runtimePhysicalDamage, percentageAmount, increase);
    }

    public void ModifyCriticalHitChance(float amount,bool increase, bool isPercentage = true)
    {
        if (isPercentage)
        {
            CurrentCriticalHitChance = StatCalc.ModifyValueByPercentage(playerCombatModel.CriticalHitChance, CurrentCriticalHitChance, amount, increase);
        }
        else
        {
            CurrentCriticalHitChance += increase ? amount : -amount;
        }
    }

    public void ModifyCriticalHitDamage(float percentageAmount, bool increase)
    {
        runtimeCriticalHitDamage = StatCalc.ModifyValueByPercentage(playerCombatModel.CriticalHitDamage.max, runtimeCriticalHitDamage, percentageAmount, increase);
    }

    public void ModifyDefense(float amount, bool increase, bool isPercentage = true)
    {
        if (isPercentage)
        {
            CurrentDefense = StatCalc.ModifyValueByPercentage(playerCombatModel.Defense, CurrentDefense, amount, increase);
        }
        else
        {
            CurrentDefense += increase ? amount : -amount;
        }
    }

    public void ModifyAttackSpeed(float percentageAmount, bool increase) // Attack speed need to be smaller to be positive
    {
        CurrentAttackSpeed = StatCalc.ModifyValueByPercentage(playerCombatModel.AttackSpeed, CurrentAttackSpeed, percentageAmount, !increase);
    }

    public void ModifyAttackRange(float amount, bool increase, bool isPercentage = true)
    {
        if (isPercentage)
        {
            CurrentAttackRange = StatCalc.ModifyValueByPercentage(playerCombatModel.AttackRange, CurrentAttackRange, amount, increase);
        }
        else
        {
            CurrentAttackRange += increase ? amount : -amount;
        }
    }

    public void ModifyLifeSteal(float amount, bool increase)
    {
        CurrentLifeSteal += increase ? amount : -amount;
    }

    public float GetHealthPercentage()
    {
        if (GetCurrentHealth != null)
        {
            return (GetCurrentHealth() / playerCombatModel.Health) * 100;
        }
        return 0;
    }

    public void ResetStats()
    {
        runtimeMaxHealth = playerCombatModel.Health;
        CurrentMoveSpeed = playerCombatModel.MoveSpeed;
        CurrentVitality = playerCombatModel.Vitality;
        CurrentAgility = playerCombatModel.Agility;
        CurrentResilience = playerCombatModel.Resilience;
        CurrentDodgeChance = playerCombatModel.DodgeChance;
        CurrentCriticalHitChance = playerCombatModel.CriticalHitChance;
        CurrentDefense = playerCombatModel.Defense;
        CurrentAttackSpeed = playerCombatModel.AttackSpeed;
        CurrentAttackRange = playerCombatModel.AttackRange;
        CurrentLifeSteal = 0;
    }
}
