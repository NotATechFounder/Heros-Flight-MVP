using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeType { Seconds, Minutes, Hours, Days }

public class TimedReward
{
    public Action OnInternetConnected;
    public Action OnInternetDisconnected;
    public Action OnRewardReadyToBeCollected;
    public Action<string> RewardPlayer;
    public Action<string> OnTimerUpdateed;

    [SerializeField] TimeType timeType;
    [SerializeField] double nextRewardDelay = 20f;
    [SerializeField] float checkingInterval = 5f; //Check for reward every 5 secs

    private double elapsedTime;
    private string rewardClaimDate = null;
    private bool isRewardReady = false;
    private bool isInternetConnected = false;
    private MonoBehaviour owner;

    public bool IsRewardReady() => isRewardReady;

    public void Init(MonoBehaviour owner, string rewardClaimDate, TimeType timeType, double nextRewardDelay, float checkingInterval)
    {
        this.owner = owner;
        this.rewardClaimDate = rewardClaimDate;
        this.timeType = timeType;
        this.nextRewardDelay = nextRewardDelay;
        this.checkingInterval = checkingInterval;
        owner.StartCoroutine(CheckingForConnection());
    }

    private IEnumerator CheckingForConnection()
    {
        while (!isRewardReady)
        {
            if (InternetManager.Instance.IsInternetTimeLoaded())
            {
                if (!isInternetConnected)
                {
                    isInternetConnected = true;
                    OnInternetConnected?.Invoke();
                }

                CheckTimer();
            }
            else
            {
                if (isInternetConnected)
                {
                    isInternetConnected = false;
                    OnInternetDisconnected?.Invoke();
                }
            }

            yield return new WaitForSeconds(checkingInterval);
        }
    }

    private void CheckTimer()
    {
        if (isRewardReady) return;

        ShowNextRewardTime();

        DateTime currentDateTime = InternetManager.Instance.GetCurrentDateTime();
        DateTime rewardClaimDateTime = DateTime.Parse((rewardClaimDate != null) ? rewardClaimDate : currentDateTime.ToString());

        switch (timeType)
        {
            case TimeType.Seconds:
                elapsedTime = (currentDateTime - rewardClaimDateTime).TotalSeconds;
                break;
            case TimeType.Minutes:
                elapsedTime = (currentDateTime - rewardClaimDateTime).TotalMinutes;
                break;
            case TimeType.Hours:
                elapsedTime = (currentDateTime - rewardClaimDateTime).TotalHours;
                break;
            case TimeType.Days:
                elapsedTime = (currentDateTime - rewardClaimDateTime).TotalDays;
                break;
            default:
                break;
        }

        if (elapsedTime >= nextRewardDelay) TimedReweardUnlocked();
    }

    private void TimedReweardUnlocked()
    {
        isRewardReady = true;
        OnRewardReadyToBeCollected?.Invoke();
    }

    /// <summary>
    /// Call this method when player claim the reward
    /// </summary>
    public void ClaimTimedReward()
    {
        if (!InternetManager.Instance.IsInternetTimeLoaded()) return;

        rewardClaimDate = InternetManager.Instance.GetCurrentDateTime().ToString();

        RewardPlayer?.Invoke(rewardClaimDate);
        isRewardReady = false;

        ShowNextRewardTime();

        owner.StartCoroutine(CheckingForConnection());
    }

    private void ShowNextRewardTime()
    {
        DateTime lastRewardTime = AddDuration(DateTime.Parse((rewardClaimDate != null) ? rewardClaimDate : InternetManager.Instance.GetCurrentDateTime().ToString()), (int)(nextRewardDelay));
        UpdateTimerDisplay(InternetManager.Instance.GetCurrentDateTime(), lastRewardTime);
    }

    private void UpdateTimerDisplay(DateTime currentDateTime, DateTime rewardClaimDateTime)
    {
        TimeSpan time = rewardClaimDateTime - currentDateTime;

        string timeValue = "";

        if (time.Days != 0) timeValue = String.Format("{0:D1}D : {1:D2}H: {2:D2}M : {3:D2}S", time.Days, time.Hours, time.Minutes, time.Seconds);
        else if (time.Days == 0 && time.Hours == 0 && time.Minutes == 0) timeValue = String.Format("{0:D2}S", time.Seconds);
        else if (time.Days == 0 && time.Hours == 0) timeValue = String.Format("{0:D2}M : {1:D2}S", time.Minutes, time.Seconds);
        else timeValue = String.Format("{0:D2}H: {1:D2}M : {2:D2}S", time.Hours, time.Minutes, time.Seconds);

        OnTimerUpdateed?.Invoke(timeValue);
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        DateTime Duration = new DateTime();
        switch (timeType)
        {
            case TimeType.Seconds:
                Duration = dateTime.AddSeconds(duration);
                break;
            case TimeType.Minutes:
                Duration = dateTime.AddMinutes(duration);
                break;
            case TimeType.Hours:
                Duration = dateTime.AddHours(duration);
                break;
            case TimeType.Days:
                Duration = dateTime.AddDays(duration);
                break;
            default: break;
        }
        return Duration;
    }
}