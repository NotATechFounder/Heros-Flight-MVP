using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kage : MonoBehaviour
{
    [Header("Animation and Viusal Settings")]
    [SerializeField] private Transform visual;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] private OverlapChecker overlapChecker;

    private int damage;

    private void Awake()
    {
        overlapChecker.OnDetect += OnOverlap;
        skeletonAnimation.AnimationState.Event += AnimationState_Event;
      skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            overlapChecker.DetectOverlap();
        }
    }

    public void Init(int damage, string animation)
    {
        this.damage = damage;
        skeletonAnimation.AnimationState.SetAnimation(0, animation, false);
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(damage,
                DamageCritType.Critical, AttackType.Regular, CalculationType.Flat, null));
            }
        }
    }
}
