using Codice.CM.Common;
using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLevelManager : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public int currentLevel;
        public float currentExp;
    }


    public event Action<int, int> OnLevelUp;

    [SerializeField] private string saveID;
    [SerializeField] private CustomAnimationCurve levelCurve;
    [SerializeField] private LevelSystem levelSystem;
    [Header("Debug")]
    [SerializeField] private int acculatedExp;
    [SerializeField] private Data data;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Test();
    }

    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //levelSystem.AddExp(100);
            //Debug.Log("Lvl." + testLevel + " - Exp" + levelSystem.GetExpForLevel (testLevel));

            for (int i = 0; i < 16; i++)
            {
                Debug.Log("Lvl." + i + " - Exp" + levelSystem.GetExpForLevel(i));
            }
        }
    }

    public void Init()
    {
        Load();
        levelSystem.OnLevelUp += (response) =>
        {
            OnLevelUp?.Invoke(response.numberOfLevelsGained, response.currentLevel);
        };
    }

    public void AddAcculatedExp(int exp)
    {
        acculatedExp += exp;
    }

    public void EvaluateAcculatedExp()
    {
        AddExp(acculatedExp);
        acculatedExp = 0;
    }

    public void AddExp(float exp)
    {
        levelSystem.AddExp(exp);
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(saveID);
        data = savedData != null ? savedData : new Data();
        levelSystem = new LevelSystem(data.currentLevel, data.currentExp, levelCurve);
    }

    public void Save()
    {
        data.currentLevel = levelSystem.CurrentLevel;
        data.currentExp = levelSystem.CurrentExp;
        FileManager.Save(saveID, data);
    }

    private void OnDrawGizmosSelected()
    {
        if (levelSystem.LevelCurve != null)
        {
            levelSystem.LevelCurve.UpdateCurve();
            Gizmos.color = Color.red;
            for (int i = 0; i < levelSystem.LevelCurve.maxLevel; i++)
            {
                Gizmos.DrawSphere(new Vector3(i, levelSystem.LevelCurve.GetCurrentValueInt(i) / 10) + transform.position, 1f);
            }
        }
    }
}

[Serializable]
public class LevelSystem
{
    public class ExpIncreaseResponse
    {
        public int numberOfLevelsGained;
        public int currentLevel;
        public float normalizedExp;
    }

    public event Action<ExpIncreaseResponse> OnLevelUp;

    private CustomAnimationCurve levelCurve;
    private int currentLevel;
    private float currentExp;

    public CustomAnimationCurve LevelCurve => levelCurve;
    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;

    public LevelSystem(int currenLevel, float currentExp, CustomAnimationCurve levelCurve)
    {
        this.currentLevel = currenLevel;
        this.currentExp = currentExp;
        this.levelCurve = levelCurve;
    }

    public int TotalExpAccumulated(int currentLevel) => levelCurve.GetTotalValue(currentLevel);
    public float GetNormalizedExp() => currentExp / GetExpForNextLevel(currentLevel);
    public int GetExpForLevel(int level) => levelCurve.GetCurrentValueInt(level);
    public float GetExpForNextLevel(int currentLevel) => levelCurve.GetCurrentValueInt(currentLevel + 1);

    public void AddExp(float exp)
    {
        currentExp += exp;
        int numberOfLevelsGained = 0;
        while (currentExp >= GetExpForNextLevel(currentLevel))
        {
            currentExp -= GetExpForNextLevel(currentLevel);
            currentLevel++;
            ++numberOfLevelsGained;
        }

        OnLevelUp?.Invoke( new ExpIncreaseResponse
        {
            numberOfLevelsGained = numberOfLevelsGained,
            currentLevel = currentLevel,
            normalizedExp = GetNormalizedExp()
        });
    }
}