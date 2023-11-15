using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveAbilityManager : MonoBehaviour
{
    public bool test;

    public event Action<int> OnSpChanged;
    public event Action<int, int, float> OnEXPAdded;

    [SerializeField] private int spPerLevel;
    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;

    public Action<int, RegularAbilityVisualData> OnActiveAbilityEquipped;
    public Action<PassiveAbilityVisualData> OnPassiveAbilityEquipped;
    public Action<PassiveAbilityType> OnPassiveAbilityRemoved;

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

    [SerializeField] private int currentLevel;
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel;


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
            AddPassiveAbility(PassiveAbilityType.Flex);
            AddPassiveAbility(PassiveAbilityType.DuckDodgeDip);
            AddPassiveAbility(PassiveAbilityType.LuckyHit);

            EquippedAbility (RegularActiveAbilityType.HeavenStab);
            EquippedAbility (RegularActiveAbilityType.SwordWhirlwind);
            EquippedAbility (RegularActiveAbilityType.LightNova);

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
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        this.characterSystem = characterStatController.GetComponent<CharacterSimpleController>();
        this.characterHealthController = characterStatController.GetComponent<HealthController>();
        this.characterAttackController = characterStatController.GetComponent<BaseCharacterAttackController>();
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
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

    public void SwapActiveAbility(RegularActiveAbilityType currentAbility, RegularActiveAbilityType newAbility)
    {
        int levelOfCurrentAbility = eqquipedRegularActivities[currentAbility].Level;
        DetachAbility(regularAbiltyAndControllerDic[currentAbility], eqquipedRegularActivities[currentAbility]);
        InitialiseAbility (newAbility, regularAbiltyAndControllerDic[currentAbility], levelOfCurrentAbility);
    }

    public void UpgradeAbility(RegularActiveAbilityType passiveActiveAbilityType)
    {
        eqquipedRegularActivities[passiveActiveAbilityType].LevelUp();
    }

    bool AbilityAlreadyEquipped(RegularActiveAbilityType passiveActiveAbilityType)
    {
        return eqquipedRegularActivities.ContainsKey(passiveActiveAbilityType);
    }

    public List<RegularActiveAbilityType> GetRandomActiveAbility(int amount , List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<RegularActiveAbilityType> randomAbilities = new List<RegularActiveAbilityType>();

        if (eqquipedRegularActivities.Count >= 3)
        {
            randomAbilities = GetRandomActiveAbilityFromEqquiped(amount, passiveActiveAbilityTypeExeption);
        }
        else
        {
            randomAbilities = GetRandomActiveAbilityFromAll(amount, passiveActiveAbilityTypeExeption);
        }

        return randomAbilities;
    }

    public List<RegularActiveAbilityType> GetRandomActiveAbilityFromEqquiped(int amount , List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<RegularActiveAbilityType> randomAbilities = new List<RegularActiveAbilityType>();
        List<RegularActiveAbilityType> avaliableAbilities = eqquipedRegularActivities.Keys.ToList();

        int differenceInAmount = passiveActiveAbilityTypeExeption.Count - eqquipedRegularActivities.Count;
        for (int i = 0; i < differenceInAmount; i++)
        {
            avaliableAbilities.Remove(passiveActiveAbilityTypeExeption[i]);
        }

        for (int i = 0; i < amount; i++)
        {
            if (avaliableAbilities.Count == 0)
                break;
            int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);
            randomAbilities.Add(avaliableAbilities[randomIndex]);
            avaliableAbilities.RemoveAt(randomIndex);
        }
        return randomAbilities;
    }

    public List<RegularActiveAbilityType> GetRandomActiveAbilityFromAll(int amount , List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<RegularActiveAbilityType> randomAbilities = new List<RegularActiveAbilityType>();
        List<RegularActiveAbilityType> avaliableAbilities = new List<RegularActiveAbilityType>(regularActiveAbilityTypes);

        for (int i = 0; i < passiveActiveAbilityTypeExeption.Count; i++)
        {
            avaliableAbilities.Remove(passiveActiveAbilityTypeExeption[i]);
        }

        for (int i = 0; i < amount; i++)
        {
            if (avaliableAbilities.Count == 0)
                break;
            int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);
            randomAbilities.Add(avaliableAbilities[randomIndex]);
            avaliableAbilities.RemoveAt(randomIndex);
        }
        return randomAbilities;
    }

    public List<PassiveAbilityType> GetRandomPassiveAbility(int amount , List<PassiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<PassiveAbilityType> randomAbilities = new List<PassiveAbilityType>();

        if(eqquipedRegularActivities.Count >=3)
        {
            randomAbilities = GetRandomPassiveAbilityForEqquiped(amount, passiveActiveAbilityTypeExeption);
        }
        else
        {
            randomAbilities = GetRandomPassiveAbilityFromAll(amount, passiveActiveAbilityTypeExeption);
        }

        return randomAbilities;
    }

    public List<PassiveAbilityType> GetRandomPassiveAbilityFromAll(int amount, List<PassiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<PassiveAbilityType> randomAbilities = new List<PassiveAbilityType>();
        List<PassiveAbilityType> avaliableAbilities = new List<PassiveAbilityType>(passiveActiveAbilityTypes);

        for (int i = 0; i < passiveActiveAbilityTypeExeption.Count; i++)
        {
            avaliableAbilities.Remove(passiveActiveAbilityTypeExeption[i]);
        }

        for (int i = 0; i < amount; i++)
        {
            if (avaliableAbilities.Count == 0)
                break;

            int randomIndex = UnityEngine.Random.Range(0, avaliableAbilities.Count);

            randomAbilities.Add(avaliableAbilities[randomIndex]);
            avaliableAbilities.RemoveAt(randomIndex);
        }
        return randomAbilities;
    }

    public List<PassiveAbilityType> GetRandomPassiveAbilityForEqquiped(int amount, List<PassiveAbilityType> passiveAbilityTypeExeption)
    {
        List<PassiveAbilityType> randomAbilities = new List<PassiveAbilityType>();
        List<PassiveAbilityType> avaliableAbilities = eqquipedPassiveAbilities.Keys.ToList();

        int differenceInAmount = passiveAbilityTypeExeption.Count - eqquipedPassiveAbilities.Count;
        for (int i = 0; i < differenceInAmount; i++)
        {
            avaliableAbilities.Remove(passiveAbilityTypeExeption[i]);
        }

        for (int i = 0; i < amount; i++)
        {
            if (avaliableAbilities.Count == 0)
                break;

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

    public int GetActiveAbilityLevel(RegularActiveAbilityType regularActiveAbilityType)
    {
        if (!eqquipedRegularActivities.ContainsKey(regularActiveAbilityType))
            return 0;
        return eqquipedRegularActivities[regularActiveAbilityType].Level;
    }

    public int GetPassiveAbilityLevel(PassiveAbilityType passiveAbilityType)
    {
        if (!eqquipedPassiveAbilities.ContainsKey(passiveAbilityType))
            return 0;
        return eqquipedPassiveAbilities[passiveAbilityType];
    }

    public void UseCharacterAbility(int slotIndex)
    {
        TimedAbilityController timedAbilityController = timedAbilitySlots[slotIndex];
        if (!timedAbilityController.IsValid)
            return;
        timedAbilityController.ActivateAbility();
    }

    public List<RegularActiveAbilityType> GetEqqipedActiveAbilities()
    {
        return new List<RegularActiveAbilityType>(eqquipedRegularActivities.Keys);
    }

    public void AddPassiveAbility(PassiveAbilityType passiveAbilityType)
    {
        EquipPassiveAbility (passiveAbilityType);
    }

    public void EquipPassiveAbility(PassiveAbilityType passiveAbilityType, int level = 1) 
    {
        bool isFirstLevel = false;
        if (eqquipedPassiveAbilities.ContainsKey(passiveAbilityType))
        {
            if (allPassiveAbilitiesDic[passiveAbilityType].IsMaxLevel(eqquipedPassiveAbilities[passiveAbilityType]))
            {
                Debug.LogError("Max level reached");
                return;
            }
                
            eqquipedPassiveAbilities[passiveAbilityType]++;
        }
        else
        {
            if (eqquipedPassiveAbilities.Count >= 3)
            {
                return;
            }

            isFirstLevel = true;
            eqquipedPassiveAbilities.Add(passiveAbilityType, level);
        }

        PassiveAbilityAction(passiveAbilityType, isFirstLevel);

        OnPassiveAbilityEquipped?.Invoke(GetPassiveAbilityVisualData(passiveAbilityType));
    }

    public void SwapPassiveAbility(PassiveAbilityType currentAbility, PassiveAbilityType newAbility)
    {
        int levelOfCurrentAbility  = eqquipedPassiveAbilities[currentAbility];
        RemovePassiveAbility(currentAbility);
        EquipPassiveAbility(newAbility , levelOfCurrentAbility);
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
                characterStatController.ModifyLifeSteal(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("LifeSteal", isFirstLevel, eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.Reflect:

                break;
            case PassiveAbilityType.LuckyHit:
                characterStatController.ModifyCriticalHitChance(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("CriticalHitChance", isFirstLevel, eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.Flex:
                characterStatController.ModifyDefense(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Defense", isFirstLevel, eqquipedPassiveAbilities[passiveAbilityType]), true);
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
                characterStatController.ModifyDodgeChance(allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("DodgeChance", isFirstLevel, eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.FullCounter:

                break;
            default: break;
        }
    }

    public void RemovePassiveAbility(PassiveAbilityType passiveAbilityType)
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
                characterStatController.ModifyLifeSteal(allPassiveAbilitiesDic[passiveAbilityType].GetLevelValue("LifeSteal", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.Reflect:

                break;
            case PassiveAbilityType.LuckyHit:
                characterStatController.ModifyCriticalHitChance(allPassiveAbilitiesDic[passiveAbilityType].GetLevelValue("CriticalHitChance", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.Flex:
                characterStatController.ModifyDefense(allPassiveAbilitiesDic[passiveAbilityType].GetLevelValue("Defense", eqquipedPassiveAbilities[passiveAbilityType]), false);    
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
                characterStatController.ModifyDodgeChance(allPassiveAbilitiesDic[passiveAbilityType].GetLevelValue("DodgeChance", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.FullCounter:

                break;
            default: break;
        }

        if (eqquipedPassiveAbilities.ContainsKey(passiveAbilityType))
        {
            eqquipedPassiveAbilities.Remove(passiveAbilityType);
            OnPassiveAbilityRemoved?.Invoke(passiveAbilityType);
        }
    }

    public List<PassiveAbilityType> GetEqquipedPassiveAbilities()
    {
        return new List<PassiveAbilityType>(eqquipedPassiveAbilities.Keys);
    }

    public void AddExp(float exp)
    {
        currentExp += exp;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        else
        {
            OnEXPAdded?.Invoke(0, 0, currentExp / expToNextLevel);
        }
    }

    private void LevelUpOnce()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
    }

    private void LevelUp()
    {
        int currentLvl = currentLevel;
        int numberOfLevelsGained = 0;
        do
        {
            LevelUpOnce();
            ++numberOfLevelsGained;
        } while (currentExp >= expToNextLevel);

        OnEXPAdded?.Invoke(currentLvl, numberOfLevelsGained, currentExp / expToNextLevel);
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