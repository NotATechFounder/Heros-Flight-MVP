using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuicyMove : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] LoopType loopType = LoopType.Yoyo;
    [SerializeField] private int loopCount = 0;

    private JuicerRuntime moveEffect;

    private void Awake()
    {
       // Vector3 outsideScreen = Screen.width * Vector3.right;

        moveEffect = transform.JuicyMove(targetPosition, duration);
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

    [ContextMenu("Test")]
    public void Test()
    {
        Debug.Log(transform.position);
    }
}
