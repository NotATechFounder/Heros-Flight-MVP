using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveActiveAbility : MonoBehaviour
{
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] GameObject[] effectParticle;
    protected int maxLevel = 15;
    protected int manjorBoostLevel = 3;

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
        if (currentLevel >= maxLevel)
        {
            return;
        }

        currentLevel++;
        if (currentLevel % 5 == 0)
        {
            LevelUpIncreaseEffect();
        }
    }

    public int GetMajorValueByLevel(int baseValue, int increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel) / manjorBoostLevel) * increasePerLevel);
    }

    public float GetMajorValueByLevel(float baseValue, float increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel) / manjorBoostLevel) * increasePerLevel);
    }

    public GameObject GetEffectParticleByLevel()
    {
        return effectParticle[Mathf.FloorToInt((currentLevel) / manjorBoostLevel)];
    }

    public int GetValuePerLevel(int baseValue, int increasePerLevel)
    {
        return baseValue + ((currentLevel - 1) * increasePerLevel);
    }

    public float GetValuePerLevel(float baseValue, float increasePerLevel)
    {
        return baseValue + ((currentLevel - 1) * increasePerLevel);
    }
}
