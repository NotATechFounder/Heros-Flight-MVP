using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApolloEffect : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    [SerializeField] private float autoAttackSpeed = 2f;
    [SerializeField] ProjectileControllerBase projectilePrefab;
    [SerializeField] List<WarningLine> warningLines;
    [SerializeField] private CircleOverlap overlapChecker;

    private Transform target;
    private float timer;
    private float damage;

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;
    }

    public void SetUp(float _damage,Action OnHit=null)
    {
        damage = _damage;
        StartCoroutine(AutoAttack());
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        if (count == 0)  return;
        target = colliders[0].transform;
        RotateToFaceTarget(arrow, target.position);
        FireProjectile();
    }

    public void RotateToFaceTarget( Transform originTransfrom,  Vector2 TargetPosition)
    {
        Vector2 direction = TargetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        originTransfrom.rotation = Quaternion.Euler(0, 0, angle);
    }

    public IEnumerator AutoAttack()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= autoAttackSpeed)
            {
                timer = 0;
                if (overlapChecker.TargetInRange())
                {
                    overlapChecker.DetectOverlap();
                }
            }
            yield return null;
        }
    }

    public void FireProjectile()
    {
        foreach (var line in warningLines)
        {
            line.Trigger(() =>
            {
                ProjectileControllerBase projectile = Instantiate(projectilePrefab, line.transform.position, Quaternion.identity);
                Vector2 direction = line.transform.position - target.position;
                projectile.SetupProjectile(damage, -direction);
                projectile.OnEnded += ResetProjectile;
            });
        }
    }

    private void ResetProjectile(ProjectileControllerInterface @interface)
    {
        var arrow = @interface as ProjectileControllerBase;
        Destroy(arrow.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
