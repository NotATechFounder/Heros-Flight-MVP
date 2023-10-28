using HeroesFlight.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IShopSystemInterface : SystemInterface
{
    public ShopManager ShopManager { get; }
}
