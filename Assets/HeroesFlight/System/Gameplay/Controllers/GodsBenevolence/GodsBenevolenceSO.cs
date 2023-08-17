using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GodsBenevolence", menuName = "GodsBenevolence", order = 1)]
public class GodsBenevolenceSO : ScriptableObject
{
    [SerializeField] private string benevolenceName;

    [TextArea(3, 10)]
    [SerializeField] private string benevolenceDescription;
    [SerializeField] private Sprite benevolenceIcon;
    [SerializeField] private GodBenevolenceType benevolenceType;
    [SerializeField] public GodBenevolenceTarget benevolenceTarget;
    [SerializeField] private GodsBenevolenceKeyValue[] benevolenceKeyValues;
    [SerializeField] private Sprite[] benevolencePuzzle;

    public string BenevolenceName => benevolenceName;
    public string BenevolenceDescription => benevolenceDescription;
    public Sprite BenevolenceIcon => benevolenceIcon;
    public GodBenevolenceType BenevolenceType => benevolenceType;

    public GodBenevolenceTarget BenevolenceTarget => benevolenceTarget;
    public GodsBenevolenceKeyValue[] BenevolenceKeyValues => benevolenceKeyValues;
    public Sprite[] BenevolencePuzzle => benevolencePuzzle;
}
