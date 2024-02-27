using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBAnalyticsInitializer : MonoBehaviour
{
    private static FBAnalyticsInitializer Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Application.targetFrameRate = 60;
            }
            Init();
        }
    }

    private void Init()
    {
        if(FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(() =>
            {
                FB.ActivateApp();
            });
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            Init();
        }
    }
}
