using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.Gameplay.Enum;

using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardArrow : MonoBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float speed;
    [SerializeField] Transform view;
    [SerializeField] LayerMask detectLayer;
    Vector2 currentDirection = default;
    float healthPercentage;
    float currentLifetime;
    bool disabled;

    void Update()
    {
        if (disabled) return;

        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifeTime)
        {
            DisableProjectile();
        }

        transform.Translate(currentDirection.normalized * speed * Time.deltaTime);
    }

    void DisableProjectile()
    {
        if (disabled) return;
        disabled = true;
        ObjectPoolManager.ReleaseObject(gameObject);
    }

    public void SetupArrow(float healthPer, Vector2 targetDirection)
    {
        currentDirection = targetDirection;
        currentLifetime = 0;
        healthPercentage = healthPer;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, view.forward);
        view.rotation = targetRotation;
        disabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (disabled) return;

        if (IsInLayerMask( col.gameObject.layer))
        {
            if (col.TryGetComponent(out IHealthController healthController))
            {
                float damage = StatCalc.GetValueOfPercentage(healthPercentage, healthController.CurrentHealth);
                healthController.DealDamage(new DamageModel(damage, DamageType.NoneCritical, AttackType.Regular));
            }

            DisableProjectile();
        }
    }

    public bool IsInLayerMask(int layer)
    {
        return detectLayer == (detectLayer | (1 << layer));
    }
}
