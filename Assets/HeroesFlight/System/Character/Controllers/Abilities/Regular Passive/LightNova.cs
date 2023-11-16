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

public class LightNova : RegularActiveAbility
{
    [Header("LightNova")] 
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private ParticleSystem chargeEffect;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private OverlapChecker overlapChecker;

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
    }

    public override void OnActivated()
    {
        if (characterHealthController.CurrentHealth <= 0) return;

        characterHealthController.SetInvulnerableState(true);
        characterSystem.SetActionState(false);
        characterAttackController.ToggleControllerState(false);

        if (characterHealthController.CurrentHealth == characterHealthController.MaxHealth)
        {
            StartCoroutine(FullHealth(ActiveAbilitySO.Duration));
        }
        else
        {
            StartCoroutine(HealthRegen(ActiveAbilitySO.Duration));
        }
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

    private IEnumerator HealthRegen(float regenTime)
    {
        AudioManager.PlaySoundEffect("ValSkillActivation", SoundEffectCategory.Hero);

        chargeEffect.transform.localScale = Vector3.one;
        chargeEffect.Play();

        float elapsedTime = 0.0f;
        float startHealth = characterStatController.GetCurrentHealth();
        while (characterHealthController.CurrentHealth < characterHealthController.MaxHealth)
        {
            elapsedTime += Time.deltaTime;
            float amountToRegen =
                Mathf.Lerp(startHealth, characterHealthController.MaxHealth, elapsedTime / regenTime) -
                characterHealthController.CurrentHealth;
            characterHealthController.Heal(amountToRegen, false);
            chargeEffect.transform.localScale = Vector3.one + (Vector3.one * (elapsedTime / regenTime));
            yield return null;
        }

        chargeEffect.Stop();
        explosionEffect.Play();
        overlapChecker.DetectOverlap();
        AudioManager.PlaySoundEffect("Explosion", SoundEffectCategory.Hero);
    }

    private IEnumerator FullHealth(float regenTime)
    {
        AudioManager.PlaySoundEffect("ValSkillActivation", SoundEffectCategory.Hero);

        chargeEffect.Play();

        float elapsedTime = 0.0f;

        while (elapsedTime < regenTime)
        {
            elapsedTime += Time.deltaTime;
            chargeEffect.transform.localScale = Vector3.one + (Vector3.one * (elapsedTime / regenTime));
            yield return null;
        }

        chargeEffect.Stop();
        explosionEffect.Play();
        overlapChecker.DetectOverlap();
        AudioManager.PlaySoundEffect("Explosion", SoundEffectCategory.Hero);
    }

    private void Explode(int hits, Collider2D[] colliders)
    {
        for (int i = 0; i < hits; i++)
        {
            if (colliders[i].TryGetComponent<IHealthController>(out var health))
            {
                health.TryDealDamage(new HealthModificationIntentModel(
                    characterStatController.CurrentMagicDamage * damageMultiplier,
                    DamageCritType.NoneCritical, AttackType.Regular, DamageCalculationType.Flat,null));
            }
        }
    }
}