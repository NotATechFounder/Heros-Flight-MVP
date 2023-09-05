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

    public BoosterDropSO BoosterDropSO => boosterDropSO;

    public int GoldInBatch => goldInBatch;
    public int GoldAmount => Mathf.RoundToInt(goldRange.GetRandomValue()) / goldInBatch;

    JuicerRuntime shineEffect;
    JuicerRuntime hitEffect;

    private void Awake()
    {
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
        hitEffect.Start();
    }
}
