using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GodsBenevolence", menuName = "GodsBenevolence/ GodsBenevolence", order = 1)]
public class GodsBenevolenceSO : ScriptableObject
{
    [SerializeField] private string benevolenceName;

    [TextArea(3, 10)]
    [SerializeField] private GodBenevolenceType benevolenceType;
    [SerializeField] public GodBenevolenceTarget benevolenceTarget;
    [SerializeField] private GodsBenevolenceKeyValue[] benevolenceKeyValues;
    [SerializeField] private GodsBenevolenceAfterEffect[] godsBenevolenceAfterEffects;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] GodsBenevolenceVisualData benevolenceVisualSO;
    [SerializeField] private string benevolenceDescription;
    [SerializeField] private Sprite benevolenceIcon;
    [SerializeField] private string completedSfxKey;
    [SerializeField] private Sprite[] benevolencePuzzle;

    public string BenevolenceName => benevolenceName;
    public string BenevolenceDescription => benevolenceDescription;
    public Sprite BenevolenceIcon => benevolenceIcon;
    public GodBenevolenceType BenevolenceType => benevolenceType;
    public GodBenevolenceTarget Target => benevolenceTarget;
    public GodsBenevolenceKeyValue[] BenevolenceKeyValues => benevolenceKeyValues;
    public GodsBenevolenceAfterEffect[] AfterEffects => godsBenevolenceAfterEffects;
    public GameObject EffectPrefab => effectPrefab;
    public GodsBenevolenceVisualData BenevolenceVisualSO => benevolenceVisualSO;
    public string CompletedSfxKey => completedSfxKey;

    public float GetValue(string key)
    {
        foreach (var keyValue in BenevolenceKeyValues)
        {
            if (keyValue.key == key)
            {
                return keyValue.GetValue();
            }
        }
        return 0;
    }
}
