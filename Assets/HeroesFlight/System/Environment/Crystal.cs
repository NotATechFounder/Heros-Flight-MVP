using HeroesFlight.System.Environment;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Action OnDestroyed;

    [SerializeField] Sprite[] sprites;
    [SerializeField] BoosterDropSO boosterDropSO;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AnimationCurve hitEffectCurve;

    [Header("Gold")]
    [SerializeField] RangeValue goldRange;
    [SerializeField] int goldInBatch = 10;

    [Header("Shake")]
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakePower = 0.5f;

    public BoosterDropSO BoosterDropSO => boosterDropSO;

    public int GoldInBatch => goldInBatch;
    public int GoldAmount => Mathf.RoundToInt(goldRange.GetRandomValue()) / goldInBatch;

    JuicerRuntime shineEffect;
    JuicerRuntime hitEffect; 
    private Vector3 lastPos;
    CoroutineHandle shakeRoutine;

    private void Awake()
    {
        lastPos = transform.position;

        shineEffect = spriteRenderer.material.JuicyFloatProperty("_ShineLocation", 1f, 0.5f);
        shineEffect.SetLoop(-1);
        shineEffect.SetStepDelay(1f);

        hitEffect = spriteRenderer.material.JuicyFloatProperty("_HitEffectBlend", 1f, 0.25f).SetEase(hitEffectCurve);
    }

    private void OnEnable()
    {
        spriteRenderer.material.SetFloat("_ShineLocation", 0f);
        spriteRenderer.material.SetFloat("_HitEffectBlend", 0f);

        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        shineEffect.Start();
    }


    private void OnDisable()
    {
        shineEffect.Stop();
    }

    public void OnHit()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Shake(randomDirection);
        hitEffect.Start();
    }

    private void Shake(Vector3 direction)
    {
        direction.y = 0;

        if (shakeRoutine != null && !shakeRoutine.IsDone)
        {
            transform.position = lastPos;
            Juicer.StopCoroutine(shakeRoutine);
        }
        lastPos = transform.position;
        shakeRoutine = transform.JuicyShakePosition(shakeDuration, direction * shakePower, 1);
    }
}
