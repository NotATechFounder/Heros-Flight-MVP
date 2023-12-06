using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AuthenticationMenu : MonoBehaviour
{
    [SerializeField] private LL_Authentication lL_Authentication;

    [Header("Main")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private AdvanceButton offlineButton;
    [SerializeField] private AdvanceButton guestButton;
    [SerializeField] private AdvanceButton appleButton;
    [SerializeField] private AdvanceButton googleButton;

    [Header("Username")]
    [SerializeField] private GameObject setUserNameCanvas;
    [SerializeField] private TMP_InputField userNameInputField;
    [SerializeField] private AdvanceButton setUserNameButton;

    private void Awake()
    {
        setUserNameButton.onClick.AddListener(OnClickSetUserName);  
        offlineButton.onClick.AddListener(OnClickOfflineLogin);
        guestButton.onClick.AddListener(OnClickGuestLogin);
        InitButtons();
    }

    private void Start()
    {
        lL_Authentication.OnInvalidUserName += OnInvalidUserName;
        lL_Authentication.OnLoginComplected += OnLoginComplected;
    }

    private void OnDestroy()
    {
        lL_Authentication.OnInvalidUserName -= OnInvalidUserName;
        lL_Authentication.OnLoginComplected -= OnLoginComplected;
    }

    private void OnLoginComplected(LoginMode mode)
    {
        mainCanvas.SetActive(false);
    }

    public void InitButtons()
    {
        appleButton.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
        googleButton.gameObject.SetActive(Application.platform == RuntimePlatform.Android);
        appleButton.onClick.AddListener(OnClickAppleLogin);
        googleButton.onClick.AddListener(OnClickGoogleLogin);
    }

    private void OnInvalidUserName()
    {
        setUserNameCanvas.SetActive(true);
    }

    private void OnClickSetUserName()
    {
        lL_Authentication.TryChangeUserName(userNameInputField.text);
    }

    public void OnClickGuestLogin()
    {
        lL_Authentication.TryLogin(LoginMode.Guest);
    }

    public void OnClickAppleLogin()
    {
        lL_Authentication.TryLogin(LoginMode.Apple);
    }

    public void OnClickGoogleLogin()
    {
        lL_Authentication.TryLogin(LoginMode.Google);
    }

    public void OnClickOfflineLogin()
    {
        lL_Authentication.TryLogin(LoginMode.Offline);
    }
}
