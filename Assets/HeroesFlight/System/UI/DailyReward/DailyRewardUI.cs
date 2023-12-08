using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.System.UI.Reward;

public class DailyRewardUI : MonoBehaviour
{
    public enum State
    {
        NotReady,
        Ready,
        Claimed    
    }

    [System.Serializable]
    public class SingleRewardView
    {
        public Image rewardBG;
        public Image rewardIcon;
        public TextMeshProUGUI rewardText;

        public void SetVisual(RewardVisual rewardVisual)
        {
            rewardIcon.sprite = rewardVisual.icon;
            rewardText.text = rewardVisual.amount.ToString();
            rewardBG.color = rewardVisual.color;
        }
    }

    public Action<int> OnRewardButtonClicked;

    [SerializeField] private State state;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private SingleRewardView[] singleRewardViews;

    [Header ("NotReady")]
    [SerializeField] private GameObject notReadyContent;

    [Header("Ready")]
    [SerializeField] private GameObject readyContent;

    [Header("Claimed")]
    [SerializeField] private GameObject claimedContent;

    private int rewardIndex;
    private AdvanceButton advanceButton;

    JuicerRuntime readyEffect;


    private void Awake()
    {
        advanceButton = GetComponentInChildren<AdvanceButton>();
        advanceButton.onClick.AddListener(() => OnRewardButtonClicked?.Invoke(rewardIndex));

        readyEffect = readyContent.transform.JuicyScale(1.2f, 1.0f);
        readyEffect.SetLoop(-1);
    }

    public void SetVisual(List<RewardVisual> rewardVisual)
    {
        for (int i = 0; i < rewardVisual.Count; i++)
        {
            singleRewardViews[i].SetVisual(rewardVisual[i]);
        }
    }

    public void SetRewardIndex (int rewardIndex)
    {
        this.rewardIndex = rewardIndex;
        dayText.text =  "Day " + (rewardIndex + 1).ToString();
    }

    public void SetState(State state)
    {
        this.state = state;

        if(state == State.Ready)
        {
            readyEffect.Start();
        }
        else
        {
            readyEffect.Stop();
        }

        advanceButton.interactable = state == State.Ready;
        notReadyContent .SetActive(state == State.NotReady);
        readyContent.SetActive(state == State.Ready);
        claimedContent.SetActive(state == State.Claimed);
    }
}
