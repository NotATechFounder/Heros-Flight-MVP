using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenStab : RegularActiveAbility
{
    [Header("LineDamage")]
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private int linesOfDamagePerIncrease = 1;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;

    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] private Transform visual;

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private int currentlinesOfDamage;
    private CharacterSimpleController characterControllerInterface;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, linesOfDamagePerIncrease);
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

    public void Initialize(int level, int baseDamage, CharacterSimpleController characterControllerInterface)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        this.characterControllerInterface = characterControllerInterface;
        characterControllerInterface.OnFaceDirectionChange += Flip;
        overlapChecker.OnDetect = OnDetect;
    }

    private void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
        overlapChecker.SetDirection(visual.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
    }

    private void OnDetect(int count, Collider2D[] collider2D)
    {
        for (int z = 0; z < count; z++)
        {
            if (collider2D[z].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealLineDamage(currentlinesOfDamage, lineDamageDelay, new HealthModificationIntentModel(currentDamage,
                    DamageType.Critical, AttackType.Regular, DamageCalculationType.Flat));
            }
        }
    }

    private void OnDestroy()
    {
        characterControllerInterface.OnFaceDirectionChange -= Flip;
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePercentageCurve.curveType != CurveType.Custom)
        {
            damagePercentageCurve.UpdateCurve();
        }
    }
}
