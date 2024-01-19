using System;
using UnityEngine.UI;
using TMPro;
using HeroesFlight.System.UI.Reward;
using UnityEngine;

public class RewardView : MonoBehaviour
{
    public Action OnClicked;

    public Image rewardBG;
    public Image rewardIcon;
    public TextMeshProUGUI rewardText;

    public void SetVisual(RewardVisualEntry rewardVisual)
    {
        rewardIcon.sprite = rewardVisual.icon;
        rewardText.text = rewardVisual.amount.ToString();
        rewardBG.color = rewardVisual.color;
    }
}
