using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenStab : PassiveActiveAbility
{
    [SerializeField] private float damagePercentage;
    [SerializeField] private float damagePercentagePerIncrease;
    [SerializeField] private OverlapChecker overlapChecker;

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);
        currentDamagePercentage = GetValueByLevel(damagePercentage, damagePercentagePerIncrease);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);
    }

    public override void OnCoolDownStarted()
    {
        GetEffectParticleByLevel().SetActive(false);
        overlapChecker.DetectOverlap();
    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override void LevelUpIncreaseEffect()
    {

    }

    public void Initialize(int level, int baseDamage)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        overlapChecker.OnDetect = OnDetect;
    }

    private void OnDetect(int arg1, Collider2D[] collider2D)
    {
        for (int i = 0; i < arg1; i++)
        {
            if (collider2D[i].TryGetComponent(out IHealthController healthController))
            {

                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                DamageType.NoneCritical, AttackType.Regular, DamageCalculationType.Flat));
            }
        }
    }
}
