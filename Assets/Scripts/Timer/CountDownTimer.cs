using System;
using System.Collections;
using UnityEngine;

public class CountDownTimer 
{
    private MonoBehaviour owner;
    private bool paused = false;
    private float currentTime = 0;
    private Coroutine timerRoutine = null;
    private Action<float> onTimeTick = null;
    private Action onTimeLapse = null;
    private float maxTime = 0;

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
        onTimeTick = _onTimeTick;
        onTimeLapse = _onTimeLapse;
        timerRoutine = owner.StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (true)
        {
            if(!paused)
            {
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

    public float GetCurrentTime() => currentTime;
    public void Stop()
    {
        owner.StopCoroutine(timerRoutine);
        timerRoutine = null;
    }
}
