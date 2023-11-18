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

public class ChainRotate : RegularActiveAbility
{
    [SerializeField] private float roatationSpeed = 1f;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private OverlapChecker overlapChecker;

    [Header("Animation and Viusal Settings")]
    [SerializeField] private Transform visual;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string attackAnimation1Name = "animation";

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private CharacterSimpleController characterControllerInterface;
    private bool canRotate = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnActivated();
        }

        if (canRotate)
        {
            visual.Rotate(0, 0, roatationSpeed * Time.deltaTime);
        }
    }

    public override void OnActivated()
    {
        GetEffectParticleByLevel().Play();

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation1Name, true);
        canRotate = true;
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {

        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
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
        skeletonAnimation.AnimationState.Event += AnimationState_Event;
        overlapChecker.OnDetect += OnOverlap;
    }

    private void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
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
