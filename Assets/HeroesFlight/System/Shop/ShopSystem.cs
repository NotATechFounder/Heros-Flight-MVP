using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopSystem : IShopSystemInterface
{
    public ShopManager ShopManager { get; private set; }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        ShopManager = scene.GetComponent<ShopManager>();
    }

    public void Reset()
    {

    }
}
