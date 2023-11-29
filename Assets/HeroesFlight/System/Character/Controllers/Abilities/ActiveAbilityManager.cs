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

    public Action<int, RegularAbilityVisualData> OnActiveAbilityEquipped;
    public Action<PassiveAbilityVisualData> OnPassiveAbilityEquipped;
    public Action<PassiveAbilityType> OnPassiveAbilityRemoved;
    public Action<RegularActiveAbilityType, RegularActiveAbilityType> OnRegularActiveAbilitySwapped;
    public Action<RegularActiveAbilityType, int> OnRegularActiveAbilityUpgraded;

    [SerializeField] private CustomAnimationCurve levelCurve;
    [SerializeField] LevelSystem levelSystem;
    [SerializeField] private RegularActiveAbilityDatabase allActiveAbilities;
    [SerializeField] private PassiveAbilityDatabase allPassiveAbilities;

    public TimedAbilityController RegularAbilityOneController => regularAbilityOneController;
    public TimedAbilityController RegularAbilityTwoController => regularAbilityTwoController;
    public TimedAbilityController RegularAbilityThreeController => regularAbilityThreeController;

    public List<TimedAbilityController> TimedAbilityControllers => timedAbilitySlots;

    // Regular Active Ability
    private TimedAbilityController regularAbilityOneController = new TimedAbilityController();
    private TimedAbilityController regularAbilityTwoController = new TimedAbilityController();
    private TimedAbilityController regularAbilityThreeController = new TimedAbilityController();

    private List<RegularActiveAbilityType> regularActiveAbilityTypes = new List<RegularActiveAbilityType>();
    private List<TimedAbilityController> timedAbilitySlots = new List<TimedAbilityController>();

    private Dictionary<RegularActiveAbilityType, RegularActiveAbilitySO> allRegularActiveAbilitiesDic =
        new Dictionary<RegularActiveAbilityType, RegularActiveAbilitySO>();

    private Dictionary<RegularActiveAbilityType, TimedAbilityController> regularAbiltyAndControllerDic =
        new Dictionary<RegularActiveAbilityType, TimedAbilityController>();

    private Dictionary<RegularActiveAbilityType, RegularActiveAbility> eqquipedRegularActivities =
        new Dictionary<RegularActiveAbilityType, RegularActiveAbility>();

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

    private float chanceToMulticast = 0;


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

            EquippedAbility(RegularActiveAbilityType.HeavenStab);
            EquippedAbility(RegularActiveAbilityType.KageBunshin);
            EquippedAbility(RegularActiveAbilityType.ChainRotate);
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

        levelSystem = new LevelSystem(0,0, levelCurve);
        levelSystem.OnLevelUp += (response) =>
        {
            OnEXPAdded.Invoke(response.currentLevel,response.numberOfLevelsGained, response.normalizedExp);
        };
    }

    public void Cache()
    {
        for (int i = 0; i < allActiveAbilities.Items.Length; i++)
        {
            regularActiveAbilityTypes.Add(allActiveAbilities.Items[i].GetAbilityVisualData.RegularActiveAbilityType);
            allRegularActiveAbilitiesDic.Add(allActiveAbilities.Items[i].GetAbilityVisualData.RegularActiveAbilityType,
                allActiveAbilities.Items[i]);
        }

        timedAbilitySlots.Add(regularAbilityOneController);
        timedAbilitySlots.Add(regularAbilityTwoController);
        timedAbilitySlots.Add(regularAbilityThreeController);

        for (int i = 0; i < allPassiveAbilities.Items.Length; i++)
        {
            passiveActiveAbilityTypes.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType);
            allPassiveAbilitiesDic.Add(allPassiveAbilities.Items[i].GetAbilityVisualData.PassiveActiveAbilityType,
                allPassiveAbilities.Items[i]);
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
                InitialiseAbility(passiveActiveAbilityType, timedAbilityController);
                return;
            }
        }

        Debug.LogError("No more ability slots");
    }

    public void InitialiseAbility(RegularActiveAbilityType passiveActiveAbilityType, TimedAbilityController timedAbilityController, int level = 1)
    {
        RegularActiveAbility passiveActiveAbility = allRegularActiveAbilitiesDic[passiveActiveAbilityType].GetAbility(characterStatController.transform.position);

        AttachAbility(timedAbilityController, passiveActiveAbility);

        switch (passiveActiveAbilityType)
        {
            case RegularActiveAbilityType.HeavenStab:
                (passiveActiveAbility as HeavenStab).Initialize(level,  (int)characterStatController.CurrentPhysicalDamage, characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.OrbOfLightning:
                (passiveActiveAbility as OrbOfLightning).Initialize(level, characterStatController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.MagicShield:
                (passiveActiveAbility as MagicShield).Initialize(level, characterHealthController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.KnifeFluffy:
                (passiveActiveAbility as KnifeFluffy).Initialize(level,  (int)characterStatController.CurrentPhysicalDamage);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.Immolation:
                (passiveActiveAbility as Immolation).Initialize(level, (int)characterStatController.CurrentMagicDamage);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.LightNova:
                (passiveActiveAbility as LightNova).Initialize(level, characterStatController, characterSystem,  characterHealthController, characterAttackController);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.SwordWhirlwind:
                (passiveActiveAbility as SwordWhirlwind).Initialize(level, (int)characterStatController.CurrentPhysicalDamage);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.LightningArrow:
                (passiveActiveAbility as LightningArrow).Initialize(level, (int)characterStatController.CurrentMagicDamage, characterSystem);
                passiveActiveAbility.transform.SetParent(characterStatController.transform);
                break;
            case RegularActiveAbilityType.ChainRotate:
                (passiveActiveAbility as ChainRotate).Initialize(level, (int)characterStatController.CurrentPhysicalDamage);
                break;
            default:  break;
        }

        int index = timedAbilitySlots.IndexOf(timedAbilityController);
        OnActiveAbilityEquipped?.Invoke(index, GetActiveAbilityVisualData(passiveActiveAbilityType));
    }

    public void AttachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility activeAbility)
    {
        timedAbilityController.Init(this, activeAbility.ActiveAbilitySO.Duration, activeAbility.ActiveAbilitySO.Cooldown);
        timedAbilityController.OnActivated += activeAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted += activeAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded += activeAbility.OnCoolDownEnded;

        eqquipedRegularActivities.CreateOrAdd(activeAbility.PassiveActiveAbilityType, activeAbility);
        regularAbiltyAndControllerDic.CreateOrAdd(activeAbility.PassiveActiveAbilityType, timedAbilityController);
    }

    public void DetachAbility(TimedAbilityController timedAbilityController, RegularActiveAbility passiveActiveAbility)
    {
        timedAbilityController.OnActivated -= passiveActiveAbility.OnActivated;
        timedAbilityController.OnCoolDownStarted -= passiveActiveAbility.OnDeactivated;
        timedAbilityController.OnCoolDownEnded -= passiveActiveAbility.OnCoolDownEnded;
        timedAbilityController.Refresh();

        eqquipedRegularActivities.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        regularAbiltyAndControllerDic.Remove(passiveActiveAbility.PassiveActiveAbilityType);
        Destroy(passiveActiveAbility.gameObject);
    }

    public void SwapActiveAbility(RegularActiveAbilityType currentAbility, RegularActiveAbilityType newAbility)
    {
        int levelOfCurrentAbility = eqquipedRegularActivities[currentAbility].Level;
        TimedAbilityController timedAbilityController = regularAbiltyAndControllerDic[currentAbility];
        DetachAbility(regularAbiltyAndControllerDic[currentAbility], eqquipedRegularActivities[currentAbility]);
        InitialiseAbility (newAbility, timedAbilityController, levelOfCurrentAbility);
        OnRegularActiveAbilitySwapped?.Invoke(currentAbility, newAbility);
    }

    public void UpgradeAbility(RegularActiveAbilityType passiveActiveAbilityType)
    {
        eqquipedRegularActivities[passiveActiveAbilityType].LevelUp();
        OnRegularActiveAbilityUpgraded?.Invoke(passiveActiveAbilityType, eqquipedRegularActivities[passiveActiveAbilityType].Level);
    }

    bool AbilityAlreadyEquipped(RegularActiveAbilityType passiveActiveAbilityType)
    {
        return eqquipedRegularActivities.ContainsKey(passiveActiveAbilityType);
    }

    public List<RegularActiveAbilityType> GetRandomActiveAbility(int amount,
        List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
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

    public List<RegularActiveAbilityType> GetRandomActiveAbilityFromEqquiped(int amount,
        List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
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

    public List<RegularActiveAbilityType> GetRandomActiveAbilityFromAll(int amount,
        List<RegularActiveAbilityType> passiveActiveAbilityTypeExeption)
    {
        List<RegularActiveAbilityType> randomAbilities = new List<RegularActiveAbilityType>();
        List<RegularActiveAbilityType> avaliableAbilities =
            new List<RegularActiveAbilityType>(regularActiveAbilityTypes);

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

        if (eqquipedRegularActivities.Count >= 3)
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
        if (timedAbilityController.ActivateAbility())
        {
            RegularActiveAbilityType regularActiveAbilityType = regularAbiltyAndControllerDic.FirstOrDefault(x => x.Value == timedAbilityController).Key;
            RegularActiveAbility regularActiveAbility = eqquipedRegularActivities[regularActiveAbilityType];
            if (regularActiveAbility.IsInstant())
            {
                bool canMulticast = UnityEngine.Random.Range(0.0f, 100.0f) <= chanceToMulticast;
                if (canMulticast)
                {
                    regularActiveAbility.StartCoroutine(regularActiveAbility.MultiCast());
                }
            }

            regularActiveAbility.transform.position = characterStatController.transform.position;
        }
    }

    public List<RegularActiveAbilityType> GetEqqipedActiveAbilities()
    {
        return new List<RegularActiveAbilityType>(eqquipedRegularActivities.Keys);
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
        switch (passiveAbilityType)
        {
            case PassiveAbilityType.FrostStrike:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);
                break;
            case PassiveAbilityType.FlameStrike:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);

                break;
            case PassiveAbilityType.LightningStrike:

                break;
            case PassiveAbilityType.LichArmor:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);
                break;
            case PassiveAbilityType.IfritsArmor:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);

                break;
            case PassiveAbilityType.HeartThief:
                characterStatController.ModifyLifeSteal(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("LifeSteal", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.Reflect:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);

                break;
            case PassiveAbilityType.LuckyHit:
                characterStatController.ModifyCriticalHitChance(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("CriticalHitChance", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.Flex:
                characterStatController.ModifyDefense(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Defense", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.Multicast:

                chanceToMulticast +=
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Chance", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]);

                break;
            case PassiveAbilityType.Sacrifice:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);

                break;
            case PassiveAbilityType.BoilingPoint:

                break;
            case PassiveAbilityType.GreedIsGood:
                characterStatController.ModifyGoldBoost(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Increase ", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                characterStatController.ModifyExperienceBoost(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("Increase ", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.DuckDodgeDip:
                characterStatController.ModifyDodgeChance(
                    allPassiveAbilitiesDic[passiveAbilityType].GetValueIncrease("DodgeChance", isFirstLevel,
                        eqquipedPassiveAbilities[passiveAbilityType]), true);
                break;
            case PassiveAbilityType.FullCounter:
                characterEffectsController.AddCombatEffect(
                    allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(0),
                    eqquipedPassiveAbilities[passiveAbilityType] - 1);

                break;
            default: break;
        }
    }

    public void RemovePassiveAbility(PassiveAbilityType passiveAbilityType)
    {
        switch (passiveAbilityType)
        {
            case PassiveAbilityType.FrostStrike:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.FlameStrike:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.LightningStrike:

                break;
            case PassiveAbilityType.LichArmor:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.IfritsArmor:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.HeartThief:
                characterStatController.ModifyLifeSteal(
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("LifeSteal", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.Reflect:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.LuckyHit:
                characterStatController.ModifyCriticalHitChance(
                    allPassiveAbilitiesDic[passiveAbilityType].GetLevelValue("CriticalHitChance",
                        eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.Flex:
                characterStatController.ModifyDefense(
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("Defense", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.Multicast:

                chanceToMulticast -=
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("Chance", eqquipedPassiveAbilities[passiveAbilityType]);

                break;
            case PassiveAbilityType.Sacrifice:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
                break;
            case PassiveAbilityType.BoilingPoint:

                break;
            case PassiveAbilityType.GreedIsGood:
                characterStatController.ModifyGoldBoost(
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("Increase ", eqquipedPassiveAbilities[passiveAbilityType]), false);
                characterStatController.ModifyExperienceBoost(
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("Increase ", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.DuckDodgeDip:
                characterStatController.ModifyDodgeChance(
                    allPassiveAbilitiesDic[passiveAbilityType]
                        .GetLevelValue("DodgeChance", eqquipedPassiveAbilities[passiveAbilityType]), false);
                break;
            case PassiveAbilityType.FullCounter:
                characterEffectsController.RemoveEffect(allPassiveAbilitiesDic[passiveAbilityType].GetCombatEffectByLvl(eqquipedPassiveAbilities[passiveAbilityType] - 1));
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