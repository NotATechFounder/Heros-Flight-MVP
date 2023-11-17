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
    }

    public RewardsHandlerInterface RewardHandler { get; private set; }

    public CharacterManager CharacterManager { get; private set; }

    public CurrencyManager CurrencyManager { get; private set; }

    public StatManager StatManager { get; private set; }

    public StatPoints StatPoints { get; private set; }


    public void Init(Scene scene = default, Action onComplete = null)
    {
        CurrencyManager = scene.GetComponent<CurrencyManager>();

        CurrencyManager.LoadCurrencies();

        CharacterManager = scene.GetComponent<CharacterManager>();
        CharacterManager.Init(CurrencyManager);

        StatManager = scene.GetComponent<StatManager>();

        StatPoints = scene.GetComponent<StatPoints>();

        onComplete?.Invoke();
    }

    public void Reset()
    {

    }
}
