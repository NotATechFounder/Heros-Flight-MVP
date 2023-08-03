using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpriteAttribute : Attribute
{
    public float Height;
    public float Width;

    public PreviewSpriteAttribute(float _Height = 80, float _Width = 80)
    {
        Height = _Height;
        Width = _Width;
    }
}
