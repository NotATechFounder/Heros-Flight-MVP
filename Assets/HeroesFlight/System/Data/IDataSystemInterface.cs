using HeroesFlight.System;
using System;

using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Rewards;


public interface DataSystemInterface : SystemInterface
{
    public CharacterManager CharacterManager { get;}

    public CurrencyManager CurrencyManager { get; }

    public StatManager StatManager { get; }

    public StatPoints StatPoints { get; }

    public AccountLevelManager AccountLevelManager { get; }

    RewardsHandlerInterface RewardHandler { get; }

    //TraitSystemInterface TraitSystem { get; }
}
