using HeroesFlightProject.System.Combat.Controllers;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityManager : MonoBehaviour, IActiveAbilityInterface
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ActiveAbilitySO[] passiveActiveAbilities;

    public TimedAbilityController PassiveAbilityOneController => passiveAbilityOneController;
    public TimedAbilityController PassiveAbilityTwoController   => passiveAbilityTwoController;
    public TimedAbilityController PassiveAbilityThreeController => passiveAbilityThreeController;

    private TimedAbilityController passiveAbilityOneController = new TimedAbilityController();
    private TimedAbilityController passiveAbilityTwoController = new TimedAbilityController();
    private TimedAbilityController passiveAbilityThreeController = new TimedAbilityController();

    private List<PassiveActiveAbilityType> passiveActiveAbilityTypes = new List<PassiveActiveAbilityType>();
    private List<TimedAbilityController> timedAbilitySlots = new List<TimedAbilityController>();
    private Dictionary<PassiveActiveAbilityType, ActiveAbilitySO> passiveActiveAbilitiesDic = new Dictionary<PassiveActiveAbilityType, ActiveAbilitySO>();
    private Dictionary<PassiveActiveAbilityType, TimedAbilityController> passiveAbiltyAndControllerDic = new Dictionary<PassiveActiveAbilityType, TimedAbilityController>();
    private Dictionary<PassiveActiveAbilityType, PassiveActiveAbility> activePassiveActivities = new Dictionary<PassiveActiveAbilityType, PassiveActiveAbility>();

    private void Awake()
    {
        Cache();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EquippedAbility(PassiveActiveAbilityType.KnifeFluffy);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Detach(passiveAbiltyAndControllerDic[PassiveActiveAbilityType.KnifeFluffy], activePassiveActivities[PassiveActiveAbilityType.KnifeFluffy]);
        }
    }

    public void Cache()
    {
        for (int i = 0; i < passiveActiveAbilities.Length; i++)
        {
            passiveActiveAbilityTypes.Add(passiveActiveAbilities[i].PassiveActiveAbilityType);
            passiveActiveAbilitiesDic.Add(passiveActiveAbilities[i].PassiveActiveAbilityType, passiveActiveAbilities[i]);
        }

        //PassiveAbilityOneController = new TimedAbilityController();
        //PassiveAbilityTwoController = new TimedAbilityController();
        //PassiveAbilityThreeController = new TimedAbilityController();

        timedAbilitySlots.Add(passiveAbilityOneController);
        timedAbilitySlots.Add(passiveAbilityTwoController);
        timedAbilitySlots.Add(passiveAbilityThreeController);
    }

    public void EquippedAbility(PassiveActiveAbilityType passiveActiveAbilityType)
    {
        if (AbilityAlreadyEquipped(passiveActiveAbilityType))
        {
            UpgradeAbility(passiveActiveAbilityType);
            Debug.Log("Ability Upgraded");  
            return;
        }

        foreach (TimedAbilityController timedAbilityController in timedAbilitySlots)
        {
            if (!timedAbilityController.IsValid)
            {
                PassiveActiveAbility passiveActiveAbility = passiveActiveAbilitiesDic[passiveActiveAbilityType].GetAbility(playerTransform.position);
                Init(timedAbilityController, passiveActiveAbility);
                passiveAbiltyAndControllerDic.CreateOrAdd(passiveActiveAbilityType, timedAbilityController);
                return;
            }
        }
    }

    public void Init(TimedAbilityController timedAbilityController, PassiveActiveAbility passiveActiveAbility)
    {
        timedAbilityController.Init(this, passiveActiveAbility.ActiveAbilitySO.Duration, passiveActiveAbility.ActiveAbilitySO.Cooldown);
        timedAbilityController.OnActivated += passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted += passiveActiveAbility.OnCoolDownStarted;
        timedAbilityController.OnCoolDownEnded += passiveActiveAbility.OnCoolDownEnded;

        activePassiveActivities.CreateOrAdd(passiveActiveAbility.PassiveActiveAbilityType, passiveActiveAbility);
    }

    public void Detach(TimedAbilityController timedAbilityController, PassiveActiveAbility passiveActiveAbility)
    {
        timedAbilityController.OnActivated -= passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted -= passiveActiveAbility.OnCoolDownStarted;
        timedAbilityController.OnCoolDownEnded -= passiveActiveAbility.OnCoolDownEnded;
        timedAbilityController.Refresh();

        activePassiveActivities.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        Destroy(passiveActiveAbility.gameObject);
    }

    public void SwapAbility(PassiveActiveAbilityType currentAbility, PassiveActiveAbilityType newAbility)
    {
        Detach(passiveAbiltyAndControllerDic[currentAbility], activePassiveActivities[currentAbility]);
        Init(passiveAbiltyAndControllerDic[currentAbility], activePassiveActivities[newAbility]);
    }

    public void UpgradeAbility(PassiveActiveAbilityType passiveActiveAbilityType)
    {
        activePassiveActivities[passiveActiveAbilityType].LevelUp();
    }

    bool AbilityAlreadyEquipped(PassiveActiveAbilityType passiveActiveAbilityType)
    {
        return activePassiveActivities.ContainsKey(passiveActiveAbilityType);
    }

    public List<PassiveActiveAbilityType> RandomAbility(int amount)
    {
        List<PassiveActiveAbilityType> avaliableAbilities = new List<PassiveActiveAbilityType>(passiveActiveAbilityTypes);
        List<PassiveActiveAbilityType> randomAbilities = new List<PassiveActiveAbilityType>();
        for (int i = 0; i < amount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);
            randomAbilities.Add(passiveActiveAbilityTypes[randomIndex]);
            avaliableAbilities.RemoveAt(randomIndex);
        }
        return randomAbilities;
    }
}

public static class DictionaryExtension
{
    public static void CreateOrAdd<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}