using HeroesFlight.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IShopSystemInterface : SystemInterface
{
    public ShopDataHolder ShopDataHolder { get; }

    public void InjectUiConnection();
    public void ProccessRewards(List<Reward> rewards);
}
