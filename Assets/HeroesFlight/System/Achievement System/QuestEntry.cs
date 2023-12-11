using HeroesFlight.Common.Enum;

public enum QuestType
{
    LevelComplection,
    UpgradeEquipment,
    ObtainEquipment,
    DefeatWorldBoss,
    DefeatMob,
    ReachLevel,
}

public class QuestEntry <T> where T : QuestBase
{
    T questType;

    public QuestEntry(T questType)
    {
        this.questType = questType;
    }

    public T GetQuestData()
    {
        return questType;
    }   
}

public class QuestBase
{
    public QuestType questType;
    public int amount;

    public QuestBase(int amount = 1)
    {
        this.amount = amount;
    }
}

public class ReachLevelQuest : QuestBase
{
    public int level;

    public ReachLevelQuest(int level) : base(level)
    {
        questType = QuestType.ReachLevel;
        this.level = level;
    }        
}


public class LevelComplectionQuest : QuestBase
{
    public WorldType worldType;

    public LevelComplectionQuest( WorldType worldType, int amount = 1) : base(amount)
    {
        questType = QuestType.LevelComplection;
        this.worldType = worldType;
    }
}

public class UpgradeEquipmentQuest : QuestBase
{
    public Rarity rarity;

    public UpgradeEquipmentQuest(Rarity rarity, int amount = 1) : base(amount)
    {
        questType = QuestType.UpgradeEquipment;
        this.rarity = rarity;
    }
}

public class ObtainEquipmentQuest : QuestBase
{
    public Rarity rarity;

    public ObtainEquipmentQuest(Rarity rarity, int amount = 1) : base(amount)
    {
        questType = QuestType.ObtainEquipment;
        this.rarity = rarity;
    }
}

public class DefeatWorldBossQuest : QuestBase
{
    public WorldType worldType;

    public DefeatWorldBossQuest(WorldType worldType, int amount = 1) : base(amount)
    {
        questType = QuestType.DefeatWorldBoss;
        this.worldType = worldType;
    }
}

public class DefeatMobsQuest : QuestBase
{
    public WorldType worldType;

    public DefeatMobsQuest(WorldType worldType, int amount = 1) : base(amount)
    {
        questType = QuestType.DefeatMob;
        this.worldType = worldType;
    }
}