using HeroesFlight.Common.Enum;
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

public class KnifeFluffy : RegularActiveAbility
{
    [Header("NumOfMobs")]
    [SerializeField] private int baseNumberOfMobsToDamage;
    [SerializeField] private int numberOfMobsToDamagePerLevel;

    [Header("Radious")]
    [SerializeField] private float baseRadious;
    [SerializeField] private CustomAnimationCurve damageRadiousCurve;
    [SerializeField] private int damagePercentage = 50;
    [SerializeField] private OverlapChecker overlapChecker;

    private int numberOfMobsToDamage;
    private float currentRadious;
    int currentDamage;
    int baseDamage;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().Play();
        numberOfMobsToDamage = GetMajorValueByLevel(baseNumberOfMobsToDamage, numberOfMobsToDamagePerLevel);
        currentRadious = damageRadiousCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, damagePercentage);
        overlapChecker.DetectOverlap(); 
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {

    }

    public void Initialize(int level, int baseDamage)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        overlapChecker.OnDetect += HandleOverlap;
    }

    private void HandleOverlap(int count, Collider2D[] collider2D)
    {
        for (int z = 0; z < count; z++)
        {
            if (collider2D[z].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                    DamageCritType.Critical, AttackType.Regular, CalculationType.Flat,null));
            }
        }
    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        //AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnHit -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        ObjectPoolManager.ReleaseObject(arrow.gameObject);
    }

    private void OnValidate()
    {
        damageRadiousCurve.UpdateCurve();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, currentRadious);
        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfMobsToDamage; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfMobsToDamage;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * currentRadious;
            Gizmos.DrawWireSphere(transform.position + pos, 0.1f);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    //private void SpawnProjectiles(int numberOfProjectiles, float radious)
    //{
    //    for (int i = 0; i < numberOfProjectiles; i++)
    //    {
    //        float angle = i * Mathf.PI * 2 / numberOfProjectiles;
    //        Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radious;
    //        ProjectileControllerBase bullet = ObjectPoolManager.SpawnObject(projectileController, transform.position + pos, Quaternion.identity);
    //        bullet.SetupProjectile(1, pos.normalized);
    //        bullet.OnEnded += HandleArrowDisable;
    //    }
    //}
}
