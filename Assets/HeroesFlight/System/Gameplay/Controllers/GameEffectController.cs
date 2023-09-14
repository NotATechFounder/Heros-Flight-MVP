using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectController : MonoBehaviour
{
    Coroutine stopTimeCoroutine;
    Coroutine stopFrameCoroutine;

    public void StopTime(float newTimeScale,float restoreSpeed, float duration)
    {
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
