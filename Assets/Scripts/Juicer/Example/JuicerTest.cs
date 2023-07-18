using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JuicerTest : MonoBehaviour
{
    public void FullUseage()
    {
        JuicerRuntime juicerRuntime = transform.JuicyScale(Vector3.one, 0.2f);

        juicerRuntime.SetTimeMode(TimeMode.Unscaled);

        juicerRuntime.SetLoop(1, LoopType.Incremental);

        juicerRuntime.AddTimeEvent(0.5f, () =>
        {
            Debug.Log("Time Event");
        });

        juicerRuntime.SetOnStart(() =>
        {
            Debug.Log("On Start");
        });

        juicerRuntime.SetOnTick(() =>
        {
            Debug.Log("On Tick");
        });

        juicerRuntime.SetOnStepComplete(() =>
        {
            Debug.Log("On StepComplete");
        });

        juicerRuntime.SetOnComplected(() =>
        {
            Debug.Log("On Complected");
        });

        juicerRuntime.Start();

        juicerRuntime.Pause();

        juicerRuntime.Stop();
    }
}