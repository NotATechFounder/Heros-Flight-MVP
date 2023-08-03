using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAttribute : Attribute
{
    public readonly string methodName;

    public ButtonAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}
