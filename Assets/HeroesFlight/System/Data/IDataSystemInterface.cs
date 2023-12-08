using HeroesFlight.System;
using System;

public interface DataSystemInterface : SystemInterface
{
    public CharacterManager CharacterManager { get;}

    public CurrencyManager CurrencyManager { get; }

    public StatManager StatManager { get; }

    public StatPoints StatPoints { get; }

    public AccountLevelManager AccountLevelManager { get; }

  
    public WorldManager WorldManger { get; }

    event Action OnApplicationQuit;
    void RequestDataSave();

    //TraitSystemInterface TraitSystem { get; }
}
