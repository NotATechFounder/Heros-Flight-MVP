using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveActiveAbility : MonoBehaviour
{
    [SerializeField] protected int currentLevel = 1;

    private ActiveAbilitySO activeAbilitySO;

    public PassiveActiveAbilityType PassiveActiveAbilityType => activeAbilitySO.PassiveActiveAbilityType;
    public int Level => currentLevel;
    public ActiveAbilitySO ActiveAbilitySO => activeAbilitySO;

    public void Init(ActiveAbilitySO activeAbilitySO)
    {
        this.activeAbilitySO = activeAbilitySO;
    }

    public abstract void OnActivated();
    public abstract void OnCoolDownStarted();
    public abstract void OnCoolDownEnded();

    public abstract void LevelUpIncreaseEffect();

    public virtual void LevelUp()
    {
        currentLevel++;
        if (currentLevel % 5 == 0)
        {
            LevelUpIncreaseEffect();
        }
    }

    public int GetValueByLevel(int baseValue, int increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel) / 5) * increasePerLevel);
    }

    public float GetValueByLevel(float baseValue, float increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel) / 5) * increasePerLevel);
    }
}
