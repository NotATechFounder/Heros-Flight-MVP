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

    [SerializeField] BoosterDropSO boosterDropSO;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AnimationCurve hitEffectCurve;

    [Header("Gold")]
    [SerializeField] RangeValue goldRange;
    [SerializeField] int goldInBatch = 10;

    public BoosterDropSO BoosterDropSO => boosterDropSO;

    public int GoldInBatch => goldInBatch;
    public int GoldAmount => Mathf.RoundToInt(goldRange.GetRandomValue()) / goldInBatch;

    public void OnHit()
    {
        spriteRenderer.material.JuicyFloatProperty("_HitEffectBlend", 1f, 0.25f)
            .SetEase(hitEffectCurve)
            .Start();
    }
}
