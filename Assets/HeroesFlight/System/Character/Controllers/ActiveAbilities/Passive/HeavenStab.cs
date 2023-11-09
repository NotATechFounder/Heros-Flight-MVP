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

public class HeavenStab : PassiveActiveAbility
{
    [Header("lineDamage")]
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;

    [Header("Damage")]
    [SerializeField] private float damagePercentage;
    [SerializeField] private float damagePercentagePerIncrease;
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

        currentDamagePercentage = GetValuePerLevel(damagePercentage, damagePercentagePerIncrease);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, 1);
        overlapChecker.DetectOverlap();
    }

    public override void OnCoolDownStarted()
    {
        GetEffectParticleByLevel().SetActive(false);
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

    private void OnDetect(int arg1, Collider2D[] collider2D)
    {
        StartCoroutine(LineDamage(arg1, collider2D));
    }

    public IEnumerator LineDamage(int count, Collider2D[] colliders)
    {
        yield return new WaitForSeconds(firstAttackDelay);
        for (int i = 0; i < currentlinesOfDamage; i++)
        {
            for (int z = 0; z < count; z++)
            {
                if (colliders[z].TryGetComponent(out IHealthController healthController))
                {
                    healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                        DamageType.Critical, AttackType.Regular, DamageCalculationType.Flat));
                }
            }
            yield return new WaitForSeconds(lineDamageDelay);
        }
    }

    private void OnDestroy()
    {
        characterControllerInterface.OnFaceDirectionChange -= Flip;
    }
}
