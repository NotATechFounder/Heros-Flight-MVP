using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewardUI : MonoBehaviour
{
    public enum State
    {
        NotReady,
        Ready,
        Claimed    
    }

    public Action<int> OnRewardButtonClicked;

    [SerializeField] private State state;
    [SerializeField] private Image rewardBG;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header ("NotReady")]
    [SerializeField] private GameObject notReadyContent;

    [Header("Ready")]
    [SerializeField] private GameObject readyContent;

    [Header("Claimed")]
    [SerializeField] private GameObject claimedContent;

    private int rewardIndex;
    private AdvanceButton advanceButton;


    private void Awake()
    {
        advanceButton = GetComponentInChildren<AdvanceButton>();
        advanceButton.onClick.AddListener(() => OnRewardButtonClicked?.Invoke(rewardIndex));   
    }

    public void SetRewardIndex (int rewardIndex)
    {
        this.rewardIndex = rewardIndex;
        dayText.text =  "Day " + (rewardIndex + 1).ToString();
    }

    public void SetState(State state)
    {
        this.state = state;
        advanceButton.interactable = state == State.Ready;
        notReadyContent .SetActive(state == State.NotReady);
        readyContent.SetActive(state == State.Ready);
        claimedContent.SetActive(state == State.Claimed);
    }
}
