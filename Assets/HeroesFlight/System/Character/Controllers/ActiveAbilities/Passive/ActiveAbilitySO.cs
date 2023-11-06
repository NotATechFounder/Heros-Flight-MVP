using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Active Ability/Regular Ability")]
public class ActiveAbilitySO : ScriptableObject
{
    [SerializeField] protected PassiveActiveAbilityType passiveActiveAbilityType;

    [Header("UI")]
    [SerializeField] protected Sprite icon;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    [Header("PROPERTIES")]
    [SerializeField] protected float duration;
    [SerializeField] protected int cooldown;
    [SerializeField] PassiveActiveAbility passiveActiveAbility;

    public PassiveActiveAbilityType PassiveActiveAbilityType => passiveActiveAbilityType;
    public Sprite Icon => icon;
    public string Description => description;
    public float Duration => duration;
    public int Cooldown => cooldown;

    public PassiveActiveAbility GetAbility(Vector3 position, Quaternion rotation = default)
    {
        PassiveActiveAbility ability = Instantiate(passiveActiveAbility, position, rotation);
        ability.Init(this);
        return ability;
    }
}


public enum PassiveActiveAbilityType
{
    HeavenStab,
    OrbOfLightning,
    MagicShield,
    KnifeFluffy,
    Immolation,
    LightNova,
}