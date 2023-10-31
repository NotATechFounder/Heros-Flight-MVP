using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using System;
using System.Collections;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using UnityEngine;
using Spine.Unity;
using Spine;

public class AresEffect : MonoBehaviour
{
    public Action OnHitEnemy;

    [SerializeField] private float autoAttackInterval = 2f;
    [SerializeField] private float damage = 10f;

    [Header("Clash")]
    [SerializeField] private Transform visual;
    [SerializeField] private OverlapChecker overlapChecker;
 
    [Header("Animation and Viusal Settings")]
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string idleAnimationName = "Idle";
    [SerializeField] public const string attackAnimation1Name = "Attack";
    [SerializeField] public const string attack2Animation1Name = "Attack2";

    private CharacterControllerInterface characterController;
    private float timer;
    int lastAttackIndex = 0;

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        skeletonAnimation.AnimationState.Event += AnimationState_Event;
    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            overlapChecker.Detect();
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);
            break;
            case attack2Animation1Name:
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

                if(overlapChecker.TargetInRange())
                {
                    if (lastAttackIndex == 0)
                    {
                        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation1Name, false);
                        lastAttackIndex = 1;
                    }
                    else
                    {
                        skeletonAnimation.AnimationState.SetAnimation(0, attack2Animation1Name, false);
                        lastAttackIndex = 0;
                    }
                    overlapChecker.Detect();
                }
            }
            yield return null;
        }
    }

    public void SetUp(float damage, CharacterControllerInterface characterControllerInterface, Action OnHitEvent=null)
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
            if (colliders[i].TryGetComponent( out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(damage, 
                    DamageType.Critical, AttackType.Regular,DamageCalculationType.Flat));
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
