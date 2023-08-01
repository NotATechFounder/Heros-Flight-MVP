using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class PermanetCardUI : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardEffectText;
    [SerializeField] private Image cardImage;

    AngelCard angelCard = null; 
    StringBuilder effectText = new StringBuilder();
    private bool isCardSet = false;

    public bool IsCardSet => isCardSet;
    public AngelCard AngelCard => angelCard;

    public void SetCard(AngelCard angelCard)
    {
        this.angelCard = angelCard;

        effectText = new StringBuilder();

        tierText.text = "Tier " + ((int)angelCard.tier + 1);

        cardNameText.text = angelCard.angelCardSO.CardName;

        effectText.Append(angelCard.angelCardSO.AffterBonusEffect.GetValue(angelCard.tier));
        effectText.Append("%");
        effectText.Append("  ");
        effectText.Append(angelCard.angelCardSO.AffterBonusEffect.effect.ToString());
        effectText.Append("  ");
        effectText.Append(angelCard.angelCardSO.AffterBonusEffect.targetType.ToString());

        cardEffectText.text = effectText.ToString();
        //cardImage.sprite = angelCard.angelCardSO.CardImage;

        isCardSet = true;

        content.SetActive(true);
    }

    public void Clear()
    {
        isCardSet = false;
        content.SetActive(false);
    }
}
