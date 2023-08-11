using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HPAttributeSO", menuName = "HeroProgression/HPAttributeSO", order = 1)]
public class HPAttributeSO : ScriptableObject
{
    [SerializeField] private HeroProgressionAttribute attribute;
    [TextArea(3, 10)]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private AttributeKeyValue[] KeyValues;

    public Sprite Icon => icon;
    public HeroProgressionAttribute Attribute => attribute;
    public string Description => description;

    public float GetKeyValue(string key)
    {
        foreach (var keyValue in KeyValues)
        {
            if (keyValue.key == key)
            {
                return keyValue.Value;
            }
        }
        return 0;
    }
}
