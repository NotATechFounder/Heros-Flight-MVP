using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenHammer : RegularActiveAbility
{
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] private Transform visual;
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private int linesOfDamagePerIncrease = 1;

    private int currentlinesOfDamage;
    private int baseDamage;
    private int currentDamage;
    CharacterSimpleController characterControllerInterface;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnActivated();
        }
    }

    public override void OnActivated()
    {
        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, linesOfDamagePerIncrease);
        currentDamage =
            (int)StatCalc.GetPercentage(baseDamage, damagePercentageCurve.GetCurrentValueFloat(currentLevel));
        skeletonAnimation.gameObject.SetActive(true);
    }

    public override void OnDeactivated()
    {
    }

    public override void OnCoolDownEnded()
    {
    }

    public void Initialize(int level, int baseDamage, CharacterSimpleController characterControllerInterface)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        overlapChecker.OnDetect += OnOverlap;
        this.characterControllerInterface = characterControllerInterface;
        characterControllerInterface.OnFaceDirectionChange += Flip;
        skeletonAnimation.AnimationState.Event += AnimationState_Event;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            overlapChecker.DetectOverlap();
            GetEffectParticleByLevel().gameObject.SetActive(true);
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        skeletonAnimation.gameObject.SetActive(false);
    }

    private void Flip(bool facingLeft)
    {
        transform.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
        overlapChecker.SetDirection(facingLeft ? OverlapChecker.Direction.Left : OverlapChecker.Direction.Right);
    }

    private void OnOverlap(int count, Collider2D[] collider2D)
    {
        for (int i = 0; i < count; i++)
        {
            if (collider2D[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                    DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat, null, currentlinesOfDamage,
                    0.25f));
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