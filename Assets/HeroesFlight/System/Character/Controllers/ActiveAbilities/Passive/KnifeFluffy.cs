using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeFluffy : PassiveActiveAbility
{
    [SerializeField] private int baseNumberOfProjectiles;
    [SerializeField] private float baseRadious;
    [SerializeField] private int numberOfProjectilesPerLevel;
    [SerializeField] private float radiousPerLevel;
    [SerializeField] private ProjectileControllerBase projectileController;

    private int currentNumberOfProjectiles;
    private float currentRadious;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);
        currentNumberOfProjectiles = GetMajorValueByLevel(baseNumberOfProjectiles, numberOfProjectilesPerLevel);
        currentRadious = GetMajorValueByLevel(baseRadious, radiousPerLevel);
        SpawnProjectiles(baseNumberOfProjectiles, baseRadious);
    }

    public override void OnCoolDownStarted()
    {
        GetEffectParticleByLevel().SetActive (false);
    }

    public override void OnCoolDownEnded()
    {

    }

    public void Initialize(int level)
    {
        this.currentLevel = level;
    }

    private void SpawnProjectiles(int numberOfProjectiles, float radious)
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfProjectiles;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radious;
            ProjectileControllerBase bullet = ObjectPoolManager.SpawnObject(projectileController, transform.position + pos, Quaternion.identity);
            bullet.SetupProjectile(1, pos.normalized);
            bullet.OnEnded += HandleArrowDisable;
        }
    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnEnded -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        ObjectPoolManager.ReleaseObject(arrow.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, currentRadious);
        Gizmos.color = Color.red;
        for (int i = 0; i < currentNumberOfProjectiles; i++)
        {
            float angle = i * Mathf.PI * 2 / currentNumberOfProjectiles;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * currentRadious;
            Gizmos.DrawWireSphere(transform.position + pos, 0.1f);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override void LevelUpIncreaseEffect()
    {

    }
}
