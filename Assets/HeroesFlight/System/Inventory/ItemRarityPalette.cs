using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Rarity Palette", menuName = "Inventory System/Rarity Palette")]

public class ItemRarityPalette : ScriptableObject
{
    public RarityPalette[] rarityPalettes;

    public RarityPalette GetRarity(Rarities rarity)
    {
        for (int i = 0; i < rarityPalettes.Length; i++)
        {
            if (rarityPalettes[i].rarity == rarity)
                return rarityPalettes[i];
        }
        return null;
    }
}

[Serializable]
public class RarityPalette
{
    public Rarities rarity;
    public Sprite itemFrame;
    public Color rarityColour;
}
