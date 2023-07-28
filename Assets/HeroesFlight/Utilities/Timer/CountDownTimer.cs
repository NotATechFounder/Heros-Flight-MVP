using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CountDownTimer
{
    public event Action<float> onTimeTick = null;
    public event Action onTimeLapse = null;

    private MonoBehaviour owner;
    private bool paused = false;
    private float currentTime = 0;
    private Coroutine timerRoutine = null;

    private float maxTime = 0;
    private float lastTime = 0;

    public CountDownTimer() { }
    public CountDownTimer(MonoBehaviour monoBehaviour)
    {
        owner = monoBehaviour;
    }
    
    
    public void Start(float _time, Action<float> _onTimeTick, Action _onTimeLapse)
    {
        if (timerRoutine != null)
            return;

        maxTime = _time;
        currentTime = maxTime;
        onTimeTick += _onTimeTick;
        onTimeLapse += _onTimeLapse;
        timerRoutine = owner.StartCoroutine(TimerRoutine());
    }

    public void Start(float _time, Action<float> _onTimeTick, Action _onTimeLapse, MonoBehaviour monoBehaviour)
    {
        owner = monoBehaviour;
        
        if (timerRoutine != null)
            return;

        maxTime = _time;
        currentTime = maxTime;
        onTimeTick += _onTimeTick;
        onTimeLapse += _onTimeLapse;
        timerRoutine = owner.StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (true)
        {
            if(!paused)
            {
                lastTime = currentTime;
                currentTime -= Time.deltaTime;
                onTimeTick?.Invoke(currentTime);

                if (currentTime <= 0)
                {
                    Stop();
                    onTimeLapse?.Invoke();
                }

                yield return null;
            }
        }
    }

    public void Pause() => paused = true;

    public void Resume() => paused = false;

    public float GetCurrentTime => currentTime;

    public float GetMaxTime => maxTime;

    public float GetNormalizedTime => currentTime / maxTime;

    public float GetLastTime => lastTime;

    public void Stop()
    {
        if (timerRoutine == null)
            return;
        
        owner.StopCoroutine(timerRoutine);
        timerRoutine = null;
    }
}
