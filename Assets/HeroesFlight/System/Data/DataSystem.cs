using StansAssets.Foundation.Extensions;
using System;
using UnityEngine.SceneManagement;

public class DataSystem : DataSystemInterface
{
    public DataSystem()
    {

    }
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
 
    public EnergyManager EnergyManager { get; private set; }
    public WorldManager WorldManger { get; private set; }

    public TutorialDataHolder TutorialDataHolder { get; private set; }

    // Todo: Remove this
    public bool TutorialMode => !TutorialDataHolder.GetData.IsCompleted;
    public void Init(Scene scene = default, Action onComplete = null)
    {
        CurrencyManager = scene.GetComponent<CurrencyManager>();
        StatManager = scene.GetComponent<StatManager>();
        StatPoints = scene.GetComponent<StatPoints>();
        CharacterManager = scene.GetComponent<CharacterManager>();
        AccountLevelManager = scene.GetComponent<AccountLevelManager>();
        WorldManger = scene.GetComponent<WorldManager>();
        EnergyManager = scene.GetComponent<EnergyManager>();
        TutorialDataHolder = scene.GetComponent<TutorialDataHolder>();

        TutorialDataHolder.Init();

        CurrencyManager.LoadCurrencies();

        StatManager.Init();

        StatPoints.OnValueChanged += StatManager.ProcessStatPointsModifiers;
        StatPoints.Init();

        CharacterManager.OnCharacterChanged += (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        CharacterManager.Init(CurrencyManager);

        AccountLevelManager.OnLevelUp += StatPoints.AddPoints;

        AccountLevelManager.Init();

        EnergyManager.Initialize(CurrencyManager);

        onComplete?.Invoke();
    }

    public void Reset()
    {
        StatPoints.OnValueChanged -= StatManager.ProcessStatPointsModifiers;
        CharacterManager.OnCharacterChanged -= (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        AccountLevelManager.OnLevelUp -= StatPoints.AddPoints;
    }
}
