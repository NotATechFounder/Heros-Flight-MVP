using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Active Ability/Regular Ability")]
public class ActiveAbilitySO : ScriptableObject
{
    [SerializeField] AbilityVisualData abilityVisualData;
    [SerializeField] protected float duration;
    [SerializeField] protected int cooldown;
    [SerializeField] PassiveActiveAbility passiveActiveAbility;

    public AbilityVisualData GetAbilityVisualData => abilityVisualData;
    public float Duration => duration;
    public int Cooldown => cooldown;

    public PassiveActiveAbility GetAbility(Vector3 position, Quaternion rotation = default)
    {
        PassiveActiveAbility ability = Instantiate(passiveActiveAbility, position, rotation);
        ability.Init(this);
        return ability;
    }
}
