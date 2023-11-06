using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeFluffy : PassiveActiveAbility
{
    [SerializeField] private int numberOfProjectiles;
    [SerializeField] private float radious;
    [SerializeField] private ProjectileControllerBase projectileController;

    public override void OnActivated()
    {
        SpawnProjectiles(numberOfProjectiles, radious);
    }

    public override void OnCoolDownStarted()
    {

    }

    public override void OnCoolDownEnded()
    {

    }

    public void Initialize(int level)
    {
        this.level = level;
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
        Gizmos.DrawWireSphere(transform.position, radious);
        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfProjectiles;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radious;
            Gizmos.DrawWireSphere(transform.position + pos, 0.1f);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }
}
