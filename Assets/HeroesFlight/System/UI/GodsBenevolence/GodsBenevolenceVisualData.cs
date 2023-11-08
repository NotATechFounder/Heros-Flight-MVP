using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GodsBenevolenceVisualData
{
    [SerializeField] private GodBenevolenceType benevolenceType;
    [TextArea(3, 10)]
    [SerializeField] private string benevolenceDescription;
    [SerializeField] private Sprite benevolenceIcon;
    [SerializeField] private BenevolencePuzzle[] benevolencePuzzles;
    [SerializeField] private string completedSfxKey;

    private int lastPuzzleIndex = 0;

    public GodBenevolenceType BenevolenceType => benevolenceType;
    public string CompletedSfxKey => completedSfxKey;

    public Sprite[] GetBenevolencePuzzle()
    {
        return GetRandomBenevolencePuzzle (ref lastPuzzleIndex);
    }

    public Sprite[] GetRandomBenevolencePuzzle(ref int lastIndex)
    {
        if (benevolencePuzzles.Length == 0)
        {
            return null;
        }

        if (benevolencePuzzles.Length == 1)
        {
            return benevolencePuzzles[0].pieces;
        }

        int randomIndex = 0;
        do
        {
            randomIndex = Random.Range(0, benevolencePuzzles.Length);
        } while (lastIndex == randomIndex);

        lastIndex = randomIndex;

        return benevolencePuzzles[lastIndex].pieces;
    }
}


[System.Serializable]
public class BenevolencePuzzle
{
    public Sprite[] pieces;
}
