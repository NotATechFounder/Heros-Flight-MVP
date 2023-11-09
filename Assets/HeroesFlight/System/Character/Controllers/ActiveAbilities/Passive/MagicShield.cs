using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : PassiveActiveAbility
{
    [SerializeField] private int baseHealthAbsorption;
    [SerializeField] private int healthAbsorptionPerIncrease;

    private int currentHealthAbsorption;
    private HealthController characterHealthController;
    bool forceEnd = false;

    public override void OnActivated()
    {
        forceEnd = false;

        GetEffectParticleByLevel().SetActive(true);

        currentHealthAbsorption = GetMajorValueByLevel (baseHealthAbsorption, healthAbsorptionPerIncrease);
        characterHealthController.SetInvulnerableState (true);
        characterHealthController.OnHitWhileImmortal += OnHitWhileImmortal;
    }

    public override void OnCoolDownStarted()
    {
        if (forceEnd)
        {
            return;
        }
        GetEffectParticleByLevel().SetActive(false);
        characterHealthController.SetInvulnerableState(false);
        characterHealthController.OnHitWhileImmortal -= OnHitWhileImmortal;
    }


    public override void OnCoolDownEnded()
    {

    }

    public void Initialize(int level , HealthController characterHealthController)
    {
        this.characterHealthController = characterHealthController;
        this.currentLevel = level;
    }

    private void OnHitWhileImmortal(HealthModificationIntentModel model)
    {
        currentHealthAbsorption -= (int)model.Amount;

        if (currentHealthAbsorption <= 0)
        {
            OnCoolDownStarted();
            forceEnd = true;
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
