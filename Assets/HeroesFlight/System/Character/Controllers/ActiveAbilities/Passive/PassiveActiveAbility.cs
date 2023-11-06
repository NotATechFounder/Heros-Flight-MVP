using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveActiveAbility : MonoBehaviour
{
    [SerializeField] protected int level;

    private ActiveAbilitySO activeAbilitySO;

    public PassiveActiveAbilityType PassiveActiveAbilityType => activeAbilitySO.PassiveActiveAbilityType;
    public int Level => level;
    public ActiveAbilitySO ActiveAbilitySO => activeAbilitySO;

    public void Init(ActiveAbilitySO activeAbilitySO)
    {
        this.activeAbilitySO = activeAbilitySO;
    }

    public abstract void OnActivated();
    public abstract void OnCoolDownStarted();
    public abstract void OnCoolDownEnded();

    public virtual void LevelUp()
    {
        level++;
    }
}
