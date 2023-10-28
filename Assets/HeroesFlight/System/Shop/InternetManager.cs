using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class InternetManager : MonoBehaviour
{
    public static InternetManager Instance { get; private set; }

    [SerializeField] bool isConnected = false;
    [SerializeField] bool isTimeLoaded = false;
    struct TimeData { public string datetime; }
    const string API_URL = "http://worldtimeapi.org/api/ip";
    private DateTime _currentDateTime = DateTime.Now;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(GetRealDateTimeFromAPI());
        }
        else Destroy(gameObject);
    }

    public void ConnectToInternet(Action success)
    {
        StartCoroutine(CheckInternetConnection(success));
    }

    IEnumerator CheckInternetConnection(Action success)
    {
        UnityWebRequest request = new UnityWebRequest("Http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null) InternetConnectionFailed(); else success();
    }

    public void InternetConnected()
    {
        isConnected = true;
    }

    public void InternetConnectionFailed()
    {
        isConnected = false;
    }

    public void ReConnect()
    {
        ConnectToInternet(InternetConnected);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            ConnectToInternet(InternetConnected);
        }
    }

    //Getting Internet Time
    public DateTime GetCurrentDateTime() => _currentDateTime.AddSeconds(Time.realtimeSinceStartup);

    public DateTime GetStartTime() => _currentDateTime;

    public bool IsInternetTimeLoaded() => isTimeLoaded;

    public bool IsInternetConnected() => isConnected;

    public IEnumerator GetRealDateTimeFromAPI()
    {
        while (!isTimeLoaded)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);
            yield return webRequest.SendWebRequest();

            if (webRequest.error == null)
            {
                TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);
                _currentDateTime = ParseDateTime(timeData.datetime);
                isTimeLoaded = true;
                InternetConnected();
            }
            else InternetConnectionFailed();
        }
    }

    DateTime ParseDateTime(string datetime)
    {
        string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;
        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }
}