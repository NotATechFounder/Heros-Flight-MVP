using HeroesFlight.Common;
using Pelumi.Juicer;
using Spine;
using Spine.Unity;
using StansAssets.Foundation.Async;
using System;
using UnityEngine;

public class CurrencyItem : MonoBehaviour
{
    public Action<CurrencySO, float> OnCurrencyInteracted;

    [SerializeField] public float waitTime = 1f;
    [SerializeField] public float amount;
    [SerializeField] private CurrencySO currencySO;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] public string loopAnimationName = "1_coin_loop";
    [SerializeField] public string gettingAnimationName = "2_coin_getting";
    [SerializeField] SkeletonAnimation skeletonAnimation;

    private Rigidbody2D rigid2D;
    private JuicerRuntime moveEffect;
    [SerializeField] private Transform target;

    public CurrencySO CurrencySO => currencySO;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        moveEffect = transform.JuicyMove (Vector2.zero, .5f);
        moveEffect.SetOnComplected(() =>
        {
            skeletonAnimation.AnimationState.SetAnimation(0, gettingAnimationName, false);
        });

        skeletonAnimation.AnimationState.SetAnimation(0, loopAnimationName, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Initialize(null, null);
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == gettingAnimationName)
        {
            OnCurrencyInteracted?.Invoke(currencySO, amount);
            Destroy(gameObject);
        }
    }

    public void Initialize(CurrencySO currency, Action<CurrencySO, float> func)
    {
        currencySO = currency;
        OnCurrencyInteracted = func;

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

    public void MoveToPlayer()
    {
        moveEffect.ChangeDesination(target.position);
        moveEffect.Start();
    }
}
