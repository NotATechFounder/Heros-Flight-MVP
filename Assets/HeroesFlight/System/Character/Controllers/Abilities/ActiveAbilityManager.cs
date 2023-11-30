using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Character.Controllers.Effects;
using HeroesFlight.System.Combat.Effects.Effects;
using UnityEngine;

public class ActiveAbilityManager : MonoBehaviour
{
    public bool test;

    public event Action<int> OnSpChanged;
    public event Action<int, int, float> OnEXPAdded;

    public Action<int, ActiveAbilityVisualData> OnActiveAbilityEquipped;
    public Action<PassiveAbilityVisualData> OnPassiveAbilityEquipped;
    public Action<PassiveAbilityType> OnPassiveAbilityRemoved;
    public Action<ActiveAbilityType, ActiveAbilityType> OnRegularActiveAbilitySwapped;
    public Action<ActiveAbilityType, int> OnRegularActiveAbilityUpgraded;

    [SerializeField] private CustomAnimationCurve levelCurve;
    [SerializeField] LevelSystem levelSystem;
    [SerializeField] private ActiveAbilityDatabase allActiveAbilities;
    [SerializeField] private PassiveAbilityDatabase allPassiveAbilities;

    public TimedAbilityController ActiveAbilityOneController => activeAbilityOneController;
    public TimedAbilityController ActiveAbilityTwoController => activeAbilityTwoController;
    public TimedAbilityController ActiveAbilityThreeController => activerAbilityThreeController;

    public List<TimedAbilityController> TimedAbilityControllers => timedAbilitySlots;

    // Regular Active Ability
    private TimedAbilityController activeAbilityOneController = new TimedAbilityController();
    private TimedAbilityController activeAbilityTwoController = new TimedAbilityController();
    private TimedAbilityController activerAbilityThreeController = new TimedAbilityController();

    private List<ActiveAbilityType> activeAbilityTypes = new List<ActiveAbilityType>();
    private List<TimedAbilityController> timedAbilitySlots = new List<TimedAbilityController>();

    private Dictionary<ActiveAbilityType, ActiveAbilitySO> allActiveAbilitiesDic =
        new Dictionary<ActiveAbilityType, ActiveAbilitySO>();

    private Dictionary<ActiveAbilityType, TimedAbilityController> activeAbiltyAndControllerDic =
        new Dictionary<ActiveAbilityType, TimedAbilityController>();

    private Dictionary<ActiveAbilityType, RegularActiveAbility> eqquipedActiveActivities =
        new Dictionary<ActiveAbilityType, RegularActiveAbility>();

    // Passive Ability
    private List<PassiveAbilityType> passiveActiveAbilityTypes = new List<PassiveAbilityType>();

    private Dictionary<PassiveAbilityType, PassiveAbilitySO> allPassiveAbilitiesDic =
        new Dictionary<PassiveAbilityType, PassiveAbilitySO>();

    private Dictionary<PassiveAbilityType, int> eqquipedPassiveAbilities = new Dictionary<PassiveAbilityType, int>();

    private CharacterStatController characterStatController;
    private CharacterSimpleController characterSystem;
    private HealthController characterHealthController;
    private BaseCharacterAttackController characterAttackController;
    private CombatEffectsController characterEffectsController;


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

            EquippedAbility(ActiveAbilityType.EnergyBlast);
            EquippedAbility(ActiveAbilityType.IlluminatedArrows);
            EquippedAbility(ActiveAbilityType.HeavenHammer);
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
        characterEffectsController = this.characterStatController.GetComponent<CharacterCombatEffectsController>();

