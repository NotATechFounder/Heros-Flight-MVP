using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Damage,
    BaseDamagePercentage,
    MaxDamagePercentage,
    SpecialDamagePercentage,

    Health,
    BaseHealthPercentage,
    MaxHealthPercentage,

    AllDamageResistencePercentage,
    MeleeDamageResistencePercentage,
    ProjectileDamageResistencePercentage,

    CriticalDamagePercentage,
    CriticalChancePercentage,
    SpecialAttackDelayPercentage,

    DamageToBossesPercentage,
    DamageToMeleePercentage,
    DamageToRangedPercentage,
    DamageToBosses,
    DamageToMelee,
    DamageToRanged,

    PetDamage,
    PetDamagePercentage,
    PetAttackSpeedPercentage,
    PetCriticalChancePercentage,
    PlayerToPetDamagePercentage,

    CoinGainPercentage,
    HeartHealingEffectPercentage,
    KillToBaseHealthPercentage,
    DodgeToBaseHealthPercentage,
    LowHealthToDamagePercentage,

    SpecialDamage
}
