using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartingMushroomHazard : EnironmentHazard
{
    [Header("Farting Mushroom Settings")]
    [SerializeField] private GameObject poisonEffect;
    [SerializeField] private Trigger2DObserver detectorObserver;
    [SerializeField] private Trigger2DObserver damagerObserver;
    [SerializeField] private float healthPercentageDecrease;
    [SerializeField] private float poisonDuration;
    [SerializeField] private float poisonDamageInterval;

    [Header("To Hide")]
    [SerializeField] private float poisonDamageTimer;
    [SerializeField] private bool isPoisonActive;
    [SerializeField] private Collider2D target;


    private void Start()
    {
        detectorObserver.OnEnter += OnEnterDetectZone;
        detectorObserver.OnStay += InDetectZone;

        damagerObserver.OnEnter += OnEnterDamageZone;
        damagerObserver.OnStay += InDamageZone;
        damagerObserver.OnExit += OnExitDamageZone;

        TogglePoison(false);
    }

    public override void Trigger()
    {
        if (target.TryGetComponent(out IHealthController healthController))
        {
            float damage = StatCalc.GetValueOfPercentage(healthPercentageDecrease, healthController.CurrentHealth);
            healthController.DealDamage(new DamageModel(damage, DamageType.NoneCritical, AttackType.Regular));
        }
    }

    public void TogglePoison(bool state)
    {
        isPoisonActive = state;
        poisonEffect.SetActive(state);
    }

    private void OnEnterDetectZone(Collider2D d)
    {
        if (isInCooldown || isPoisonActive) return;
        TogglePoison(true);
        StartCoroutine(Runtime());
    }

    private void InDetectZone(Collider2D d)
    {
        if (!isInCooldown && !isPoisonActive)
        {
            TogglePoison(true);
            StartCoroutine(Runtime());
        }
    }

    private void OnEnterDamageZone(Collider2D d)
    {
        Debug.Log("Enter");
        poisonDamageTimer = poisonDamageInterval;
        target = d;
    }

    private void InDamageZone(Collider2D collider2D)
    {
        if (poisonDamageTimer >= poisonDamageInterval)
        {
            target = collider2D;    
            Trigger();
            poisonDamageTimer = 0;
        }
        else
        {
            poisonDamageTimer += Time.deltaTime;
        }
    }

    private void OnExitDamageZone(Collider2D d)
    {   
        Debug.Log("Exit");
        target = null;
    }

    public IEnumerator Runtime()
    {
        yield return new WaitForSeconds(poisonDuration);
        TogglePoison(false);
        yield return ActivateCooldown();
    }
}
