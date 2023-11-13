using HeroesFlight.System.Character;
using Pelumi.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GodsBenevolence : MonoBehaviour
{
    public Action OnTrigger;
    [SerializeField] private GodsBenevolenceSO[] godsBenevolenceArray;

    private CharacterStatController characterStatController;
    private GodsBenevolenceSO currentBenevolenceSO;
    private List<GodsBenevolenceAfterEffectInfo> afterEffectGodsBenevolences = new List<GodsBenevolenceAfterEffectInfo>();
    private GameObject benevolenceEffect;
    private GodsBenevolenceSocket benevolenceSocket;

    public GodsBenevolenceSO CurrentBenevolenceSO => currentBenevolenceSO;

    [field: SerializeField] public float CurrentLifeSteal;

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        benevolenceSocket = characterStatController.GetComponent<GodsBenevolenceSocket>();
    }

    public void ActivateGodsBenevolence(GodBenevolenceType godBenevolenceType)
    {
        GodsBenevolenceSO godsBenevolence = GetGodsBenevolenceSO(godBenevolenceType);
        if (currentBenevolenceSO != null || godsBenevolence == null)
        {
            return;
        }

        currentBenevolenceSO = godsBenevolence;

        switch (godsBenevolence.BenevolenceType)
        {
            case GodBenevolenceType.Ares:
                benevolenceEffect = ObjectPoolManager.SpawnObject(godsBenevolence.EffectPrefab, benevolenceSocket.TopSocket);

                CurrentLifeSteal += godsBenevolence.GetValue("LifeSteal");

                float damageInc = godsBenevolence.GetValue("DamageInc");
                damageInc = StatCalc.GetPercentage(characterStatController.CurrentPhysicalDamage, damageInc);
                benevolenceEffect.GetComponent<AresEffect>().SetUp(damageInc, characterStatController.GetComponent<CharacterControllerInterface>(), OnEnemyKilled);
                break;
            case GodBenevolenceType.Apollo:
                float damagePercentage = godsBenevolence.GetValue("DamagePercentage");
                float damage = StatCalc.GetPercentage(characterStatController.CurrentPhysicalDamage, damagePercentage);
                benevolenceEffect = ObjectPoolManager.SpawnObject(godsBenevolence.EffectPrefab, benevolenceSocket.TopSocket);
                benevolenceEffect.GetComponent<ApolloEffect>().SetUp(damage, characterStatController.GetComponent<CharacterControllerInterface>());
                break;
            case GodBenevolenceType.Hercules:
                float damagePerc = godsBenevolence.GetValue("DamagePercentage");
                float dam = StatCalc.GetPercentage(characterStatController.CurrentPhysicalDamage, damagePerc);
                benevolenceEffect = ObjectPoolManager.SpawnObject(godsBenevolence.EffectPrefab, benevolenceSocket.TopSocket);
                benevolenceEffect.GetComponent<HerculesEffect>().SetUp(dam, characterStatController.GetComponent<CharacterControllerInterface>());

                break;
            case GodBenevolenceType.Hermes:
                benevolenceEffect = ObjectPoolManager.SpawnObject(godsBenevolence.EffectPrefab, benevolenceSocket.BottomSocket);
                characterStatController.ModifyMoveSpeed(godsBenevolence.GetValue("FlySpeedInc"), true);
                characterStatController.ModifyAttackRange(godsBenevolence.GetValue("AttackRangeInc"), true);
                break;
            case GodBenevolenceType.Sekhmet:

                break;
            case GodBenevolenceType.Hotei:

                break;
            default: break;
        }
    }

    public void DeactivateGodsBenevolence()
    {
        if (currentBenevolenceSO == null)
        {
            return;
        }

        ObjectPoolManager.ReleaseObject(benevolenceEffect);

        switch (currentBenevolenceSO.BenevolenceType)
        {
            case GodBenevolenceType.Ares:

                CurrentLifeSteal -= currentBenevolenceSO.GetValue("LifeSteal");

                break;
            case GodBenevolenceType.Apollo:

                break;
            case GodBenevolenceType.Hercules:

                break;
            case GodBenevolenceType.Hermes:
                characterStatController.ModifyMoveSpeed(currentBenevolenceSO.GetValue("FlySpeedInc"), false);
                characterStatController.ModifyAttackRange(currentBenevolenceSO.GetValue("AttackRangeInc"), false);
                break;
            case GodBenevolenceType.Sekhmet:

                break;
            case GodBenevolenceType.Hotei:

                break;
            default: break;
        }

        currentBenevolenceSO = null;
    }

    public GodsBenevolenceAfterEffectInfo AlreadyExists(GodBenevolenceType godBenevolenceType)
    {
        foreach (var benevolence in afterEffectGodsBenevolences)
        {
            if (benevolence.GodBenevolenceType == godBenevolenceType)
            {
                return benevolence;
            }
        }
        return null;
    }

    public void AddAfterEffects()
    {
        if (currentBenevolenceSO == null)
        {
            return;
        }

        GodsBenevolenceAfterEffectInfo godsBenevolenceAfterEffectInfo = afterEffectGodsBenevolences.Find(x => x.GodBenevolenceType == currentBenevolenceSO.BenevolenceType);

        if (godsBenevolenceAfterEffectInfo == null)
        {
            godsBenevolenceAfterEffectInfo = new GodsBenevolenceAfterEffectInfo(currentBenevolenceSO.BenevolenceType, currentBenevolenceSO.AfterEffects);
            afterEffectGodsBenevolences.Add(godsBenevolenceAfterEffectInfo);
        }
        else
        {
            godsBenevolenceAfterEffectInfo.IncreaseStack();
        }

        switch (currentBenevolenceSO.BenevolenceType)
        {
            case GodBenevolenceType.Ares:
                characterStatController.ModifyLifeSteal(godsBenevolenceAfterEffectInfo.GetValue("LifeSteal"), true);
                break;
            case GodBenevolenceType.Apollo:

                break;
            case GodBenevolenceType.Hermes:

                characterStatController.ModifyMoveSpeed(godsBenevolenceAfterEffectInfo.GetValue("FlySpeedInc"), true);
                characterStatController.ModifyAttackRange(godsBenevolenceAfterEffectInfo.GetValue("AttackRangeInc"), true);
                break;
            case GodBenevolenceType.Sekhmet:

                break;
            case GodBenevolenceType.Hotei:

                break;
            default: break;
        }
    }

    public void RemoveAfterEffects()
    {
        foreach (GodsBenevolenceAfterEffectInfo benevolenceAfterEffectInfo in afterEffectGodsBenevolences)
        {
            switch (benevolenceAfterEffectInfo.GodBenevolenceType)
            {
                case GodBenevolenceType.Ares:
                    characterStatController.ModifyLifeSteal(benevolenceAfterEffectInfo.GetTotalValue("LifeSteal"), false);
                    break;
                case GodBenevolenceType.Apollo:

                    break;
                case GodBenevolenceType.Hermes:

                    characterStatController.ModifyMoveSpeed(benevolenceAfterEffectInfo.GetTotalValue("FlySpeedInc"), false);
                    characterStatController.ModifyAttackRange(benevolenceAfterEffectInfo.GetTotalValue("AttackRangeInc"), false);

                    break;
                case GodBenevolenceType.Sekhmet:

                    break;
                case GodBenevolenceType.Hotei:

                    break;
                default: break;
            }
        }
        afterEffectGodsBenevolences.Clear();
    }    
    
    public void OnEnemyKilled()
    {
        float healthInc = StatCalc.GetPercentage(characterStatController.PlayerStatData.Health, CurrentLifeSteal);
        characterStatController.ModifyHealth(healthInc, true);
    }

    public void TriggerGodsBenevolence()
    {
        OnTrigger?.Invoke();
    }

    public GodsBenevolenceVisualData GetRandomGodsBenevolenceVisualSO(GodBenevolenceType selectedBenevolence)
    {
        if (godsBenevolenceArray.Length == 0)
        {
            return null;
        }

        if (godsBenevolenceArray.Length == 1)
        {
            return godsBenevolenceArray[0].BenevolenceVisualSO;
        }

        GodBenevolenceType random = selectedBenevolence;
        do
        {
            random = godsBenevolenceArray[UnityEngine.Random.Range(0, godsBenevolenceArray.Length)].BenevolenceType;
        } while (random == selectedBenevolence);

        // TODO: Remove this
        random = godsBenevolenceArray[1].BenevolenceType;

        GodsBenevolenceSO godsBenevolenceSO = GetGodsBenevolenceSO(random);
        return godsBenevolenceSO.BenevolenceVisualSO;
    }

    GodsBenevolenceSO GetGodsBenevolenceSO(GodBenevolenceType godBenevolenceType)
    {
        for (int i = 0; i < godsBenevolenceArray.Length; i++)
        {
            if (godsBenevolenceArray[i].BenevolenceType == godBenevolenceType)
            {
                return godsBenevolenceArray[i];
            }
        }
        return null;
    }
}

