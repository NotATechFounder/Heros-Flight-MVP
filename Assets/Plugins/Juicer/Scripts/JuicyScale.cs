using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuicyScale : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 endSacle;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] LoopType loopType = LoopType.Yoyo;
    [SerializeField] private int loopCount = 0;

    private JuicerRuntime moveEffect;

    private void Awake()
    {
        moveEffect = transform.JuicyScale(endSacle, duration);
        moveEffect.SetEase(easeType);
        moveEffect.SetLoop(loopCount, loopType);
    }

    private void OnEnable()
    {
        StartJuicy();
    }

    private void OnDisable()
    {
        StopJuicy();
    }

    public void StartJuicy()
    {
        moveEffect.Start();
    }

    public void PauseJuicy()
    {
        moveEffect.Pause();
    }

    public void StopJuicy()
    {
        moveEffect.Stop();
    }
}
