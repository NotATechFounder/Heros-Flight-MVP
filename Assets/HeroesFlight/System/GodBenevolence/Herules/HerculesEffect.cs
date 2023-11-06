using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;

public class HerculesEffect : MonoBehaviour
{
    [SerializeField] private CircleOverlap overlapChecker;
    [SerializeField] private float autoAttackSpeed = 2f;
    private float timer;
    private float damage;

    private void Awake()
    {
        overlapChecker.OnDetect += OnOverlap;
    }

    public void SetUp(float _damage,Action OnHit=null)
    {
        damage = _damage;
        StartCoroutine(AutoAttack());
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
                    Debug.Log("HerulesEffect AutoAttack");
                    overlapChecker.DetectOverlap();
                }
            }
            yield return null;
        }
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(damage, 
                    DamageType.Critical, AttackType.Regular,DamageCalculationType.Flat));
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
