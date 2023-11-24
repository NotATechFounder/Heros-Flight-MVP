using StansAssets.Foundation.Extensions;
using System;

using HeroesFlight.System.FileManager.Rewards;
using UnityEngine.SceneManagement;

public class DataSystem : DataSystemInterface
{
    public DataSystem()
    {
        RewardHandler = new RewardsHandler();
    }

    public RewardsHandlerInterface RewardHandler { get; private set; }
    public event Action OnApplicationQuit;
    public void RequestDataSave()
    {
        OnApplicationQuit?.Invoke();
    }

    public CharacterManager CharacterManager { get; private set; }

    public CurrencyManager CurrencyManager { get; private set; }

    public StatManager StatManager { get; private set; }

    public StatPoints StatPoints { get; private set; }

    public AccountLevelManager AccountLevelManager { get; private set; }


    public void Init(Scene scene = default, Action onComplete = null)
    {
        CurrencyManager = scene.GetComponent<CurrencyManager>();

        CurrencyManager.LoadCurrencies();

        StatManager = scene.GetComponent<StatManager>();
        StatManager.Init();

        StatPoints = scene.GetComponent<StatPoints>();
        StatPoints.OnValueChanged += StatManager.ProcessStatPointsModifiers;
        StatPoints.Init();

        CharacterManager = scene.GetComponent<CharacterManager>();
        CharacterManager.OnCharacterChanged += (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        CharacterManager.Init(CurrencyManager);

        AccountLevelManager = scene.GetComponent<AccountLevelManager>();
        AccountLevelManager.OnLevelUp += StatPoints.AddPoints;

        onComplete?.Invoke();
    }

    public void Reset()
    {
        StatPoints.OnValueChanged -= StatManager.ProcessStatPointsModifiers;
        CharacterManager.OnCharacterChanged -= (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        AccountLevelManager.OnLevelUp -= StatPoints.AddPoints;
    }
}
