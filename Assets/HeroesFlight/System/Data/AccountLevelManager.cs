using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLevelManager : MonoBehaviour
{
    public event Action<int> OnLevelUp;
    public event Action<int, int, float> OnEXPAdded;

    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;

    [Header("Debug")]
    [SerializeField] private Data data;
    private float expToNextLevel => expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, data.currentLevel);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(100);
        }
    }

    public void AddExp(float exp)
    {
        data.currentExp += exp;
        if (data.currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        else
        {
            OnEXPAdded?.Invoke(0, 0, data.currentExp / expToNextLevel);
        }
    }

    private void LevelUp()
    {
        int currentLvl = data.currentLevel;
        int numberOfLevelsGained = 0;
        do
        {
            LevelUpOnce();
            ++numberOfLevelsGained;
        } while (data.currentExp >= expToNextLevel);

        OnEXPAdded?.Invoke(currentLvl, numberOfLevelsGained, data.currentExp / expToNextLevel);
    }

    private void LevelUpOnce()
    {
        data.currentExp -= expToNextLevel;
        data.currentLevel++;
    }

    public float GetNormalizedExp()
    {
        return data.currentExp / expToNextLevel;
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>("AccountLevel");
        data = savedData != null ? savedData : new Data();
    }

    public void Save()
    {
        FileManager.Save( "AccountLevel",data);
    }

    [System.Serializable]
    public class Data
    {
        public int currentLevel;
        public float currentExp;
    }
}
