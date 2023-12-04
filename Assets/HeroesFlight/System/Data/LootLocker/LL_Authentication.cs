using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System;

public class LL_Authentication : MonoBehaviour
{
    public void GuestLogin(Action OnGestLoginComplected)
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }
            OnGestLoginComplected?.Invoke();    
            Debug.Log("successfully started LootLocker session");
        });
    }

    public void AppleLogIn()
    {
        string authCode = "put authorizationCode here";
        LootLockerSDKManager.StartAppleSession(authCode, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("session started successfully");
        });

        LootLockerSDKManager.RefreshAppleSession((response) =>
        {
            if (!response.success)
            {
                if (response.statusCode == 401)
                {
                    // Refresh token has expired, use StartAppleSession
                }
                else
                {
                    Debug.Log("error starting LootLocker session");
                }

                return;
            }

            Debug.Log("session started successfully");
        });
    }

    public void AndroidLogIn()
    {
        string idToken = "eyJhbGciOiJSUz............";
        LootLockerSDKManager.StartGoogleSession(idToken, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("session started successfully");

            // Store these to be able to refresh the session without using the full sign in flow
            string refreshToken = response.refresh_token;
        });

        LootLockerSDKManager.RefreshGoogleSession((response) =>
        {
            if (!response.success)
            {
                if (response.statusCode == 401)
                {
                    // Refresh token has expired, use StartGoogleSession
                }
                else
                {
                    Debug.Log("error starting LootLocker session");
                }

                return;
            }

            Debug.Log("session started successfully");
        });
    }
}
