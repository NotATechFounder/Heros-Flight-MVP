using ScriptableObjectDatabase;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Ability / Passive Ability")]
public class PassiveAbilitySO : ScriptableObject, IHasID
{
    [SerializeField] PassiveAbilityVisualData regularAbilityVisualData;
    [SerializeField] PassiveAbilityKeyValue[] passiveAbilityKeyValues;
    private int maxLevel = 9;

    public PassiveAbilityVisualData GetAbilityVisualData => regularAbilityVisualData;

    public string GetID()
    {
        return regularAbilityVisualData.PassiveActiveAbilityType.ToString();
    }

    public float GetLevelValue(string key, int level = 0)
    {
        foreach (var item in passiveAbilityKeyValues)
        {
            if (item.key == key)
            {
                return item.startValue + item.increasePerLevel * level;
            }
        }
        return 0;
    }

    public float GetValueIncrease(string key, bool  isFirstLevel)
    {
        foreach (var item in passiveAbilityKeyValues)
        {
            if (item.key == key)
            {
                return isFirstLevel ? item.startValue : item.increasePerLevel;
            }
        }
        return 0;
    }

    public bool IsMaxLevel(int currentLevel)
    {
        return currentLevel >= maxLevel;
    }

    [System.Serializable]
    public class PassiveAbilityKeyValue
    {
        public string key;
        public float startValue;
        public float increasePerLevel;
    }
}