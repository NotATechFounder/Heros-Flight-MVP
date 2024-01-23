using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Plugins.Audio_System;
using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Combat.Enum;
using HeroesFlightProject.System.Combat.Controllers;
using UnityEngine;
using Spine.Unity;
using Spine;

public class LightNova : RegularActiveAbility
{
    [Header("LightNova")] 
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private float healthPercentage = 20;
    [SerializeField] private OverlapChecker overlapChecker;
    [Header("Animation and Viusal Settings")]
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string attackAnimation1Name = "animation";

    private CharacterStatController characterStatController;
    private CharacterSimpleController characterSystem;
    private HealthController characterHealthController;
    private BaseCharacterAttackController characterAttackController;

    private void Awake()
    {
        overlapChecker.OnDetect = Explode;
    }

    public void Initialize(int level,  CharacterStatController characterStatController, CharacterSimpleController characterSystem,  HealthController characterHealthController, BaseCharacterAttackController characterAttackController)
    {
        this.currentLevel = level;
        this.characterStatController = characterStatController;
        this.characterSystem = characterSystem;
        this.characterHealthController = characterHealthController;
        this.characterAttackController = characterAttackController;

        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        skeletonAnimation.gameObject.SetActive(false);
    }

    public override void OnActivated()
    {
        if (characterHealthController.CurrentHealth <= 0) return;

        characterHealthController.SetInvulnerableState(true);
        characterSystem.SetActionState(false);
        characterAttackController.ToggleControllerState(false);

        skeletonAnimation.gameObject.SetActive(true);
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation1Name, false);
        GetEffectParticleByLevel().Play();
        overlapChecker.DetectOverlap();
        AudioManager.PlaySoundEffect("Explosion", SoundEffectCategory.Hero);

        float healthToHeal = StatCalc.GetPercentage(characterHealthController.MaxHealth, healthPercentage);
        characterHealthController.Heal(healthToHeal, true);
    }

    public override void OnDeactivated()
    {
        characterHealthController.SetInvulnerableState(false);
        characterSystem.SetActionState(true);
        characterAttackController.ToggleControllerState(true);
    }

    public override void OnCoolDownEnded()
    {

    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
                skeletonAnimation.gameObject.SetActive(false);
                break;
            default: break;
        }
    }

    private void Explode(int hits, Collider2D[] colliders)
    {
        for (int i = 0; i < hits; i++)
        {
            if (colliders[i].TryGetComponent<IHealthController>(out var health))
            {
                health.TryDealDamage(new HealthModificationIntentModel(
                    characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage) * damageMultiplier,
                    DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat,null));
            }
        }
    }
}