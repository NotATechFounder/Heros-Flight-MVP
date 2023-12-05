using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationSystem : IAuthenticationInterface
{
    public LL_Authentication LL_Authentication { get; private set; }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        LL_Authentication = scene.GetComponent<LL_Authentication>();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
