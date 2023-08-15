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
    [SerializeField] private AttributeKeyValue[] keyValues;

    public Sprite Icon => icon;
    public HeroProgressionAttribute Attribute => attribute;
    public string Description => description;
    public AttributeKeyValue[] KeyValues => keyValues;
}
