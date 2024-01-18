using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenStab : RegularActiveAbility
{
    [SerializeField] private float damageRate;

    [Header("LineDamage")]
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private int linesOfDamagePerIncrease = 1;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private ProjectileControllerBase projectileController;
    [SerializeField] private Transform visual;

    private ParticleSystem hitEffect;

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private int currentlinesOfDamage;
    private CharacterSimpleController characterControllerInterface;

    private OverlapChecker overlapChecker;
    private float currentTime;
    bool isOn = false;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().Play();
        GetEffectParticleByLevel().GetComponent<ParticleSystem>().Play();

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, linesOfDamagePerIncrease);

        FireProjectile();
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

    public void Initialize(int level, int baseDamage, CharacterSimpleController characterControllerInterface)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        this.characterControllerInterface = characterControllerInterface;
        characterControllerInterface.OnFaceDirectionChange += Flip;
    }

    private void Flip(bool facingLeft)
    {
        transform.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
    }

    private void FireProjectile()
    {
        ProjectileControllerBase bullet = ObjectPoolManager.SpawnObject(projectileController, transform.position, Quaternion.identity);
        bullet.transform.localScale = new Vector3(transform.localScale.x * bullet.transform.localScale.y, bullet.transform.localScale.y, 1);
        OverlapChecker overlapChecker = bullet.GetComponent<OverlapChecker>();
        hitEffect = bullet.GetComponentInChildren<ParticleSystem>();

        overlapChecker.OnDetect = OnDetect;
        this.overlapChecker = overlapChecker;

        bullet.SetupProjectile(currentDamage, -transform.localScale.x * Vector2.right);
        bullet.OnDeactivate += HandleArrowDisable;
    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        overlapChecker = null;
        //AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnDeactivate -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        ObjectPoolManager.ReleaseObject(arrow.gameObject);
    }

    private void OnDetect(int arg1, Collider2D[] collider2D)
    {
        for (int i = 0; i < arg1; i++)
        {
            if (collider2D[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat, null, currentlinesOfDamage));
                hitEffect.Play();
            }
        }
    }

    private void Update()
    {
        if (overlapChecker)
        {
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

    private void OnDestroy()
    {
        characterControllerInterface.OnFaceDirectionChange -= Flip;
    }

    private void OnValidate()
    {
        damagePercentageCurve.UpdateCurve();
    }
}
