using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlight.System.Character;
using Spine.Unity;
using Spine;

public class HerculesEffect : MonoBehaviour
{
    public Action OnHitEnemy;

    [SerializeField] private float autoAttackInterval = 2f;

    [Header("Clash")]
    [SerializeField] private Transform visual;
    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] private ParticleSystem hitEffect;

    [Header("Animation and Viusal Settings")]
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string idleAnimationName = "Idle";
    [SerializeField] public const string attackAnimation1Name = "Punch";

    private float damage = 10f;
    private CharacterControllerInterface characterController;
    private float timer;

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        skeletonAnimation.AnimationState.Event += AnimationState_Event;

        skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);

        StartCoroutine(AutoAttack());
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            Attack();
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);
                break;
            default: break;
        }
    }

    public IEnumerator AutoAttack()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= autoAttackInterval)
            {
                timer = 0;

                if (overlapChecker.TargetInRange())
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation1Name, false);
                    Attack();
                }
            }
            yield return null;
        }
    }

    public void Attack()
    {
        overlapChecker.DetectOverlap();
        hitEffect.Play();
    }

    public void SetUp(float damage, CharacterControllerInterface characterControllerInterface, Action OnHitEvent = null)
    {
        this.damage = damage;
        OnHitEnemy = OnHitEvent;
        this.characterController = characterControllerInterface;
        characterController.OnFaceDirectionChange += Flip;
        StartCoroutine(AutoAttack());
    }

    public void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
        overlapChecker.SetDirection(visual.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.TryDealLineDamage(3, 0.25f, new HealthModificationIntentModel(damage,
                    DamageType.Critical, AttackType.Regular, DamageCalculationType.Flat));
                OnHitEnemy?.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        OnHitEnemy = null;
        StopAllCoroutines();
        characterController.OnFaceDirectionChange -= Flip;
    }
}
