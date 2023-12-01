using HeroesFlight.System.Combat.Effects.Effects;
using ScriptableObjectDatabase;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Ability / Passive Ability")]
public class PassiveAbilitySO : ScriptableObject, IHasID
{
    [SerializeField] PassiveAbilityVisualData regularAbilityVisualData;
    [SerializeField] CombatEffect[] combatEffects; 

    private int maxLevel = 9;

    public PassiveAbilityVisualData GetAbilityVisualData => regularAbilityVisualData;
    public CombatEffect[] GetCombatEffects => combatEffects;

    public string GetID()
    {
        return regularAbilityVisualData.PassiveActiveAbilityType.ToString();
    }

    public bool IsMaxLevel(int currentLevel)
    {
        return currentLevel >= maxLevel;
    }
}