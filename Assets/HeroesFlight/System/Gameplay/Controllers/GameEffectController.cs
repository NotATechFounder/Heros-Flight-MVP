using StansAssets.Foundation.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectController : MonoBehaviour
{
    Coroutine stopTimeCoroutine;
    Coroutine stopFrameCoroutine;
    private bool stoppingTime = false;

    public void ForceStop(Action OnFinished)
    {
        stoppingTime = true;
    
        if (stopTimeCoroutine != null)
        {
            StopCoroutine(stopTimeCoroutine);
            stopTimeCoroutine = null;
        }

      
        Time.timeScale = 0.2f;
        Debug.Log(Time.timeScale);
        CoroutineUtility.WaitForSecondsRealtime(2f, () =>
        {
            Time.timeScale = 1f;
            stoppingTime = false;
            OnFinished?.Invoke();
        });
    }

    public void StopTime(float newTimeScale,float restoreSpeed, float duration)
    {
        if (stoppingTime)
            return;
        
        Time.timeScale = newTimeScale;
        if (stopTimeCoroutine != null) return;
        stopTimeCoroutine = StartCoroutine(StopTimeCoroutine(restoreSpeed, duration));
    }

    private IEnumerator StopTimeCoroutine(float restoreSpeed, float duration)
    {
      
        yield return new WaitForSecondsRealtime(duration);
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.unscaledDeltaTime * restoreSpeed;
            yield return null;
        }
     
        Time.timeScale = 1;
        stopTimeCoroutine = null;
    }

    public void StopFrame(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        if (stopFrameCoroutine != null) return;
        stopFrameCoroutine = StartCoroutine(StopTimeCoroutine());
    }

    private IEnumerator StopTimeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Time.timeScale = 1;
        stopFrameCoroutine = null;
    }
}
