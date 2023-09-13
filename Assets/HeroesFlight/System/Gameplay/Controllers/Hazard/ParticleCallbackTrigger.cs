using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallbackTrigger : MonoBehaviour
{
    public Action OnEnd;

    private ParticleSystem pSystem;

    private void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped()
    {
        OnEnd?.Invoke();
    }

    public void Play()
    {
        pSystem.Play();
    }
}
