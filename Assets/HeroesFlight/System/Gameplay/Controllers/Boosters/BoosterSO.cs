using ScriptableObjectDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Booster", menuName = "Boost/Booster", order = 1)]
public class BoosterSO : ScriptableObject, IHasID
{
    [SerializeField] private string boosterName;
    [SerializeField] private Sprite boosterSprite;
    [SerializeField] private BoosterEffectType boosterEffectType;
    [SerializeField] private BoosterStackType boosterStackType;
    [SerializeField] private float boosterValue;
    [SerializeField] private float boosterDuration;

    public string BoosterName => boosterName;
    public Sprite BoosterSprite => boosterSprite;

    public BoosterEffectType BoosterEffectType => boosterEffectType;

    public BoosterStackType BoosterStackType => boosterStackType;

    public float BoosterValue => boosterValue;

    public float BoosterDuration => boosterDuration;

    public string GetID()
    {
        return BoosterName;
    }
}

public enum BoosterEffectType
{
    Health,
    Attack,
    Defense,
    MoveSpeed,
    AttackSpeed
}

public enum BoosterStackType
{
    None,
    Duration,
    Effect
}

[Serializable]
public class Boost
{
    public BoosterSO boosterSO;
    public Action OnStart;
    public Action OnEnd;

    public Boost(BoosterSO boosterSO, Action onStart,Action onEnd)
    {
        this.boosterSO = boosterSO;
        OnStart = onStart;
        OnEnd = onEnd;
    }
}