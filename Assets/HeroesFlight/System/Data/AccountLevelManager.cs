using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLevelManager : MonoBehaviour
{
    public event Action<int, int> OnLevelUp;

    [SerializeField] private CustomAnimationCurve levelCurve;
    [Header("Debug")]
    [SerializeField] private Data data;

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(100);
        }
    }

    public int TotalExpAccumulated(int currentLevel) => levelCurve.GetTotalValue(currentLevel);
    public float GetNormalizedExp() => data.currentExp / GetExpForNextLevel(data.currentLevel);
    public int GetExpForLevel(int level) => levelCurve.GetCurrentValueInt(level);
    public float GetExpForNextLevel(int currentLevel) => levelCurve.GetCurrentValueInt(currentLevel + 1);

    public void AddExp(float exp)
    {
        data.currentExp += exp;
        int numberOfLevelsGained = 0;
        while (data.currentExp >= GetExpForNextLevel(data.currentLevel))
        {
            Debug.Log("Exp: " + data.currentExp + " / " + GetExpForNextLevel(data.currentLevel));
            data.currentExp -= GetExpForNextLevel(data.currentLevel);
            data.currentLevel++;
            ++numberOfLevelsGained;
        }

        if (numberOfLevelsGained > 0)
        {
            OnLevelUp?.Invoke(data.currentLevel, numberOfLevelsGained);
        }
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

    private void OnDrawGizmosSelected()
    {
        if (levelCurve != null)
        {
            levelCurve.UpdateCurve();
            Gizmos.color = Color.red;
            for (int i = 0; i < levelCurve.maxLevel; i++)
            {
                Gizmos.DrawSphere(new Vector3(i, levelCurve.GetCurrentValueInt(i) / 10) + transform.position, 1f);
            }
        }
    }

    [Serializable]
    public class Data
    {
        public int currentLevel;
        public float currentExp;
    }
}
