using HeroesFlight.Common.Progression;
using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class StatPoints : MonoBehaviour
{
    public event Action OnSpChanged;
    public event Action<Dictionary<StatAttributeType, int>> OnValueChanged;

    [SerializeField] private StatPointSO[] statPointSO;
    [SerializeField] private SkillPointData skillPointData;
    [SerializeField] private SkillPointData diceRollData;
    private Dictionary<StatAttributeType, int> statPointsDic = new Dictionary<StatAttributeType, int>();
    private Dictionary<StatAttributeType, int> tempStatPointsDic = new Dictionary<StatAttributeType, int>();
    private Dictionary<StatAttributeType, int> diceRollDic = new Dictionary<StatAttributeType, int>();
    [SerializeField] int currentSp;

    public void Init()
    {
        foreach (StatPointSO statPointSo in statPointSO)
        {
            statPointsDic.Add(statPointSo.StatAttributeType, 0);
            diceRollDic.Add(statPointSo.StatAttributeType, 0);
        }

        Load();
    }

    public void Load()
    {
        SkillPointData savedSkillPointData = FileManager.Load<SkillPointData>("SkillPoint");
        skillPointData = savedSkillPointData != null ? savedSkillPointData : new SkillPointData();

        currentSp = skillPointData.avaliableSp;

        foreach (StatPointSingleData statPointSingleData in skillPointData.statPointSingleDatas)
        {
            statPointsDic[statPointSingleData.statAttributeType] = statPointSingleData.sp;
            diceRollDic[statPointSingleData.statAttributeType] = statPointSingleData.diceRoll;
        }

        OnValueChanged ?.Invoke(statPointsDic);
    }

    public void Confirm()
    {
        foreach (var statPointSo in tempStatPointsDic)
        {
            statPointsDic[statPointSo.Key] += statPointSo.Value;
        }

        foreach (var statPointSo in statPointsDic)
        {
            skillPointData.SetXp(statPointSo.Key, statPointSo.Value);
        }

        Save();

        tempStatPointsDic.Clear();

        OnValueChanged?.Invoke(statPointsDic);
    }   

    public void Save()
    {
        skillPointData.avaliableSp = currentSp;
        FileManager.Save("SkillPoint", skillPointData);
    }

    public bool TryAddSp(StatAttributeType statPointType)
    {
        if (currentSp > 0)
        {
            if (!tempStatPointsDic.ContainsKey(statPointType))
            {
                tempStatPointsDic.Add(statPointType, 0);
            }

            tempStatPointsDic[statPointType]++;
            currentSp--;

            Debug.Log(statPointType + " " + tempStatPointsDic[statPointType]);
            return true;
        }
        return false;
    }

    public bool TrytRemoveSp(StatAttributeType statPointType)
    {
        if (!tempStatPointsDic.ContainsKey(statPointType))
        {
            return false;
        }

        if (tempStatPointsDic[statPointType] > 0)
        {
            tempStatPointsDic[statPointType]--;
            currentSp++;


            Debug.Log(statPointType + " " + tempStatPointsDic[statPointType]);

            if (tempStatPointsDic[statPointType] == 0)
            {
                tempStatPointsDic.Remove(statPointType);
            }

            return true;
        }
        return false;
    }


    public void AddPoints(int newLevel, int nuberOfPoints)
    {
        currentSp += nuberOfPoints;
        Save();
        OnSpChanged?.Invoke();
    }

    public int GetSp(StatAttributeType statPointType)
    {
        return statPointsDic[statPointType];
    }

    public void ResetSp(StatAttributeType statPointType)
    {
        statPointsDic[statPointType] = 0;
    }

    public int GetAvailableSp()
    {
        return currentSp;
    }

    public int GetDiceRollValue(StatAttributeType type)
    {
        return diceRollDic[type];
    }
}

[Serializable]
public class SkillPointData
{
    public int avaliableSp;
    public List<StatPointSingleData> statPointSingleDatas = new List<StatPointSingleData>();

    public void SetXp(StatAttributeType heroProgressionAttribute, int sp)
    {
        foreach (var heroProgressionSingleData in statPointSingleDatas)
        {
            if (heroProgressionSingleData.statAttributeType == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp = sp;
                return;
            }
        }

        StatPointSingleData heroProgressionSingleData1 = new StatPointSingleData();
        heroProgressionSingleData1.statAttributeType = heroProgressionAttribute;
        heroProgressionSingleData1.sp = sp;
        statPointSingleDatas.Add(heroProgressionSingleData1);
    }

    public int GetSp(StatAttributeType heroProgressionAttribute)
    {
        foreach (var heroProgressionSingleData in statPointSingleDatas)
        {
            if (heroProgressionSingleData.statAttributeType == heroProgressionAttribute)
            {
                return heroProgressionSingleData.sp;
            }
        }

        return 0;
    }

    public void ModifySp(StatAttributeType heroProgressionAttribute, int amount, StatModel.StatModificationType statModificationType)
    {
        foreach (var heroProgressionSingleData in statPointSingleDatas)
        {
            if (heroProgressionSingleData.statAttributeType == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp = (statModificationType == StatModel.StatModificationType.Addition) ? heroProgressionSingleData.sp + amount : heroProgressionSingleData.sp - amount;
                return;
            }
        }

        if (statModificationType == StatModel.StatModificationType.Addition)
        {
            StatPointSingleData newData = new StatPointSingleData();
            newData.statAttributeType = heroProgressionAttribute;
            newData.sp = amount;
            statPointSingleDatas.Add(newData);
        }
    }
}

[Serializable]
public class StatPointSingleData
{
    public StatAttributeType statAttributeType;
    public int sp;
    public int diceRoll;
}