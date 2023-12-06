using HeroesFlight.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuthenticationInterface : SystemInterface
{
    public LL_Authentication LL_Authentication { get; }
}
