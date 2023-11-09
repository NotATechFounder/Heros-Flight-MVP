using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityManager : MonoBehaviour
{
    public bool test;

    public Action<int, RegularAbilityVisualData> OnActiveAbilityEquipped;
    public Action<int, PassiveAbilityVisualData> OnPassiveAbilityEquipped;

    [SerializeField] private RegularActiveAbilityDatabase allActiveAbilities;
    [SerializeField] private PassiveAbilityDatabase allPassiveAbilities;

    public TimedAbilityController RegularAbilityOneController => regularAbilityOneController;
    public TimedAbilityController RegularAbilityTwoController   => regularAbilityTwoController;
    public TimedAbilityController RegularAbilityThreeController => regularAbilityThreeController;

    public List<TimedAbilityController> TimedAbilityControllers => timedAbilitySlots;

    // Regular Active Ability
    private TimedAbilityController regularAbilityOneController = new TimedAbilityController();
    private TimedAbilityController regularAbilityTwoController = new TimedAbilityController();
    private TimedAbilityController regularAbilityThreeController = new TimedAbilityController();

    private List<RegularActiveAbilityType> regularActiveAbilityTypes = new List<RegularActiveAbilityType>();
    private List<TimedAbilityController> timedAbilitySlots = new List<TimedAbilityController>();
    private Dictionary<RegularActiveAbilityType, RegularActiveAbilitySO> allRegularActiveAbilitiesDic = new Dictionary<RegularActiveAbilityType, RegularActiveAbilitySO>();
    private Dictionary<RegularActiveAbilityType, TimedAbilityController> regularAbiltyAndControllerDic = new Dictionary<RegularActiveAbilityType, TimedAbilityController>();
    private Dictionary<RegularActiveAbilityType, RegularActiveAbility> eqquipedRegularActivities = new Dictionary<RegularActiveAbilityType, RegularActiveAbility>();

    // Passive Ability
    private List<PassiveAbilityType> passiveActiveAbilityTypes = new List<PassiveAbilityType>();
    private Dictionary<PassiveAbilityType, PassiveAbilitySO> allPassiveAbilitiesDic = new Dictionary<PassiveAbilityType, PassiveAbilitySO>();
    private Dictionary<PassiveAbilityType, int> eqquipedPassiveAbilities = new Dictionary<PassiveAbilityType, int>();

    private CharacterStatController characterStatController;
    private CharacterSimpleController characterSystem;
    private HealthController characterHealthController;
    private BaseCharacterAttackController characterAttackController;


    private void Awake()
    {
        Cache();

        if (test)
        {
            characterStatController = GetComponent<CharacterStatController>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EquippedAbility(RegularActiveAbilityType.MagicShield);
            //EquippedAbility(PassiveActiveAbilityType.HeavenStab);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DetachAbility(regularAbiltyAndControllerDic[RegularActiveAbilityType.KnifeFluffy], eqquipedRegularActivities[RegularActiveAbilityType.KnifeFluffy]);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseCharacterAbility(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseCharacterAbility(1);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseCharacterAbility(2);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddPassiveAbility(PassiveAbilityType.HeartThief);
        }
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        this.characterSystem = characterStatController.GetComponent<CharacterSimpleController>();
        this.characterHealthController = characterStatController.GetComponent<HealthController>();
        this.characterAttackController = characterStatController.GetComponent<BaseCharacterAttackController>();
    }

    public void Cache()
    {
        for (int i = 0; i < allActiveAbilities.Items.Length; i++)
        {
            regularActiveAbilityTypes.Add(allActiveAbilities.Items[i].GetAbilityVisualData.RegularActiveAbilityType);
            allRegularActiveAbilitiesDic.Add(allActiveAbilities.Items[i].GetAbilityVisualData.RegularActiveAbilityType, allActiveAbilities.Items[i]);
        }

        timedAbilitySlots.Add(regularAbilityOneController);
        timedAbilitySlots.Add(regularAbilityTwoController);
        timedAbilitySlots.Add(regularAbilityThreeController);

        for (int i = 0; i < allPassiveAbilities.Items.Length; i++)
        {
            passiveActiveAbilityTypes.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType);
            allPassiveAbilitiesDic.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType, allPassiveAbilities.Items[i]);
        }
    }

    public void EquippedAbility(RegularActiveAbilityType passiveActiveAbilityType)
    {
        if (AbilityAlreadyEquipped(passiveActiveAbilityType))
        {
            UpgradeAbility(passiveActiveAbilityType); 
            return;
        }

        foreach (TimedAbilityController timedAbilityController in timedAbilitySlots)
        {
            if (!timedAbilityController.IsValid)
            {
                InitialiseAbility (passiveActiveAbilityType, timedAbilityController);
                return;
            }
        }

        Debug.LogError("No more ability slots");    
    }

    public void InitialiseAbility(RegularActiveAbilityType passiveActiveAbilityType, TimedAbilityController timedAbilityController, int level = 1)
    {
        RegularActiveAbility passiveActiveAbility = allRegularActiveAbilitiesDic[passiveActiveAbilityType].GetAbility(characterStatController.transform.position);
        passiveActiveAbility.transform.SetParent(characterStatController.transform);

        AttachAbility(timedAbilityController, passiveActiveAbility);
        regularAbiltyAndControllerDic.CreateOrAdd(passiveActiveAbilityType, timedAbilityController);

        switch (passiveActiveAbilityType)
        {
            case RegularActiveAbilityType.HeavenStab:
                (passiveActiveAbility as HeavenStab).Initialize(level, (int)characterStatController.CurrentPhysicalDamage, characterSystem);
                break;
            case RegularActiveAbilityType.OrbOfLightning:
                (passiveActiveAbility as OrbOfLightning).Initialize(level, characterStatController);
                break;
            case RegularActiveAbilityType.MagicShield:
                (passiveActiveAbility as MagicShield).Initialize(level , characterHealthController);
                break;
            case RegularActiveAbilityType.KnifeFluffy:
                (passiveActiveAbility as KnifeFluffy).Initialize(level, (int)characterStatController.CurrentPhysicalDamage);
                break;
            case RegularActiveAbilityType.Immolation:
                (passiveActiveAbility as Immolation).Initialize(level, (int)characterStatController.CurrentMagicDamage);
                break;
            case RegularActiveAbilityType.LightNova:
                (passiveActiveAbility as LightNova).Initialize(level, characterStatController, characterSystem, characterHealthController, characterAttackController);
                break;
            case RegularActiveAbilityType.SwordWhirlwind:
                (passiveActiveAbility as SwordWhirlwind).Initialize(level, (int)characterStatController.CurrentPhysicalDamage);
                break;
            default:  break;
        }

        int index = timedAbilitySlots.IndexOf(timedAbilityController);
        OnActiveAbilityEquipped?.Invoke(index, GetActiveAbilityVisualData (passiveActiveAbilityType));
    }

    public void AttachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility passiveActiveAbility)
    {
        timedAbilityController.Init(this, passiveActiveAbility.ActiveAbilitySO.Duration, passiveActiveAbility.ActiveAbilitySO.Cooldown);
        timedAbilityController.OnActivated += passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted += passiveActiveAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded += passiveActiveAbility.OnCoolDownEnded;

        eqquipedRegularActivities.CreateOrAdd(passiveActiveAbility.PassiveActiveAbilityType, passiveActiveAbility);
    }

    public void DetachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility passiveActiveAbility)
    {
        timedAbilityController.OnActivated -= passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted -= passiveActiveAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded -= passiveActiveAbility.OnCoolDownEnded;
        timedAbilityController.Refresh();

        eqquipedRegularActivities.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        Destroy(passiveActiveAbility.gameObject);
    }

    public void SwapAbility(RegularActiveAbilityType currentAbility, RegularActiveAbilityType newAbility)
    {
        DetachAbility(regularAbiltyAndControllerDic[currentAbility], eqquipedRegularActivities[currentAbility]);
        AttachAbility(regularAbiltyAndControllerDic[currentAbility], eqquipedRegularActivities[newAbility]);
    }

    public void UpgradeAbility(RegularActiveAbilityType passiveActiveAbilityType)
    {
        eqquipedRegularActivities[passiveActiveAbilityType].LevelUp();
    }

    bool AbilityAlreadyEquipped(RegularActiveAbilityType passiveActiveAbilityType)
    {
        return eqquipedRegularActivities.ContainsKey(passiveActiveAbilityType);
    }

    public RegularActiveAbilityType GetRandomActiveAbility(RegularActiveAbilityType regularActiveAbilityTypeExeption)
    {
        List<RegularActiveAbilityType> avaliableAbilities = new List<RegularActiveAbilityType>(regularActiveAbilityTypes);
        avaliableAbilities.Remove(regularActiveAbilityTypeExeption);
        int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);
        return avaliableAbilities[randomIndex];
    }

    public List<PassiveAbilityType> GetRandomPassiveAbility(int amount, List<PassiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<PassiveAbilityType> randomAbilities = new List<PassiveAbilityType>();
        List<PassiveAbilityType> avaliableAbilities = new List<PassiveAbilityType>(passiveActiveAbilityTypes);

        for (int i = 0; i < passiveActiveAbilityTypeExeption.Count; i++)
        {
            avaliableAbilities.Remove(passiveActiveAbilityTypeExeption[i]);
        }

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);
            randomAbilities.Add(avaliableAbilities[randomIndex]);
            avaliableAbilities.RemoveAt(randomIndex);
        }
        return randomAbilities;
    }

    public RegularAbilityVisualData GetActiveAbilityVisualData(RegularActiveAbilityType activeAbilityType)
    {
        return allRegularActiveAbilitiesDic[activeAbilityType].GetAbilityVisualData;
    }

    public PassiveAbilityVisualData GetPassiveAbilityVisualData(PassiveAbilityType passiveAbilityType)
    {
        return allPassiveAbilitiesDic[passiveAbilityType].GetAbilityVisualData;
    }

    public void UseCharacterAbility(int slotIndex)
    {
        TimedAbilityController timedAbilityController = timedAbilitySlots[slotIndex];
        if (timedAbilityController.IsActive)
            return;
        timedAbilityController.ActivateAbility();
    }

    public void AddPassiveAbility(PassiveAbilityType passiveAbilityType)
    {
        bool isFirstLevel = false;
        if (eqquipedPassiveAbilities.ContainsKey(passiveAbilityType))
        {
            if (allPassiveAbilitiesDic[passiveAbilityType].IsMaxLevel(eqquipedPassiveAbilities[passiveAbilityType]))
                return;
            eqquipedPassiveAbilities[passiveAbilityType]++;
        }
        else
        {
            isFirstLevel = true;
            eqquipedPassiveAbilities.Add(passiveAbilityType, 1);
        }

        PassiveAbilityAction(passiveAbilityType, isFirstLevel);
    }

    public void PassiveAbilityAction(PassiveAbilityType passiveAbilityType, bool isFirstLevel)
    {
        switch (passiveAbilityType)
        {
            case PassiveAbilityType.FrostStrike:

                break;
            case PassiveAbilityType.FlameStrike:

                break;
            case PassiveAbilityType.LightningStrike:

                break;
            case PassiveAbilityType.LichArmor:

                break;
            case PassiveAbilityType.IfritsArmor:

                break;
            case PassiveAbilityType.HeartThief:
                characterStatController.ModifyLifeSteal(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("LifeSteal", isFirstLevel), true);
                break;
            case PassiveAbilityType.Reflect:

                break;
            case PassiveAbilityType.LuckyHit:
                characterStatController.ModifyCriticalHitChance(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("CriticalHitChance", isFirstLevel), true);
                break;
            case PassiveAbilityType.Flex:
                characterStatController.ModifyDefense(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Defense", isFirstLevel), true);
                break;
            case PassiveAbilityType.Multicast:

                break;
            case PassiveAbilityType.Sacrifice:

                break;
            case PassiveAbilityType.BoilingPoint:

                break;
            case PassiveAbilityType.GreedIsGood:

                break;
            case PassiveAbilityType.DuckDodgeDip:
                characterStatController.ModifyDodgeChance(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("DodgeChance", isFirstLevel), true);
                break;
            case PassiveAbilityType.FullCounter:

                break;
            default: break;
        }
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