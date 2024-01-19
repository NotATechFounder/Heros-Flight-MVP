using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_ANDROID
    private static readonly string storeID = "5523599";
    private static readonly string videoID = "Interstitial_Android";
    private static readonly string rewaredID = "Rewarded_Android";
#elif UNITY_IOS
    private static readonly string storeID = "5523598";
    private static readonly string videoID = "Interstitial_iOS";
    private static readonly string rewaredID = "Rewarded_iOS";
#endif

    private Action adSuccess;
    private Action adSkipped;
    private Action adFailed;

#if UNITY_EDITOR
    private static bool testMode = true;
#else
 private static bool testMode = false;
#endif

    public bool IsReady = false;

    public void Init()
    {
        if (!Advertisement.isInitialized && !Advertisement.isSupported) return;
        Advertisement.Initialize(storeID, testMode, this);
        Debug.Log("loading ads");
        Advertisement.Load(rewaredID, this);
        Advertisement.Load(videoID, this);
    }

    public void OnInitializationComplete()
    {
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError("Error: " + message);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"AD LOADED {placementId}");
#if UNITY_ANDROID
        if (placementId == rewaredID)
        {
            IsReady = true;
        }
#elif UNITY_IOS
   if (placementId == rewaredID)
        {
            IsReady = true;
        }
#endif
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == rewaredID)
        {
            switch (showCompletionState)
            {
                case UnityAdsShowCompletionState.SKIPPED:
                    adSkipped?.Invoke();
                    break;
                case UnityAdsShowCompletionState.COMPLETED:
                    adSuccess?.Invoke();
                    break;
                case UnityAdsShowCompletionState.UNKNOWN:
                    adFailed?.Invoke();
                    break;
                default: break;
            }
        }
    }


    public void ShowStandardAd()
    {
        Advertisement.Show(videoID, this);
    }

    public void ShowRewardedAd(Action success, Action skipped = null, Action failed = null)
    {
        adSuccess = success;
        adSkipped = skipped;
        adFailed = failed;
        Advertisement.Show(rewaredID, this);
        Advertisement.Load(rewaredID, this);
        Advertisement.Load(videoID, this);
    }
}