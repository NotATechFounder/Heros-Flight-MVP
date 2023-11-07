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

    public override void OnActivated()
    {
        currentHealthAbsorption = GetValueByLevel (baseHealthAbsorption, healthAbsorptionPerIncrease);
        characterHealthController.SetInvulnerableState (true);
        characterHealthController.OnHitWhileImmortal += OnHitWhileImmortal;
    }

    public override void OnCoolDownEnded()
    {
        characterHealthController.SetInvulnerableState(false);
        characterHealthController.OnHitWhileImmortal -= OnHitWhileImmortal;
    }

    public override void OnCoolDownStarted()
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
            characterHealthController.SetInvulnerableState(false);
            characterHealthController.OnHitWhileImmortal -= OnHitWhileImmortal;
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
