using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatController : MonoBehaviour
{
    [SerializeField] PlayerStatData playerCombatModel;


    public bool debug = false;

    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float CurrentMoveSpeed { get; private set; }
    [field: SerializeField] public float CurrentVitality { get; private set; }
    [field: SerializeField] public float CurrentAgility { get; private set; }
    [field: SerializeField] public float CurrentResilience { get; private set; }
    public float CurrentMagicDamage => playerCombatModel.MagicDamage.GetRandomValue() + runtimeMagicDamage;
    public float CurrentPhysicalDamage  => playerCombatModel.PhysicalDamage.GetRandomValue() + runtimePhysicalDamage;
    [field: SerializeField] public float CurrentCriticalHitChance { get; private set; }
    public float CurrentCriticalHitDamage => playerCombatModel.CriticalHitDamage.GetRandomValue() + runtimeCriticalHitDamage;
    [field: SerializeField] public float CurrentDefense { get; private set; }
    [field: SerializeField] public float CurrentAttackSpeed { get; private set; }

    private float runtimeMagicDamage;
    private float runtimePhysicalDamage;
    private float runtimeCriticalHitDamage;

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
        CurrentHealth = playerCombatModel.Health;
        CurrentMoveSpeed = playerCombatModel.MoveSpeed;
        CurrentVitality = playerCombatModel.Vitality;
        CurrentAgility = playerCombatModel.Agility;
        CurrentResilience = playerCombatModel.Resilience;
        CurrentCriticalHitChance = playerCombatModel.CriticalHitChance;
        CurrentDefense = playerCombatModel.Defense;
        CurrentAttackSpeed = playerCombatModel.AttackSpeed;
    }

    public void ModifyHealth(float percentageAmount, bool increase)
    {
        CurrentHealth = ModifyValue(playerCombatModel.Health, CurrentHealth, percentageAmount, increase);
    }

    public void ModifyMoveSpeed(float percentageAmount, bool increase)
    {
        CurrentMoveSpeed = ModifyValue(playerCombatModel.MoveSpeed, CurrentMoveSpeed, percentageAmount, increase);
    }

    public void ModifyVitality(float percentageAmount, bool increase)
    {
        CurrentMoveSpeed = ModifyValue(playerCombatModel.Vitality, CurrentVitality, percentageAmount, increase);
    }

    public void ModifyAgility(float percentageAmount, bool increase)
    {
        CurrentAgility = ModifyValue(playerCombatModel.Agility, CurrentAgility, percentageAmount, increase);
    }

    public void ModifyResilience(float percentageAmount, bool increase)
    {
        CurrentResilience = ModifyValue(playerCombatModel.Resilience, CurrentResilience, percentageAmount, increase);
    }

    public void ModifyMagicDamage(float percentageAmount, bool increase)
    {
        runtimeMagicDamage = ModifyValue(playerCombatModel.MagicDamage.max, runtimeMagicDamage, percentageAmount, increase);
    }

    public void ModifyPhysicalDamage(float percentageAmount, bool increase)
    {
        runtimePhysicalDamage = ModifyValue(playerCombatModel.PhysicalDamage.max, runtimePhysicalDamage, percentageAmount, increase);
    }

    public void ModifyCriticalHitChance(float percentageAmount, bool increase)
    {
        CurrentCriticalHitChance = ModifyValue(playerCombatModel.CriticalHitChance, CurrentCriticalHitChance, percentageAmount, increase);
    }

    public void ModifyCriticalHitDamage(float percentageAmount, bool increase)
    {
        runtimeCriticalHitDamage = ModifyValue(playerCombatModel.CriticalHitDamage.max, runtimeCriticalHitDamage, percentageAmount, increase);
    }

    public void ModifyDefense(float percentageAmount, bool increase)
    {
        CurrentDefense = ModifyValue(playerCombatModel.Defense, CurrentDefense, percentageAmount, increase);
    }

    public void ModifyAttackSpeed(float percentageAmount, bool increase)
    {
        CurrentAttackSpeed = ModifyValue(playerCombatModel.AttackSpeed, CurrentAttackSpeed, percentageAmount, increase);
    }

    private float ModifyValue(float baseValue, float currentValue, float percentageAmount, bool increase)
    {
        float percentageValue = ((float)percentageAmount / 100) * baseValue;
        return increase ? currentValue + percentageValue : currentValue - percentageValue;
    }
}
