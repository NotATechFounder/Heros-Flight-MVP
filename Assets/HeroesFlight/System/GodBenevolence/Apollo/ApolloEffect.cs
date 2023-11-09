using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
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

public class ApolloEffect : MonoBehaviour
{
    [SerializeField] private float autoAttackSpeed = 3f;
    [SerializeField] private float linesOfDamage = 3;
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;

    [SerializeField] private Transform visual;
    [SerializeField] private ParticleSystem attackEffect;
    [SerializeField] private OverlapChecker[] overlapCheckers;
    [SerializeField] private OverlapChecker detector;

    [Header("Animation and Viusal Settings")]
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string idleAnimationName = "Idle";
    [SerializeField] public const string attackAnimation1Name = "Attack_Multi_Bow";
    [SerializeField] public const string attackAnimation2Name = "Attack_One_Bow";

    private CharacterControllerInterface characterController;
    private float timer;
    private float damage;

    private void Start()
    {
        foreach (var overlapChecker in overlapCheckers)
        {
            overlapChecker.OnDetect = OnDamgeOverlap;
        }

        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;

         StartCoroutine(AutoAttack());
    }

    public void SetUp(float damage, CharacterControllerInterface characterControllerInterface)
    {
        this.damage = damage;
        this.characterController = characterControllerInterface;
        characterController.OnFaceDirectionChange += Flip;
        StartCoroutine(AutoAttack());
    }

    private void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);

        foreach (var overlapChecker in overlapCheckers)
        {
            overlapChecker.SetDirection(visual.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
        }

        detector.SetDirection(visual.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
    }

    public IEnumerator AutoAttack()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= autoAttackSpeed)
            {
                timer = 0;
                if (detector.TargetInRange())
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation1Name, false);
                    attackEffect.Play();
                    foreach (var overlapChecker in overlapCheckers)
                    {
                        overlapChecker.DetectOverlap();
                    }
                }
            }
            yield return null;
        }
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {

        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);
            break;
            case attackAnimation2Name:
                    skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);
            break;
            default: break;
        }
    }

    private void OnDamgeOverlap(int count, Collider2D[] colliders)
    {
        StartCoroutine(LineDamage(count, colliders));
    }

    public IEnumerator LineDamage(int count, Collider2D[] colliders)
    {
        yield return new WaitForSeconds(firstAttackDelay);
        float currentDamage = damage / linesOfDamage;
        for (int i = 0; i < linesOfDamage; i++)
        {
            for (int z = 0; z < count; z++)
            {
                if (colliders[z].TryGetComponent(out IHealthController healthController))
                {
                    healthController.TryDealDamage(new HealthModificationIntentModel(damage,
                        DamageType.Critical, AttackType.Regular, DamageCalculationType.Flat));
                }
            }
            yield return new WaitForSeconds(lineDamageDelay);
        }
    }


    private void OnDisable()
    {
        characterController.OnFaceDirectionChange -= Flip;
        StopAllCoroutines();
    }
}
