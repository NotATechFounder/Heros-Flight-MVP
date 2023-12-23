using HeroesFlight.System.FileManager;
using System;
using UnityEngine;

public class AccountLevelManager : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public int currentLevel;
        public float currentExp;
    }

    public event Action<LevelSystem.ExpIncreaseResponse> OnLevelUp;

    public const string SAVE_ID = "AccountLevelData";

    [SerializeField] private CustomAnimationCurve levelCurve;
    [SerializeField] private LevelSystem levelSystem;
    [Header("Debug")]
    [SerializeField] private Data data;

    public void Init()
    {
        Load();

        levelSystem.OnLevelUp += (response) =>
        {
            OnLevelUp?.Invoke(response);
        };
    }

    public LevelSystem.ExpIncreaseResponse GetExpIncreaseResponse()
    {
        return new LevelSystem.ExpIncreaseResponse
        {
            numberOfLevelsGained = 0,
            currentLevel = levelSystem.CurrentLevel,
            normalizedExp = levelSystem.GetNormalizedExp()
        };
    }

    public void AddExp(float exp)
    {
        levelSystem.AddExp(exp);
        Save();
    }

    public void SetXp(float exp)
    {
        levelSystem.SetExp(exp);
        Save();
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(SAVE_ID);
        data = savedData != null ? savedData : new Data();
        levelSystem = new LevelSystem(data.currentLevel, data.currentExp, levelCurve);
    }

    public void Save()
    {
        data.currentLevel = levelSystem.CurrentLevel;
        data.currentExp = levelSystem.CurrentExp;
        FileManager.Save(SAVE_ID, data);
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
