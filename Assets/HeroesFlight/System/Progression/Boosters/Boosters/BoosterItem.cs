using Pelumi.ObjectPool;
using System;
using UnityEngine;
using Spine;
using Spine.Unity;

public class BoosterItem : MonoBehaviour
{
    public Func<BoosterItem, bool> OnBoosterInteracted;

    [SerializeField] private float launchForce = 10f;
    [SerializeField] private Trigger2DObserver triggerObserver;
    [SerializeField] SkeletonAnimation skeletonAnimation;

    [Header("Animation and Viusal Settings")]
    [SerializeField] public const string idleAnimationName = "Idle";
    [SerializeField] public const string flyingRotateName = "Flying_Rotate";
    [SerializeField] public const string pickUpAnimationName = "Pickup";
    [SerializeField] public const string rotateAnimationName = "Rotate";

    [SerializeField] private BoosterSO boosterSO;

    private Rigidbody2D rigid2D;
    private bool isUsed;
    public BoosterSO BoosterSO => boosterSO;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        triggerObserver = GetComponentInChildren<Trigger2DObserver>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    private void Start()
    {
        triggerObserver.OnEnter += OnEnter;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
    }

    public void Initialize(BoosterSO booster, Func<BoosterItem, bool> func)
    {
        isUsed = false;
        boosterSO = booster;
        OnBoosterInteracted = func;

        skeletonAnimation.skeleton.SetSkin(booster.SkinReference);
        skeletonAnimation.AnimationState.SetAnimation(0, flyingRotateName, true);

        ApplyUpWardForce(launchForce);
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
                skeletonAnimation.AnimationState.SetAnimation(0, pickUpAnimationName, false);
                Release();
            }
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case pickUpAnimationName: Release(); break;
            default: break;
        }
    }

    public void Release()
    {
        isUsed = true;
        ObjectPoolManager.ReleaseObject(this);
    }
}