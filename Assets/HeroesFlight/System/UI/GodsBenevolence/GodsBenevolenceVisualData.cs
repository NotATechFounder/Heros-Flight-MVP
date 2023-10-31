using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GodsBenevolenceVisualData
{
    [SerializeField] private GodBenevolenceType benevolenceType;
    [SerializeField] private string benevolenceDescription;
    [SerializeField] private Sprite benevolenceIcon;
    [SerializeField] private Sprite[] benevolencePuzzle;
    [SerializeField] private string completedSfxKey;

    public GodBenevolenceType BenevolenceType => benevolenceType;
    public Sprite[] BenevolencePuzzle => benevolencePuzzle;
    public string CompletedSfxKey => completedSfxKey;
}
