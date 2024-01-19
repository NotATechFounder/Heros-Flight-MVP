using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RegularActiveAbility : MonoBehaviour
{
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] ParticleSystem[] effectParticle;
    protected int maxLevel = 9;
    protected int majorBoostLevel = 3;

    private ActiveAbilitySO activeAbilitySO;

    public ActiveAbilityType PassiveActiveAbilityType => activeAbilitySO.GetAbilityVisualData.ActiveAbilityType;
    public int Level => currentLevel;
    public ActiveAbilitySO ActiveAbilitySO => activeAbilitySO;

    public void Init(ActiveAbilitySO activeAbilitySO)
    {
        this.activeAbilitySO = activeAbilitySO;
    }

    public bool IsInstant() => activeAbilitySO.Duration == 0;
    public abstract void OnActivated();

    public abstract void OnDeactivated();

    public abstract void OnCoolDownEnded();

    public virtual void LevelUp()
    {
        if (currentLevel >= maxLevel)
        {
            return;
        }
        currentLevel++;
    }

    public int GetMajorValueByLevel(int baseValue, int increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel) * increasePerLevel);
    }

    public int GetNormalisedLevel()
    {
        return Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel);
    }

    public bool IsMaxLevel()
    {
        return currentLevel >= maxLevel;
    }

    public ParticleSystem GetEffectParticleByLevel()
    {
        //return effectParticle[Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel)];
        int effetIndex = Mathf.Min(currentLevel, 3) - 1;
        return effectParticle[effetIndex];
    }

    public IEnumerator MultiCast()
    {
        yield return new WaitForSeconds(1f);
        OnActivated();
    }
}
