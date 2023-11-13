using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using System;
using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float shootInterval;
    [SerializeField] private ProjectileControllerBase projectileController;
    [SerializeField] private OverlapChecker overlapChecker;

    private float shootTimer;

    private void Start()
    {
        overlapChecker.OnDetect = Explode;
    }

    private void Update()
    {
        if (shootTimer > 0)
            shootTimer -= Time.deltaTime;
        else
        {
            shootTimer = shootInterval;
            Shoot();
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void Shoot()
    {
        overlapChecker.DetectOverlap();
    }

    private void Explode(int arg1, Collider2D[] arg2)
    {
        for (int i = 0; i < arg1; i++)
        {
            ProjectileControllerBase bullet = ObjectPoolManager.SpawnObject(projectileController, transform.position, Quaternion.identity);
            bullet.SetupProjectile(damage, (arg2[i].transform.position - transform.position).normalized);
            bullet.OnHit += HandleArrowDisable;
        }
    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnHit -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        ObjectPoolManager.ReleaseObject(arrow.gameObject);
    }
}
