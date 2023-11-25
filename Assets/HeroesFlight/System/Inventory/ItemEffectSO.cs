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
    public class RarityBuffStat
    {
        public Rarities rarity;
        public CustomAnimationCurve statCurve;
    }

    public ItemEffectType buffType;
    public BuffInfo buffInfo;
    public RarityBuffStat[] buffRarityStats;


#if UNITY_EDITOR

    private void OnValidate()
    {
        foreach (RarityBuffStat rarityBuffStat in buffRarityStats)
        {
            rarityBuffStat.statCurve.UpdateCurve();
        }
    }

    public void RenameFile()
    {
        string thisFileNewName = AddSpacesToSentence(buffType.ToString());
        string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, thisFileNewName);

        foreach (RarityBuffStat rarityBuffStat in buffRarityStats)
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
public struct BuffInfo
{
    public string buffName;
    public string buffSuffix;
}

