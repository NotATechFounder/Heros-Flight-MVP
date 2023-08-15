using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsBenevolence : MonoBehaviour
{
    [SerializeField] private GodsBenevolenceSO[] godsBenevolenceArray;

    [Header("Debug")]
    [SerializeField] private GodsBenevolenceInfo[] godsBenevolenceInfos;
    [SerializeField] private CharacterStatController characterStatController;

    public GodsBenevolenceSO[] GodsBenevolenceSOs => godsBenevolenceArray;
    public GodsBenevolenceInfo[] GodsBenevolenceInfos => godsBenevolenceInfos;

    private void Start()
    {
        Initialize(characterStatController);
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        godsBenevolenceInfos = new GodsBenevolenceInfo[godsBenevolenceArray.Length];
        for (int i = 0; i < godsBenevolenceArray.Length; i++)
        {
            godsBenevolenceInfos[i] = new GodsBenevolenceInfo(godsBenevolenceArray[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateGodsBenevolence(GodBenevolenceType.Hermes);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DeactivateGodsBenevolence(GodBenevolenceType.Hermes);
        }
    }

    public void ActivateGodsBenevolence(GodBenevolenceType godBenevolenceType)
    {
        GodsBenevolenceInfo godsBenevolenceInfo = GetGodsBenevolenceInfo(godBenevolenceType);
        godsBenevolenceInfo.AddStack();
        switch (godBenevolenceType)
        {
            case GodBenevolenceType.Zeus:

                break;
            case GodBenevolenceType.Ares:

                break;
            case GodBenevolenceType.Apollo:

                break;
            case GodBenevolenceType.Hermes:

                characterStatController.ModifyMoveSpeed(godsBenevolenceInfo.GetDefaultValue("FlySpeedInc"), true);

                break;
            case GodBenevolenceType.Sekhmet:

                break;
            case GodBenevolenceType.Hotei:

                break;
            default: break;
        }
    }

    public void DeactivateGodsBenevolence(GodBenevolenceType godBenevolenceType)
    {
        GodsBenevolenceInfo godsBenevolenceInfo = GetGodsBenevolenceInfo(godBenevolenceType);
        switch (godBenevolenceType)
        {
            case GodBenevolenceType.Zeus:

                break;
            case GodBenevolenceType.Ares:

                break;
            case GodBenevolenceType.Apollo:

                break;
            case GodBenevolenceType.Hermes:

                characterStatController.ModifyMoveSpeed(godsBenevolenceInfo.GetTotalValue("FlySpeedInc"), false);

                break;
            case GodBenevolenceType.Sekhmet:

                break;
            case GodBenevolenceType.Hotei:

                break;
            default: break;
        }
        godsBenevolenceInfo.Reset();
    }

    public GodsBenevolenceSO GetGodsBenevolenceSO(GodBenevolenceType godBenevolenceType)
    {
        foreach (var benevolence in godsBenevolenceArray)
        {
            if (benevolence.BenevolenceType == godBenevolenceType)
            {
                return benevolence;
            }
        }
        return null;
    }

    public GodsBenevolenceInfo GetGodsBenevolenceInfo(GodBenevolenceType godBenevolenceType)
    {
        foreach (var benevolence in godsBenevolenceInfos)
        {
            if (benevolence.GodsBenevolenceSO.BenevolenceType == godBenevolenceType)
            {
                return benevolence;
            }
        }
        return null;
    }
}

[Serializable]
public class GodsBenevolenceInfo
{
    [SerializeField] private GodsBenevolenceSO godsBenevolenceSO;
    [SerializeField] int stackCount = 0;

    public GodsBenevolenceSO GodsBenevolenceSO => godsBenevolenceSO;
    public int StackCount => stackCount;

    public GodsBenevolenceInfo(GodsBenevolenceSO godsBenevolenceSO)
    {
        this.godsBenevolenceSO = godsBenevolenceSO;
    }

    public void AddStack()
    {
        stackCount++;
    }

    public void RemoveStack()
    {
        stackCount--;
    }

    public void Reset()
    {
        stackCount = 0;
    }
    public float GetDefaultValue(string key)
    {
        foreach (var keyValue in godsBenevolenceSO.BenevolenceKeyValues)
        {
            if (keyValue.key == key)
            {
                return keyValue.GetValue();
            }
        }
        return 0;
    }
    public float GetTotalValue(string key)
    {
        foreach (var keyValue in godsBenevolenceSO.BenevolenceKeyValues)
        {
            if (keyValue.key == key)
            {
               return keyValue.GetValue() * stackCount;
            }
        }
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