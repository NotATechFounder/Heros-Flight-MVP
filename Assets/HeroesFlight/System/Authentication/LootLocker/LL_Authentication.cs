using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System;

public class LL_Authentication : MonoBehaviour
{
    public Action<LoginMode> OnLoginComplected;
    public Action OnInvalidUserName;

    [SerializeField] private LoginMode currentLoginMode;
    [SerializeField] private string playerID;
    [SerializeField] private LLPlayerProfile playerProfile;

    public LoginMode CurrentLoginMode => currentLoginMode;
    public LLPlayerProfile GetPlayerProfile => playerProfile;

    public bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public void TryLogin(LoginMode loginMode)
    {
        switch (loginMode)
        {
            case LoginMode.Guest:
                GuestLogin(LoginSuccessFul);
                break;
            case LoginMode.Apple:
                AppleLogIn(LoginSuccessFul);
                break;
            case LoginMode.Google:
                AndroidLogIn(LoginSuccessFul);
                break;
            case LoginMode.Offline:
                OnLoginComplected?.Invoke(LoginMode.Offline);
                break;
            default:  break;
        }
    }

    public void LoginSuccessFul(LoginMode loginMode)
    {
        currentLoginMode = loginMode;

        if (currentLoginMode == LoginMode.Offline)
        {
            OnLoginComplected?.Invoke(currentLoginMode);
        }
        else
        {
            StartCoroutine(CreatePlayerProfile(CheckUserName));
        }
    }

    public IEnumerator CreatePlayerProfile(Action OnCreated)
    {
        yield return LootLockerUtil.CreatePlayerProfile(playerID, result =>
        {
            playerProfile = result;
        });
        OnCreated?.Invoke();
    }

    public void CheckUserName()
    {
        if (string.IsNullOrEmpty(playerProfile.name))
        {
            OnInvalidUserName?.Invoke();
        }
        else
        {
            Debug.Log("CheckUserName SuccessFul");
            OnLoginComplected?.Invoke(currentLoginMode);
        }
    }

    public void TryChangeUserName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("Invalid name");
            return;
        }
        StartCoroutine(ChangeUserName(newName));
    }

    public IEnumerator ChangeUserName(string newName)
    {
        if (string.IsNullOrEmpty(playerProfile.name))
        {
            yield return LootLockerUtil.ChangePlayerName(newName, result =>
            {
                playerProfile.name = newName;
                OnLoginComplected?.Invoke(currentLoginMode);
            });
        }
    }

    public void GuestLogin(Action<LoginMode> LoginComplected)
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");
                return;
            }

            playerID = response.player_id.ToString();
            LoginComplected?.Invoke(LoginMode.Guest);    
        });
    }

    public void AppleLogIn(Action<LoginMode> LoginComplected)
    {
        string authCode = "put authorizationCode here";
        LootLockerSDKManager.StartAppleSession(authCode, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            playerID = response.player_id.ToString();
            LoginComplected?.Invoke(LoginMode.Apple);
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

    public void AndroidLogIn(Action<LoginMode> LoginComplected)
    {
        string idToken = "eyJhbGciOiJSUz............";
        LootLockerSDKManager.StartGoogleSession(idToken, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            string refreshToken = response.refresh_token;

            playerID = response.player_id.ToString();
            LoginComplected?.Invoke(LoginMode.Apple);
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
