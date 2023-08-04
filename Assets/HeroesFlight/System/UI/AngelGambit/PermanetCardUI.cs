using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Pelumi.Juicer;
using System;

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

    JuicerRuntime openEffectBG;
    JuicerRuntime tierEffect;
    JuicerRuntime statEffect;

    private void Awake()
    {
        openEffectBG = content.transform.JuicyScale(Vector3.one, 0.15f).
            SetEase(Ease.EaseInOutSine);

        tierEffect = tierText.transform.JuicyScale(Vector3.one, 0.25f)
            .SetEase(Ease.Linear)
            .SetOnComplected(() => tierText.text = "Tier " + ((int)angelCard.tier + 1));

        statEffect = cardEffectText.JuicyText("", 0.5f)
            .SetTextAnimationMode(TextAnimationMode.ClearOldText)
            .SetOnStart(() => cardEffectText.color = Color.green)
            .SetOnComplected(() => cardEffectText.color = Color.white)
            .SetDelay(0.25f);
    }

    public void SetCard(AngelCard angelCard, bool update)
    {
        this.angelCard = angelCard;

        effectText = new StringBuilder();

        effectText.Append(angelCard.angelCardSO.AffterBonusEffect.GetValue(angelCard.tier));
        effectText.Append("%");
        effectText.Append("  ");
        effectText.Append(angelCard.angelCardSO.AffterBonusEffect.effect.ToString());
        //effectText.Append("  ");
        //effectText.Append(angelCard.angelCardSO.AffterBonusEffect.targetType.ToString());
        //cardImage.sprite = angelCard.angelCardSO.CardImage;

        if(!update)
        {
            isCardSet = true;

            tierText.text = "Tier " + ((int)angelCard.tier + 1);

            cardNameText.text = angelCard.angelCardSO.CardName;

            cardEffectText.text = effectText.ToString();

            content.transform.localScale = Vector3.zero;

            content.SetActive(true);

            openEffectBG.Start(() => content.transform.localScale = Vector3.zero);
        }
        else
        {
            tierEffect.Start(() => tierText.transform.localScale = Vector2.zero);
            statEffect.Start(() => statEffect.ChangeDesination(effectText.ToString()));
        }
    }

    internal void ResetCard()
    {
        isCardSet = false;
        content.SetActive(false);
    }
}