        levelSystem = new LevelSystem(0, 0, levelCurve);
        levelSystem.OnLevelUp += (response) =>
        {
            OnEXPAdded.Invoke(response.currentLevel, response.numberOfLevelsGained, response.normalizedExp);
        };
    }

    public void Cache()
    {
        for (int i = 0; i < allActiveAbilities.Items.Length; i++)
        {
            activeAbilityTypes.Add(allActiveAbilities.Items[i].GetAbilityVisualData.ActiveAbilityType);
            allActiveAbilitiesDic.Add(allActiveAbilities.Items[i].GetAbilityVisualData.ActiveAbilityType,
                allActiveAbilities.Items[i]);
        }

        timedAbilitySlots.Add(activeAbilityOneController);
        timedAbilitySlots.Add(activeAbilityTwoController);
        timedAbilitySlots.Add(activerAbilityThreeController);

        for (int i = 0; i < allPassiveAbilities.Items.Length; i++)
        {
            passiveActiveAbilityTypes.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType);
            allPassiveAbilitiesDic.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType,
                allPassiveAbilities.Items[i]);
        }
    }

    public void EquippedAbility(ActiveAbilityType passiveActiveAbilityType)
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
                InitialiseAbility(passiveActiveAbilityType, timedAbilityController);
                return;
            }
        }

        Debug.LogError("No more ability slots");
    }

    public void InitialiseAbility(ActiveAbilityType passiveActiveAbilityType,
        TimedAbilityController timedAbilityController, int level = 1)
    {
        RegularActiveAbility passiveActiveAbility = allActiveAbilitiesDic[passiveActiveAbilityType]
            .GetAbility(characterStatController.transform.position);

        AttachAbility(timedAbilityController, passiveActiveAbility);

        switch (passiveActiveAbilityType)
        {
            case ActiveAbilityType.HeavenStab:
                (passiveActiveAbility as HeavenStab).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage),
                    characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.OrbOfLightning:
                (passiveActiveAbility as OrbOfLightning).Initialize(level, characterStatController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.MagicShield:
                (passiveActiveAbility as MagicShield).Initialize(level, characterHealthController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.KnifeFluffy:
                (passiveActiveAbility as KnifeFluffy).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage));
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.Immolation:
                (passiveActiveAbility as Immolation).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage));
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.LightNova:
                (passiveActiveAbility as LightNova).Initialize(level, characterStatController, characterSystem,
                    characterHealthController, characterAttackController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.SwordWhirlwind:
                (passiveActiveAbility as SwordWhirlwind).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage));
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.LightningArrow:
                (passiveActiveAbility as LightningArrow).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage),
                    characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.ChainRotate:
                (passiveActiveAbility as ChainRotate).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage));
                break;
            case ActiveAbilityType.KageBunshin:
                (passiveActiveAbility as KageBunshin).Initialize(level,
                    (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage));
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.EnergyBlast:
                    (passiveActiveAbility as EnergyBlast).Initialize(level, (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage), characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.HeavenHammer:
                (passiveActiveAbility as HeavenHammer).Initialize(level, (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage), characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case ActiveAbilityType.IlluminatedArrows:
                (passiveActiveAbility as IlluminatedArrows).Initialize(level, (int)characterStatController.GetStatModel.GetCurrentStatValue(StatType.MagicDamage), characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            default:  break;
        }

        int index = timedAbilitySlots.IndexOf(timedAbilityController);
        OnActiveAbilityEquipped?.Invoke(index, GetActiveAbilityVisualData(passiveActiveAbilityType));
    }

    public void AttachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility activeAbility)
    {
        timedAbilityController.Init(this, activeAbility.ActiveAbilitySO.Duration,
            activeAbility.ActiveAbilitySO.Cooldown);
        timedAbilityController.OnActivated += activeAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted += activeAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded += activeAbility.OnCoolDownEnded;

        eqquipedActiveActivities.CreateOrAdd(activeAbility.PassiveActiveAbilityType, activeAbility);
        activeAbiltyAndControllerDic.CreateOrAdd(activeAbility.PassiveActiveAbilityType, timedAbilityController);
    }

    public void DetachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility passiveActiveAbility)
    {
        timedAbilityController.OnActivated -= passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted -= passiveActiveAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded -= passiveActiveAbility.OnCoolDownEnded;
        timedAbilityController.Refresh();

        eqquipedActiveActivities.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        activeAbiltyAndControllerDic.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        Destroy(passiveActiveAbility.gameObject);
    }

    public void SwapActiveAbility(ActiveAbilityType currentAbility, ActiveAbilityType newAbility)
    {
        int levelOfCurrentAbility = eqquipedActiveActivities[currentAbility].Level;
        TimedAbilityController timedAbilityController = activeAbiltyAndControllerDic[currentAbility];
        DetachAbility(activeAbiltyAndControllerDic[currentAbility], eqquipedActiveActivities[currentAbility]);
        InitialiseAbility (newAbility, timedAbilityController, levelOfCurrentAbility);
        OnRegularActiveAbilitySwapped?.Invoke(currentAbility, newAbility);
    }

    public void UpgradeAbility(ActiveAbilityType passiveActiveAbilityType)
    {
        eqquipedActiveActivities[passiveActiveAbilityType].LevelUp();
        OnRegularActiveAbilityUpgraded?.Invoke(passiveActiveAbilityType, eqquipedActiveActivities[passiveActiveAbilityType].Level);
    }

    bool AbilityAlreadyEquipped(ActiveAbilityType passiveActiveAbilityType)
    {
        return eqquipedActiveActivities.ContainsKey(passiveActiveAbilityType);
    }

    public List<ActiveAbilityType> GetRandomActiveAbility(int amount,
        List<ActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<ActiveAbilityType> randomAbilities = new List<ActiveAbilityType>();

        if (eqquipedActiveActivities.Count >= 3)
        {
            randomAbilities = GetRandomActiveAbilityFromEqquiped(amount, passiveActiveAbilityTypeExeption);
        }
        else
        {
            randomAbilities = GetRandomActiveAbilityFromAll(amount, passiveActiveAbilityTypeExeption);
        }

        return randomAbilities;
    }

    public List<ActiveAbilityType> GetRandomActiveAbilityFromEqquiped(int amount,
        List<ActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<ActiveAbilityType> randomAbilities = new List<ActiveAbilityType>();
        List<ActiveAbilityType> avaliableAbilities = eqquipedActiveActivities.Keys.ToList();

        int differenceInAmount = passiveActiveAbilityTypeExeption.Count - eqquipedActiveActivities.Count;
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

    public List<ActiveAbilityType> GetRandomActiveAbilityFromAll(int amount,
        List<ActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<ActiveAbilityType> randomAbilities = new List<ActiveAbilityType>();
        List<ActiveAbilityType> avaliableAbilities = new List<ActiveAbilityType>();


        foreach (ActiveAbilityType activeAbilityType in activeAbilityTypes)
        {
            ActiveAbilitySO activeAbilitySO = allActiveAbilitiesDic[activeAbilityType];
            if (activeAbilitySO.GetAbilityVisualData.ActiveType == ActiveType.Special)
            {
                int randomChance = UnityEngine.Random.Range(0, 100);
                if (randomChance <= 15)
                {
                    avaliableAbilities.Add(activeAbilityType);
                }
                continue;
            }
            avaliableAbilities.Add(activeAbilityType);
        }


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

    public List<PassiveAbilityType> GetRandomPassiveAbility(int amount,
        List<PassiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<PassiveAbilityType> randomAbilities = new List<PassiveAbilityType>();

        if (eqquipedActiveActivities.Count >= 3)
        {
            randomAbilities = GetRandomPassiveAbilityForEqquiped(amount, passiveActiveAbilityTypeExeption);
        }
        else
        {
            randomAbilities = GetRandomPassiveAbilityFromAll(amount, passiveActiveAbilityTypeExeption);
        }

        return randomAbilities;
    }

    public List<PassiveAbilityType> GetRandomPassiveAbilityFromAll(int amount,
        List<PassiveAbilityType> passiveActiveAbilityTypeExeption)
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

    public List<PassiveAbilityType> GetRandomPassiveAbilityForEqquiped(int amount,
        List<PassiveAbilityType> passiveAbilityTypeExeption)
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

    public ActiveAbilityVisualData GetActiveAbilityVisualData(ActiveAbilityType activeAbilityType)
    {
        return allActiveAbilitiesDic[activeAbilityType].GetAbilityVisualData;
    }

    public PassiveAbilityVisualData GetPassiveAbilityVisualData(PassiveAbilityType passiveAbilityType)
    {
        return allPassiveAbilitiesDic[passiveAbilityType].GetAbilityVisualData;
    }

    public int GetActiveAbilityLevel(ActiveAbilityType regularActiveAbilityType)
    {
        if (!eqquipedActiveActivities.ContainsKey(regularActiveAbilityType))
            return 0;
        return eqquipedActiveActivities[regularActiveAbilityType].Level;
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
        if (timedAbilityController.ActivateAbility())
        {
            ActiveAbilityType regularActiveAbilityType = activeAbiltyAndControllerDic.FirstOrDefault(x => x.Value == timedAbilityController).Key;
            RegularActiveAbility regularActiveAbility = eqquipedActiveActivities[regularActiveAbilityType];
            if (regularActiveAbility.IsInstant())
            {
                bool canMulticast = UnityEngine.Random.Range(0.0f, 100.0f) <=
                                    characterStatController.GetStatModel.GetCurrentStatValue(StatType.MulticastChance);
                if (canMulticast)
                {
                    regularActiveAbility.StartCoroutine(regularActiveAbility.MultiCast());
                }
            }

            regularActiveAbility.transform.position = characterStatController.transform.position;
        }
    }

    public List<ActiveAbilityType> GetEqqipedActiveAbilities()
    {
        return new List<ActiveAbilityType>(eqquipedActiveActivities.Keys);
    }

    public void AddPassiveAbility(PassiveAbilityType passiveAbilityType)
    {
        EquipPassiveAbility(passiveAbilityType);
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
        int levelOfCurrentAbility = eqquipedPassiveAbilities[currentAbility];
        RemovePassiveAbility(currentAbility);
        EquipPassiveAbility(newAbility, levelOfCurrentAbility);
    }

    public void PassiveAbilityAction(PassiveAbilityType passiveAbilityType, bool isFirstLevel)
    {
        characterEffectsController.AddCombatEffect(
            allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
            eqquipedPassiveAbilities[passiveAbilityType] - 1);
    }

    public void RemovePassiveAbility(PassiveAbilityType passiveAbilityType)
    {
        characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType]
            .GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));

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
        levelSystem.AddExp(exp);
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