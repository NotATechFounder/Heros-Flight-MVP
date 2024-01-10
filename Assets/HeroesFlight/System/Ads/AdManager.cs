using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_ANDROID
    private static readonly string storeID = "4407797";
#elif UNITY_IOS
    private static readonly string storeID = "4407796";
#endif

    private static readonly string videoID = "video";
    private static readonly string rewaredID = "rewardedVideo";

    private Action adSuccess;
    private Action adSkipped;
    private Action adFailed;

#if UNITY_EDITOR
    private static bool testMode = true;
#else
 private static bool testMode = false;
#endif

    public static AdManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Advertisement.Initialize(storeID, testMode);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnInitializationComplete()
    {
 
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {

    }

    public void OnUnityAdsAdLoaded(string placementId)
    {

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
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                if (placementId == rewaredID)
                {
                    adSkipped();
                }
                break;
            case UnityAdsShowCompletionState.COMPLETED:
                if (placementId == rewaredID)
                {
                    adSuccess();
                }
                break;
            case UnityAdsShowCompletionState.UNKNOWN:
                if (placementId == rewaredID)
                {
                    adFailed();
                }
                break;
            default:   break;
        }
    }

    public static void ShowStandardAd()
    {
        Advertisement.Show(videoID);
    }

    public static void ShowRewarededAd(Action success, Action skipped, Action failed)
    {
        instance.adSuccess = success;
        instance.adSkipped = skipped;
        instance.adFailed = failed;
        Advertisement.Show(rewaredID);
    }


    public static void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
}