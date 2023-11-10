using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : RegularActiveAbility
{
    [SerializeField] private CustomAnimationCurve healthAbsorptionCurve;

    private int currentHealthAbsorption;
    private HealthController characterHealthController;
    bool forceEnd = false;

    public override void OnActivated()
    {
        forceEnd = false;

        GetEffectParticleByLevel().SetActive(true);

        currentHealthAbsorption = healthAbsorptionCurve.GetCurrentValueInt(currentLevel);

        characterHealthController.SetShieldedState(true);
        characterHealthController.OnHitWhileIsShielded += OnHitWhileShielded;
    }

    public override void OnDeactivated()
    {
        if (forceEnd)
        {
            return;
        }
        characterHealthController.SetShieldedState(false);
        characterHealthController.OnHitWhileIsShielded -= OnHitWhileShielded;
        GetEffectParticleByLevel().SetActive(false);
    }


    public override void OnCoolDownEnded()
    {
        GetEffectParticleByLevel().SetActive(false);
    }

    public void Initialize(int level , HealthController characterHealthController)
    {
        this.characterHealthController = characterHealthController;
        this.currentLevel = level;
    }

    private void OnHitWhileShielded(HealthModificationIntentModel model)
    {
        currentHealthAbsorption -= (int)model.Amount;

        if (currentHealthAbsorption <= 0)
        {
            OnDeactivated();
            forceEnd = true;
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();


    }

    private void OnDrawGizmosSelected()
    {
        if (healthAbsorptionCurve.curveType != CurveType.Custom)
        {
            healthAbsorptionCurve.UpdateCurve();
        }
    }
}
