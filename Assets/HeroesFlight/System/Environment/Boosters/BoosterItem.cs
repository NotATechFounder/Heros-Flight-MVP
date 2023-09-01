using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using StansAssets.Foundation.Async;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BoosterItem : MonoBehaviour
{
    public Func<BoosterItem, bool> OnBoosterInteracted;

    [SerializeField] private float launchForce = 10f;
    [SerializeField] private Trigger2DObserver triggerObserver;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoosterSO boosterSO;
    [SerializeField] float amplitude = 0.2f;
    [SerializeField] float period = 1.5f;
    float timeCustomizer;
    private Rigidbody2D rigid2D;
    private bool isUsed;
    Coroutine floatingRoutine;
    Transform particle;
    public BoosterSO BoosterSO => boosterSO;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        timeCustomizer=Random.Range(-5, 5);
        var rng = Random.Range(0, 0.1f);
        amplitude += rng;
    }

    private void Start()
    {
        triggerObserver.OnEnter += OnEnter;
    }

    public void Initialize(BoosterSO booster, Func<BoosterItem, bool> func)
    {
        isUsed = false;
        boosterSO = booster;
        OnBoosterInteracted = func;
        spriteRenderer.sprite = boosterSO.BoosterSprite;
        ApplyUpWardForce(launchForce);

        particle = ObjectPoolManager.SpawnObject(booster.BoosterFlare, transform).transform;
        floatingRoutine = StartCoroutine(FloatingRoutine());     
    }

    public void ApplyUpWardForce(float force)
    {
        Vector2 lauchPos = new Vector2(UnityEngine.Random.Range(-5f, 5f), force);
        rigid2D.AddForce(lauchPos, ForceMode2D.Impulse);
    }

    private void OnEnter(Collider2D collider)
    {
        if (OnBoosterInteracted != null && !isUsed)
        {
            if (OnBoosterInteracted.Invoke(this))
            {
                Release();
            }
        }
    }

    IEnumerator FloatingRoutine()
    {
        while (true)
        {
            var modifiedTime = Time.time + timeCustomizer;
            spriteRenderer.transform.localPosition = new Vector3(0, Mathf.Sin(modifiedTime*period) * amplitude , 0);
            particle.localPosition = new Vector3(0, Mathf.Sin(modifiedTime*period) * amplitude  , 0);
            yield return null;
        }
    }

    public void Release()
    {
        isUsed = true;
        StopCoroutine(floatingRoutine);
        ObjectPoolManager.ReleaseObject(particle);
        particle = null;
        ObjectPoolManager.ReleaseObject(this);
    }
}