using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Reward;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Achievement_System.ProgressionUnlocks;
using HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards;
using HeroesFlight.System.ShrineSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementSystem : IAchievementSystemInterface
{
    public QuestRewardHandler questRewardHandler { get; private set; }
    public ProgressionUnlocksHandler UnlocksHandlers { get; }

    private RewardSystemInterface rewardSystemInterface;
    private InventorySystemInterface inventorySystemInterface;
    private IUISystem uiSystem;
    private CombatSystemInterface combatSystemInterface;
    private EnvironmentSystemInterface environmentSystemInterface;
    private DataSystemInterface dataSystemInterface;
    private ShrineSystemInterface shrineSystem;

    public AchievementSystem(IUISystem uiSystem, RewardSystemInterface rewardSystemInterface,
        InventorySystemInterface inventorySystemInterface,
        CombatSystemInterface combatSystemInterface, EnvironmentSystemInterface environmentSystemInterface,
        DataSystemInterface dataSystemInterface, ShrineSystemInterface shrineSystem)
    {
        this.uiSystem = uiSystem;
        this.rewardSystemInterface = rewardSystemInterface;
        this.inventorySystemInterface = inventorySystemInterface;
        this.combatSystemInterface = combatSystemInterface;
        this.environmentSystemInterface = environmentSystemInterface;
        this.dataSystemInterface = dataSystemInterface;
        this.shrineSystem = shrineSystem;
        UnlocksHandlers = new ProgressionUnlocksHandler(dataSystemInterface);
        UnlocksHandlers.OnRewardUnlocked += HandleRewardUnlocked;
    }

    private void HandleRewardUnlocked(UnlockReward data)
    {
        switch (data.UnlockType)
        {
            case ProgressionUnlockType.Character:
                var characterData = data as CharacterUnlock;
                dataSystemInterface.CharacterManager.UnlockCharacter(characterData.CharacterCharacterType);
                Debug.Log($"{characterData.CharacterCharacterType} unlocked");
                break;
            case ProgressionUnlockType.NPC:
                var npcData = data as NPCUnlock;
                shrineSystem.UnlockNpc(npcData.NPCType);
                break;
            case ProgressionUnlockType.World:
                var worldData = data as WorldUnlock;
                dataSystemInterface.WorldManger.UnlockWorld(worldData.TargetWorld);
                Debug.Log($"Unlockign world {worldData.TargetWorld}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        questRewardHandler = scene.GetComponent<QuestRewardHandler>();

        questRewardHandler.Load();

        inventorySystemInterface.InventoryHandler.OnEquipmentAdded
            += (item) => AddQuestProgress(new QuestEntry<ObtainEquipmentQuest>(new ObtainEquipmentQuest(item.GetItemData<ItemEquipmentData>().rarity)));

        inventorySystemInterface.InventoryHandler.OnEquipmentUpdated
            += (item) => AddQuestProgress(new QuestEntry<UpgradeEquipmentQuest>(new UpgradeEquipmentQuest(item.GetItemData<ItemEquipmentData>().rarity)));

        combatSystemInterface.OnEntityDied += (deathModel) =>
        {
            switch (deathModel.EntityType)
            {
                case CombatEntityType.Player:
           
                    break;
                case CombatEntityType.Mob:
                    AddQuestProgress(new QuestEntry<DefeatMobsQuest>(new DefeatMobsQuest(dataSystemInterface.WorldManger.SelectedWorld)));
                    break;
                case CombatEntityType.MiniBoss:
                    AddQuestProgress(new QuestEntry<DefeatMobsQuest>(new DefeatMobsQuest(dataSystemInterface.WorldManger.SelectedWorld)));
                    break;
                case CombatEntityType.Boss:
                    AddQuestProgress(new QuestEntry<DefeatWorldBossQuest>(new DefeatWorldBossQuest(dataSystemInterface.WorldManger.SelectedWorld)));
                    AddProgressionProgress(new QuestEntry<DefeatWorldBossQuest>(new DefeatWorldBossQuest(dataSystemInterface.WorldManger.SelectedWorld)));
                    break;
                case CombatEntityType.TempMob:

                    break;
            }
        };

        // gamePlaySystemInterface.OnLevelComplected += () =>
        // {
        //     AddQuestProgress(new QuestEntry<LevelComplectionQuest>(new LevelComplectionQuest(dataSystemInterface.WorldManger.SelectedWorld)));
        // };

        // example
        //AddQuestProgress(new QuestEntry<DefeatMobsQuest>(new DefeatMobsQuest(WorldType.World1)));
        //AddQuestProgress(new QuestEntry<ReachLevelQuest>(new ReachLevelQuest(10)));
        //AddQuestProgress(new QuestEntry<DefeatWorldBossQuest>(new DefeatWorldBossQuest(WorldType.World1)));
        //AddQuestProgress(new QuestEntry<LevelComplectionQuest>(new LevelComplectionQuest(WorldType.World1)));
        //AddQuestProgress(new QuestEntry<ObtainEquipmentQuest>(new ObtainEquipmentQuest(Rarity.Common)));
        //AddQuestProgress(new QuestEntry<UpgradeEquipmentQuest>(new UpgradeEquipmentQuest(Rarity.Common)));
    }

    private void UpdateQuestVisual()
    {
        uiSystem.UiEventHandler.MainMenu.UpdateQuestInfo(
            new UISystem.QuestStatusRequest(questRewardHandler.CurrentQuest.GetQuestInfo(), questRewardHandler.CurrentData.qP, questRewardHandler.CurrentQuest.GetQuestGoal()));
    }

    public void Reset()
    {
 
    }

    public void AddQuestProgress<T>(QuestEntry<T> questEntry) where T : QuestBase
    {
        if (questRewardHandler.CurrentQuest == null || questRewardHandler.CurrentQuest.IsQuestCompleted(questRewardHandler.CurrentData.qP)
            || questRewardHandler.CurrentQuest.GetQuestType() != questEntry.GetQuestData().questType) return;

        switch (questEntry.GetQuestData().questType)
        {
            case QuestType.ReachLevel:
                questRewardHandler.CurrentData.qP = questEntry.GetQuestData().amount;
                break;
            case QuestType.DefeatMob:
                DefeatMobsQuest defeatMobs  = questEntry.GetQuestData() as DefeatMobsQuest;
                DefeatMobQuestSO defeatMobQuestSO = (DefeatMobQuestSO)questRewardHandler.CurrentQuest;
                if (defeatMobQuestSO.worldType == defeatMobs.worldType)
                {
                    questRewardHandler.CurrentData.qP += questEntry.GetQuestData().amount;
                }
                break;
            case QuestType.DefeatWorldBoss:
                DefeatWorldBossQuest defeatWorldBoss = questEntry.GetQuestData() as DefeatWorldBossQuest;
                DefeatWorldBossSO defeatWorldBossSO = (DefeatWorldBossSO)questRewardHandler.CurrentQuest;
                if (defeatWorldBossSO.worldType == defeatWorldBoss.worldType)
                {
                    questRewardHandler.CurrentData.qP += questEntry.GetQuestData().amount;
                }
                break;
            case QuestType.LevelCompletion:
                LevelComplectionQuest levelComplection = questEntry.GetQuestData() as LevelComplectionQuest;
                LevelComplectionQuestSO levelComplectionQuestSO = (LevelComplectionQuestSO)questRewardHandler.CurrentQuest;
                if (levelComplectionQuestSO.worldType == levelComplection.worldType)
                {
                    questRewardHandler.CurrentData.qP += questEntry.GetQuestData().amount;
                }
                break;
                case QuestType.ObtainEquipment:
                ObtainEquipmentQuest obtainEquipment = questEntry.GetQuestData() as ObtainEquipmentQuest;
                ObtainEquipmentQuestSO obtainEquipmentQuestSO = (ObtainEquipmentQuestSO)questRewardHandler.CurrentQuest;
                if (obtainEquipmentQuestSO.rarity == obtainEquipment.rarity)
                {
                    questRewardHandler.CurrentData.qP += questEntry.GetQuestData().amount;
                }
                break;
                case QuestType.UpgradeEquipment:
                UpgradeEquipmentQuest upgradeEquipment = questEntry.GetQuestData() as UpgradeEquipmentQuest;
                UpgradeEquipmentQuestSO upgradeEquipmentQuestSO = (UpgradeEquipmentQuestSO)questRewardHandler.CurrentQuest;
                if (upgradeEquipmentQuestSO.rarity == upgradeEquipment.rarity)
                {
                    questRewardHandler.CurrentData.qP += questEntry.GetQuestData().amount;
 
                }
                break;           
                default: break;
        }

        questRewardHandler.CurrentData.qP = Mathf.Clamp(questRewardHandler.CurrentData.qP, 0, questRewardHandler.CurrentQuest.GetQuestGoal());
        UpdateQuestVisual();
        questRewardHandler.Save();
    }

    public void AddProgressionProgress<T>(QuestEntry<T> questEntry) where T : QuestBase
    {
        switch (questEntry.GetQuestData().questType)
        {
            case QuestType.LevelCompletion:
                LevelComplectionQuest levelComplection = questEntry.GetQuestData() as LevelComplectionQuest;
                UnlocksHandlers.ProcessWorldProgression(levelComplection.worldType,levelComplection.amount);
                break;
            case QuestType.UpgradeEquipment:
                break;
            case QuestType.ObtainEquipment:
                break;
            case QuestType.DefeatWorldBoss:
                DefeatWorldBossQuest bossKill = questEntry.GetQuestData() as DefeatWorldBossQuest;
                UnlocksHandlers.ProcessBossKilled(bossKill.worldType,bossKill.worldType);
                break;
            case QuestType.DefeatMob:
                break;
            case QuestType.ReachLevel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ClaimQuestReward()
    {
        if (!questRewardHandler.CurrentQuest.IsQuestCompleted(questRewardHandler.CurrentData.qP))
        {
            Debug.Log("Quest not completed");
            return;
        }

        questRewardHandler.RewardClaimed();
        List<Reward> rewards = questRewardHandler.CurrentQuest.GetQuestRewardPack().GetReward();
        rewardSystemInterface.ProcessRewards(rewards);
        List<RewardVisual> rewardVisuals = rewardSystemInterface.GetRewardVisuals(rewards);
        uiSystem.UiEventHandler.RewardMenu.DisplayRewardsVisual (rewardVisuals.ToArray());
        UpdateQuestVisual();
    }

    public void InjectUiConnection()
    {
        uiSystem.UiEventHandler.MainMenu.OnQuestClaimButtonPressed += ClaimQuestReward;
        UpdateQuestVisual();
    }
}
