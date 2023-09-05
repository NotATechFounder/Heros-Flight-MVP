using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHazard : EnironmentHazard
{
    public enum FartingMushroomType
    {
        Damage,
        Slow,
        Both,
    }

    [Header("Farting Mushroom Settings")]
    [SerializeField] private FartingMushroomType fartingMushroomType;
    [SerializeField] private Trigger2DObserver detectorObserver;
    [SerializeField] private Trigger2DObserver effectAreaObserver;
    [SerializeField] private GameObject effectArea;

    [Header("Damage Settings")]
    [SerializeField] private float healthPercentageDecrease;
    [SerializeField] private float poisonDuration;
    [SerializeField] private float poisonDamageInterval;

    [Header("Animation and Viusal Settings")]
    [SerializeField] public const string idleHidingAnimationName = "1_idle_hiding";
    [SerializeField] public const string idleToShowAnimationName = "2_idle_to_show";
    [SerializeField] public const string idleWaitingAnimationName = "3_idle_waiting";
    [SerializeField] public const string idleTohideAnimationName = "8_to_hide";
    [SpineSkin] [SerializeField] string pinkSkinReference;
    [SpineSkin] [SerializeField] string purpleSkinReference;
    [SerializeField] SkeletonAnimation skeletonAnimation;

    [Header("Damage Effect Settings")]
    [SerializeField] private Color damageEffectColor;

    [Header("Slow Effect Settings")]
    [SerializeField] private Color SlowEffectColor;

    [Header("Particle Effect Settings")]
    [SerializeField] private ParticleSystem mainfartingParticle;
    [SerializeField] private ParticleSystem subfartingParticle;

    [Header("Slow Settings")]
    [SerializeField] private float slowPercentageDecrease;

    [Header("To Hide")]
    private float poisonDamageTimer;
    private bool isEffectActive;


    private void Start()
    {
        detectorObserver.OnEnter += OnEnterDetectZone;
        detectorObserver.OnStay += InDetectZone;

        effectAreaObserver.OnEnter += OnEnterDamageZone;
        effectAreaObserver.OnStay += InDamageZone;
        effectAreaObserver.OnExit += OnExitDamageZone;

        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;

        SetCharacterSkin();

        SetAllParticleColor();

        ToggleMushroomEffect(false);
    }

    public void SetCharacterSkin()
    {
        switch (fartingMushroomType)
        {
            case FartingMushroomType.Damage:
                skeletonAnimation.Skeleton.SetSkin(pinkSkinReference);
                break;
            case FartingMushroomType.Slow:
                skeletonAnimation.Skeleton.SetSkin(purpleSkinReference);
                break;
            case FartingMushroomType.Both:
                skeletonAnimation.Skeleton.SetSkin(pinkSkinReference);
                break;
            default:  break;
        }
    }

    public void SetAllParticleColor()
    {
        switch (fartingMushroomType)
        {
            case FartingMushroomType.Damage:
                SetParticleColor(mainfartingParticle, damageEffectColor);
                SetParticleColor(subfartingParticle, damageEffectColor);
                break;
            case FartingMushroomType.Slow:
                SetParticleColor(mainfartingParticle, SlowEffectColor);
                SetParticleColor(subfartingParticle, SlowEffectColor);
                break;
            case FartingMushroomType.Both:
                SetParticleColor(mainfartingParticle, damageEffectColor);
                SetParticleColor(subfartingParticle, SlowEffectColor);
                break;
            default: break;
        }
    }

    public void SetParticleColor(ParticleSystem particle, Color startColor)
    {
        var main = particle.main;
        main.startColor = startColor;
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case idleToShowAnimationName:
                skeletonAnimation.AnimationState.SetAnimation(0, idleWaitingAnimationName, true);
                ToggleMushroomEffect(true);
                break;
                case idleTohideAnimationName:
                skeletonAnimation.AnimationState.SetAnimation(0, idleHidingAnimationName, true);
                ToggleMushroomEffect(false);
                break;
            default: break;
        }
    }

    private void Trigger(Collider2D collider2D)
    {
        if (collider2D.TryGetComponent(out IHealthController healthController))
        {
            float damage = StatCalc.GetValueOfPercentage(healthPercentageDecrease, healthController.CurrentHealth);
            healthController.DealDamage(new DamageModel(damage, DamageType.NoneCritical, AttackType.Regular));
        }
    }

    public void ToggleMushroomEffect(bool state)
    {
        isEffectActive = state;
        effectArea.SetActive(state);
        if (state)
        {
            mainfartingParticle.Play();
        }
        else
        {
            mainfartingParticle.Stop();
        }

        if (state)  StartCoroutine(Runtime());
    }

    private void OnEnterDetectZone(Collider2D d)
    {
        if (isInCooldown || isEffectActive) return;
        skeletonAnimation.AnimationState.SetAnimation(0, idleToShowAnimationName, false);
    }

    private void InDetectZone(Collider2D d)
    {
        if (!isInCooldown && !isEffectActive)
        {
            isEffectActive = true;
            skeletonAnimation.AnimationState.SetAnimation(0, idleToShowAnimationName, false);
        }
    }

    private void OnEnterDamageZone(Collider2D collider2D)
    {
        switch (fartingMushroomType)
        {
            case FartingMushroomType.Damage:
                poisonDamageTimer = poisonDamageInterval;
                break;
            case FartingMushroomType.Slow:

                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, false);
                }

                break;
            case FartingMushroomType.Both:
                poisonDamageTimer = poisonDamageInterval;
                if (collider2D.TryGetComponent(out characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, false);
                }
                break;
            default: break;
        }
    }

    private void InDamageZone(Collider2D collider2D)
    {
        switch (fartingMushroomType)
        {
            case FartingMushroomType.Damage:
                if (poisonDamageTimer >= poisonDamageInterval)
                {
                    Trigger(collider2D);
                    poisonDamageTimer = 0;
                }
                else
                {
                    poisonDamageTimer += Time.deltaTime;
                }
                break;
            case FartingMushroomType.Slow:

                break;
            case FartingMushroomType.Both:
                if (poisonDamageTimer >= poisonDamageInterval)
                {
                    Trigger(collider2D);
                    poisonDamageTimer = 0;
                }
                else
                {
                    poisonDamageTimer += Time.deltaTime;
                }
                break;
            default: break;
        }
    }

    private void OnExitDamageZone(Collider2D collider2D)
    {
        switch (fartingMushroomType)
        {
            case FartingMushroomType.Damage:

                break;
            case FartingMushroomType.Slow:
                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, true);
                }
                break;
            case FartingMushroomType.Both:
                if (collider2D.TryGetComponent(out characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, true);
                }
                break;
            default: break;
        }
    }

    public IEnumerator Runtime()
    {
        yield return new WaitForSeconds(poisonDuration);
        skeletonAnimation.AnimationState.SetAnimation(0, idleTohideAnimationName, false);
        yield return ActivateCooldown();
    }
}
