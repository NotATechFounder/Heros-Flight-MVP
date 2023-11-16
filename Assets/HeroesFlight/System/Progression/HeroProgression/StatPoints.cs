using HeroesFlight.Common.Progression;
using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPoints : MonoBehaviour
{
    [SerializeField] private StatPointSO[] statPointSO;
    [SerializeField] private SkillPointData skillPointData;
    private Dictionary<StatAttributeType, int> statPointsDic = new Dictionary<StatAttributeType, int>();

    private void Start()
    {
        Init();
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddXp (StatAttributeType.Power, 1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RemoveXp(StatAttributeType.Power, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Confirm();
        }
    }

    public void Init()
    {
        foreach (StatPointSO statPointSo in statPointSO)
        {
            statPointsDic.Add(statPointSo.StatPointType, 0);
        }
    }

    public void Load()
    {
        SkillPointData savedSkillPointData = FileManager.Load<SkillPointData>("SkillPoint");
        skillPointData = savedSkillPointData != null ? savedSkillPointData : new SkillPointData();

        foreach (StatPointSingleData statPointSingleData in skillPointData.statPointSingleDatas)
        {
            statPointsDic[statPointSingleData.statPointType] = statPointSingleData.sp;
        }
    }

    public void Confirm()
    {
        foreach (var statPointSo in statPointsDic)
        {
            skillPointData.SetXp(statPointSo.Key, statPointSo.Value);
        }

        Save();
    }   

    public void Save()
    {
        FileManager.Save("SkillPoint", skillPointData);
    }

    public void AddXp(StatAttributeType statPointType, int amount)
    {
        statPointsDic[statPointType] += amount;
    }

    public void RemoveXp(StatAttributeType statPointType, int amount)
    {
        if (statPointsDic[statPointType] > 0)
        {
            statPointsDic[statPointType] -= amount;
        }
    }

    public int GetXp(StatAttributeType statPointType)
    {
        return statPointsDic[statPointType];
    }

    public void ResetXp(StatAttributeType statPointType)
    {
        statPointsDic[statPointType] = 0;
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
            if (heroProgressionSingleData.statPointType == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp = sp;
                return;
            }
        }

        StatPointSingleData heroProgressionSingleData1 = new StatPointSingleData();
        heroProgressionSingleData1.statPointType = heroProgressionAttribute;
        heroProgressionSingleData1.sp = sp;
        statPointSingleDatas.Add(heroProgressionSingleData1);
    }

    public int GetSp(StatAttributeType heroProgressionAttribute)
    {
        foreach (var heroProgressionSingleData in statPointSingleDatas)
        {
            if (heroProgressionSingleData.statPointType == heroProgressionAttribute)
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
            if (heroProgressionSingleData.statPointType == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp = (statModificationType == StatModel.StatModificationType.Addition) ? heroProgressionSingleData.sp + amount : heroProgressionSingleData.sp - amount;
                return;
            }
        }

        if (statModificationType == StatModel.StatModificationType.Addition)
        {
            StatPointSingleData newData = new StatPointSingleData();
            newData.statPointType = heroProgressionAttribute;
            newData.sp = amount;
            statPointSingleDatas.Add(newData);
        }
    }
}

[Serializable]
public class StatPointSingleData
{
    public StatAttributeType statPointType;
    public int sp;
}