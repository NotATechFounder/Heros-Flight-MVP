using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;
using System;

[CreateAssetMenu(fileName = "New ItemEffectSO", menuName = "Inventory System/ItemEffectSO")]
public class ItemEffectSO : ScriptableObject
{
    [System.Serializable]
    public class EffectRarityStat
    {
        public Rarity rarity;
        public CustomAnimationCurve statCurve;
    }

    public ItemEffectType itemEffectType;
    public EffectInfo buffInfo;
    public EffectRarityStat[] effectRarityStats;


#if UNITY_EDITOR

    private void OnValidate()
    {
        foreach (EffectRarityStat rarityBuffStat in effectRarityStats)
        {
            rarityBuffStat.statCurve.UpdateCurve();
        }
    }

    public void RenameFile()
    {
        string thisFileNewName = AddSpacesToSentence(itemEffectType.ToString());
        string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, thisFileNewName);

        foreach (EffectRarityStat rarityBuffStat in effectRarityStats)
        {
            rarityBuffStat.statCurve.UpdateCurve();
        }
    }

    string AddSpacesToSentence(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ') newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }
#endif
}

[System.Serializable]
public struct EffectInfo
{
    public string effectName;
    public string effectSuffix;
}