[Serializable]
public class GodsBenevolenceAfterEffect
{
    [SerializeField] float increasePerStack;
    [SerializeField] GodsBenevolenceKeyValue keyValue;

    public float IncreasePerStack => increasePerStack;
    public GodsBenevolenceKeyValue KeyValue => keyValue;
}

[Serializable]
public class GodsBenevolenceAfterEffectInfo
{
    [SerializeField] int stackCount = 0;
    [SerializeField] GodBenevolenceType godBenevolenceType;
    [SerializeField] GodsBenevolenceAfterEffect[] afterEffects;

    public int StackCount => stackCount;
    public GodsBenevolenceAfterEffect[] AfterEffects => afterEffects;
    public GodBenevolenceType GodBenevolenceType => godBenevolenceType;

    public GodsBenevolenceAfterEffectInfo(GodBenevolenceType godBenevolenceType, GodsBenevolenceAfterEffect[] afterEffects)
    {
        this.godBenevolenceType = godBenevolenceType;
        this.afterEffects = afterEffects;
    }

    public void IncreaseStack()
    {
        stackCount++;
    }

    public float GetValue(string key)
    {
        foreach (GodsBenevolenceAfterEffect afterEffect in afterEffects)
        {
            if (afterEffect.KeyValue.key == key)
            {
                if (stackCount > 0)
                {
                    return afterEffect.IncreasePerStack;
                }
                return afterEffect.KeyValue.GetValue();
            }
        }
        Debug.LogError("Key not found");
        return 0;
    }

    public float GetDefaultValue(string key)
    {
        foreach (GodsBenevolenceAfterEffect afterEffect in afterEffects)
        {
            if (afterEffect.KeyValue.key == key)
            {
                return afterEffect.KeyValue.GetValue();
            }
        }
        Debug.LogError("Key not found");
        return 0;
    }

    public float GetTotalValue(string key)
    {
        foreach (GodsBenevolenceAfterEffect afterEffect in afterEffects)
        {
            if (afterEffect.KeyValue.key == key)
            {
                return afterEffect.KeyValue.GetValue() + (afterEffect.IncreasePerStack * stackCount);
            }
        }
        Debug.LogError("Key not found");
        return 0;
    }
}

public enum GodBenevolenceTarget
{
    All,
    Melee,
    Archer,
    Mage
}

[System.Serializable]
public class GodsBenevolenceKeyValue
{
    public string key;
    public float Value;

    public float GetValue()
    {
        return Value;
    }
}