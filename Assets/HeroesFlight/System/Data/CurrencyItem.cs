
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using StansAssets.Foundation.Async;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CurrencyItem : MonoBehaviour
{
    public Action<CurrencyItem, float> OnCurrencyInteracted;

    [SerializeField] public float waitTime = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] protected ParticleSystem sparkParticle;

    private CurrencySO currencySO;
    private Rigidbody2D rigid2D;
    private JuicerRuntime scaleEffect;
    private Transform target;
    public float amount;

    public CurrencySO CurrencySO => currencySO;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        scaleEffect = transform.JuicyScale(0, .15f);
        scaleEffect.SetOnComplected(() =>
        {
            OnCurrencyInteracted?.Invoke(this, amount);
            ObjectPoolManager.ReleaseObject(this);
        });
    }

    public void Initialize(CurrencySO currency, Action<CurrencyItem, float> func , float amount, Transform target)
    {
        transform.localScale = Vector3.one;
        currencySO = currency;
        spriteRenderer.sprite = currency.GetSprite;
        OnCurrencyInteracted = func;
        this.amount = amount;
        this.target = target;

        ColorOverLifetimeModule colorOverLifetime = particle.colorOverLifetime;
        colorOverLifetime.color = currency.GetGradient;


        ColorOverLifetimeModule sparkColorOverLifetime = sparkParticle.colorOverLifetime;
        sparkColorOverLifetime.color = currency.GetSparkGradient;

        ApplyUpWardForce(launchForce);

        CoroutineUtility.WaitForSeconds(waitTime, () =>
        {
            MoveToPlayer();
        });
    }

    public void ApplyUpWardForce(float force)
    {
        Vector2 lauchPos = new Vector2(UnityEngine.Random.Range(-5f, 5f), force);
        rigid2D.AddForce(lauchPos, ForceMode2D.Impulse);
    }

    public IEnumerator MoveToPosition(Action OnReachPlayer)
    {
        yield return null;
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / speed;
            transform.position = Vector3.Lerp(currentPos, target.position, t);
            yield return null;
        }

        if (t >= 1) OnReachPlayer.Invoke();
    }

    public void MoveToPlayer()
    {
       StartCoroutine(MoveToPosition(() =>
       {
           scaleEffect.Start();
       }));
    }
}
