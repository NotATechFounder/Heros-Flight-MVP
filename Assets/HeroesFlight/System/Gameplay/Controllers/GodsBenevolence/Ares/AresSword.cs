using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AresSword : MonoBehaviour
{
    [SerializeField] private Transform swordHolder;
    [SerializeField] private Transform swordHandle;
    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] private ParticleSystem clashEffect;

    [Header("Clash Foward Effect")]
    [SerializeField] private Vector3 rotateFowardDest = new Vector3(0, 0, 360);
    [SerializeField] private float fowardSpeed = 1f;
    [SerializeField] private Ease fowardEase = Ease.Linear;

    [Header("Clash Backward Effect")]
    [SerializeField] private Vector3 rotateBackwardDest = new Vector3(0, 0, 180);
    [SerializeField] private float backwardSpeed = 1f;
    [SerializeField] private Ease backwardEase = Ease.Linear;

    JuicerRuntime clashFowardEffectFoward;
    JuicerRuntime clashFowardEffectBackward;

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;

        clashFowardEffectFoward = swordHandle.JuicyLocalRotate(rotateFowardDest, fowardSpeed);
        clashFowardEffectFoward.SetEase(fowardEase);
        clashFowardEffectFoward.AddTimeEvent(0.5f, () =>
        {
            overlapChecker.SetDirection(swordHolder.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
            overlapChecker.Detect();
        });
        clashFowardEffectFoward.SetOnComplected(() =>
        {
            clashFowardEffectBackward.Start();
        });

        clashFowardEffectBackward = swordHandle.JuicyLocalRotate(rotateBackwardDest, backwardSpeed);
        clashFowardEffectBackward.SetEase(backwardEase);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            clashEffect.Play();
            clashFowardEffectFoward.Start();
        }
    }

    public void Flip(float direction)
    {
        swordHolder.localScale = new Vector3(direction, 1, 1);
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(i + " " + colliders[i].name);
        }
    }
}
