using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterItem : MonoBehaviour
{
    public Func<BoosterItem, bool> OnBoosterInteracted;

    [SerializeField] private float launchForce = 10f;
    [SerializeField] private Trigger2DObserver triggerObserver;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoosterSO boosterSO;

    private Rigidbody2D rigid2D;
    private bool isUsed;

    public BoosterSO BoosterSO => boosterSO;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        triggerObserver.OnEnter += OnEnter;
    }

    public void Initialize(BoosterSO booster,Func<BoosterItem, bool>  func)
    {
        isUsed = false;
        boosterSO = booster;
        OnBoosterInteracted = func;
        spriteRenderer.sprite = boosterSO.BoosterSprite;
        ApplyUpWardForce(launchForce);
        Instantiate(booster.BoosterFlare, transform);
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
                isUsed = true;
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        triggerObserver.OnEnter -= OnEnter;
    }
}
