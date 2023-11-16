using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWhirlwind : RegularActiveAbility
{
    [Header("lineDamage")]
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;

    [SerializeField] private OverlapChecker overlapChecker;

    private int baseDamage;
    private int currentDamage;
    private int currentlinesOfDamage;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, damagePercentageCurve.GetCurrentValueFloat(currentLevel));
        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, 1);
        overlapChecker.DetectOverlap();
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {
        GetEffectParticleByLevel().SetActive(false);
    }

    public override void LevelUp()
    {
        base.LevelUp();
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

                healthController.TryDealLineDamage(currentlinesOfDamage, lineDamageDelay, new HealthModificationIntentModel(currentDamage,
                DamageCritType.NoneCritical, AttackType.Regular, DamageCalculationType.Flat,null));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePercentageCurve.curveType != CurveType.Custom)
        {
            damagePercentageCurve.UpdateCurve();
        }
    }
}
