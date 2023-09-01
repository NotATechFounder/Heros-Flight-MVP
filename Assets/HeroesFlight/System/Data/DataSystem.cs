using StansAssets.Foundation.Extensions;
using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Rewards;
using UnityEngine.SceneManagement;

public class DataSystem : DataSystemInterface
{
    public DataSystem()
    {
        RewardHandler = new RewardsHandler();
        unlockedCharacters.Add(CharacterType.Tagon);
    }
    CurrencyManager currencyManager;
    public RewardsHandlerInterface RewardHandler { get; private set; }

    public event Action<CurrencySO, bool> OnCurrencyChange;
    List<CharacterType> unlockedCharacters = new();

    public void Init(Scene scene = default, Action onComplete = null)
    {
        currencyManager = scene.GetComponent<CurrencyManager>();
        currencyManager.OnCurrencyChange += OnCurrencyChange;
        LoadCurrencies();
        onComplete?.Invoke();
    }

    public void Reset()
    {
        currencyManager.OnCurrencyChange -= OnCurrencyChange;
    }

    public void AddCurency(string key, float amount)
    {
        currencyManager.AddCurency(key, amount);
    }

    public float GetCurrencyAmount(string currencyKey)
    {
        return currencyManager.GetCurrencyAmount(currencyKey);
    }

    public void ReduceCurency(string key, float amount)
    {
        currencyManager.ReduceCurency(key, amount);
    }

    public void SetCurencyAmount(string key, float amount)
    {
        currencyManager.SetCurencyAmount(key, amount);
    }

    public void SaveCurrencies()
    {
        currencyManager.SaveCurrencies();
    }

    public void LoadCurrencies()
    {
        currencyManager.LoadCurrencies();
    }

    public List<CharacterType> GetUnlockedHeroes()
    {
        return unlockedCharacters;
    }

    public void UnlockHero(CharacterType type)
    {
        if (!unlockedCharacters.Contains(type))
        {
            unlockedCharacters.Add(type);
        }
    }
}
