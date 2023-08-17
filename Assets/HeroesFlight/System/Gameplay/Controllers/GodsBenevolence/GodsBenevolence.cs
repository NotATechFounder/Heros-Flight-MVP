using HeroesFlight.System.Character;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsBenevolence : MonoBehaviour
{
    [SerializeField] private GodsBenevolenceSO[] godsBenevolenceArray;

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private GodsBenevolenceSO debugBenevolenceType;
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private GodsBenevolenceSO currentBenevolenceSO;
    [SerializeField] private List<GodsBenevolenceAfterEffectInfo> afterEffectGodsBenevolences = new List<GodsBenevolenceAfterEffectInfo>();
    [SerializeField] private GameObject benevolenceEffect;
    [SerializeField] private GodsBenevolenceSocket benevolenceSocket;

    public GodsBenevolenceSO[] GodsBenevolenceSOs => godsBenevolenceArray;
    public GodsBenevolenceSO CurrentBenevolenceSO => currentBenevolenceSO;

    [field: SerializeField] public float CurrentLifeSteal;

    private void Start()
    {
        if (debug)
        {
            Initialize(characterStatController);
        }
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        benevolenceSocket = characterStatController.GetComponent<GodsBenevolenceSocket>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateGodsBenevolence(debugBenevolenceType);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DeactivateGodsBenevolence();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemoveAfterEffects();
        }
    }

    public void ActivateGodsBenevolence(GodsBenevolenceSO godsBenevolence)
    {
        if (currentBenevolenceSO != null)
        {
            return;
        }

        currentBenevolenceSO = godsBenevolence;

        switch (godsBenevolence.BenevolenceType)
        {
            case GodBenevolenceType.Zeus:

                break;
            case GodBenevolenceType.Ares:
                benevolenceEffect = ObjectPoolManager.SpawnObject(godsBenevolence.EffectPrefab, benevolenceSocket.TopSocket);

                CurrentLifeSteal += godsBenevolence.GetValue("LifeSteal");

                float damageInc = godsBenevolence.GetValue("DamageInc");
                damageInc = StatCalc.GetPercentage(characterStatController.PlayerStatData.PhysicalDamage.GetRandomValue(), damageInc);
                benevolenceEffect.GetComponent<AresSword>().SetUp(characterStatController.GetComponent<CharacterControllerInterface>(), damageInc, OnEnemyKilled);
                break;
            case GodBenevolenceType.Apollo:

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
            case GodBenevolenceType.Zeus:

                break;
            case GodBenevolenceType.Ares:

                CurrentLifeSteal -= currentBenevolenceSO.GetValue("LifeSteal");

                break;
            case GodBenevolenceType.Apollo:

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

        AddAfterEffects();

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
            case GodBenevolenceType.Zeus:

                break;
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
                case GodBenevolenceType.Zeus:

                    break;
                case GodBenevolenceType.Ares:
                    Debug.Log("LifeSteal: " + benevolenceAfterEffectInfo.GetTotalValue("LifeSteal"));
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

public enum GodBenevolenceType
{
    Zeus,
    Ares,
    Apollo,
    Hermes,
    Sekhmet,
    Hotei
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