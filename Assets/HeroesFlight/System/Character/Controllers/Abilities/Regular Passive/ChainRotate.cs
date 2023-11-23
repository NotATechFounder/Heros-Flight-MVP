using HeroesFlight.System.Character;
using HeroesFlightProject.System.Gameplay.Controllers;
using Plugins.Audio_System;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using StansAssets.Foundation;

public class ChainRotate : RegularActiveAbility
{
    [SerializeField] private float roatationSpeed = 1f;

    [Header("Damage")]
    [SerializeField] private float damageRate = 1f;
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private OverlapChecker overlapChecker;

    [Header("Animation and Viusal Settings")]
    [SerializeField] private Transform visual;
    [SerializeField] SkeletonAnimation skeletonAnimation;

    public const string ChainOutAnimationName = "Chain_Out";
    public const string ChainLoopAnimationName = "Loop";
    public const string ChainInAnimationName = "Chain_In";

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private CharacterSimpleController characterControllerInterface;
    private bool canRotate = false;
    private float currentTime;

    private void Update()
    {
        if (canRotate)
        {
            visual.Rotate(0, 0, roatationSpeed * Time.deltaTime);

            if (currentTime <= 0)
            {
                currentTime = damageRate;
                overlapChecker.DetectOverlap();
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }
    }

    public override void OnActivated()
    {
        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);
        skeletonAnimation.gameObject.SetActive(true);
        skeletonAnimation.AnimationState.SetAnimation(0, ChainOutAnimationName, false);
    }

    public override void OnDeactivated()
    {
        canRotate = false;
        skeletonAnimation.AnimationState.SetAnimation(0, ChainInAnimationName, false);
        GetEffectParticleByLevel().gameObject.SetActive(false);
    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case ChainOutAnimationName:
                canRotate = true;
                skeletonAnimation.AnimationState.SetAnimation(0, ChainLoopAnimationName, true);
                GetEffectParticleByLevel().gameObject.SetActive(true);
                break;
            case ChainInAnimationName:
                skeletonAnimation.gameObject.SetActive(false);
                break;
            default: break;
        }
    }

    public void Initialize(int level, int baseDamage, CharacterSimpleController characterControllerInterface)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        this.characterControllerInterface = characterControllerInterface;
        characterControllerInterface.OnFaceDirectionChange += Flip;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        overlapChecker.OnDetect += OnOverlap;
    }

    private void Flip(bool facingLeft)
    {
       // visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
        //overlapChecker.SetDirection(!facingLeft ? OverlapChecker.Direction.Left : OverlapChecker.Direction.Right);
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                    DamageCritType.Critical, AttackType.Regular, CalculationType.Flat, null));
            }

            Debug.Log("Hit");
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
