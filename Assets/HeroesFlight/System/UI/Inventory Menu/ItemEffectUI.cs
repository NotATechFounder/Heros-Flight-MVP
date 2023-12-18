using HeroesFlight.Common.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HeroesFlight.System.UI.Inventory_Menu;
using UnityEngine.UI;

public class ItemEffectUI : MonoBehaviour
{
    public Image effectBg;
    public TextMeshProUGUI effectNameText;
    public TextMeshProUGUI effectValueText;
    public TextMeshProUGUI effectNextValueText;

    public void Init(ItemEffectEntryUi itemEffectEntryUi, bool unlocked)
    {
        effectNameText.text = itemEffectEntryUi.effectName;
        effectValueText.text = itemEffectEntryUi.value.ToString();

        effectBg.color = itemEffectEntryUi.rarityPalette.backgroundColour;
        if(unlocked)
        {
            if(itemEffectEntryUi.nextValue != 0 && itemEffectEntryUi.nextValue != itemEffectEntryUi.value)
            {
                effectNextValueText.text = " -> " + itemEffectEntryUi.nextValue.ToString();
                effectNextValueText.color = Color.yellow;
            }
            else
            {
                effectNextValueText.text = "";
            }
        }
        else
        {
            effectNextValueText.text = "Locked";
            effectNextValueText.color = Color.red;
        }
    }
}
