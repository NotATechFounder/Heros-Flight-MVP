using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private DailyRewardMenu dailyRewardMenu;
    [SerializeField] private DailyReward dailyReward;

    private void Awake()
    {
        dailyReward.OnRewardReadyToBeCollected += dailyRewardMenu.OnRewardReadyToBeCollected;

        dailyRewardMenu.GetLastUnlockedIndex += dailyReward.GetLastUnlockedIndex;
        dailyRewardMenu.IsRewardReady += dailyReward.IsRewardReady;
        dailyRewardMenu.OnClaimRewardButtonClicked += dailyReward.ClaimReward;
    }

    private void Start()
    {
        dailyRewardMenu.InitDailyRewards();
        dailyRewardMenu.RefreshDailyRewards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
    
        }
    }
}
