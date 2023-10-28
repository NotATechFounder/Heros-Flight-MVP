using Pelumi.Juicer;
using Spine;
using Spine.Unity;
using System;
using System.Collections;

using Plugins.Audio_System;
using UnityEngine;

namespace HeroesFlight.System.Environment.Objects
{
    public class Crystal : MonoBehaviour
{
    public Action SpawnLoot;
    public Action<Crystal> OnDestroyed;

    [SerializeField] BoosterDropSO boosterDropSO;
    [SerializeField] AnimationCurve hitEffectCurve;
    [SerializeField] float loopDelay = 0.5f;

    [Header("Animation and Viusal Settings")]
    [SerializeField] public const string idle1Animation = "Idle_1";
    [SerializeField] public const string idle1HittedAnimation = "Idle_1_Hitted";
    [SerializeField] public const string idle2AnimationName = "Idle_2";
    [SerializeField] public const string idle2HittedAnimation = "Idle_2_Hitted";
    [SerializeField] public const string idle3Animation = "Idle_3";
    [SerializeField] public const string idle3HittedAnimation = "Idle_3_Hitted";
    [SerializeField] public const string idle4Animation = "Idle_4";
    [SerializeField] public const string idle4HittedAnimation = "Idle_4_Hitted";
    [SerializeField] public const string idle5Animation = "Idle_5";
    [SerializeField] public const string idle5HittedAnimation = "Idle_5_Hitted";
    [SerializeField] public const string FinalBreakAnimation = "Final_Break";
    [SerializeField] SkeletonAnimation skeletonAnimation;

    [Header("Gold")]
    [SerializeField] RangeValue goldRange;
    [SerializeField] int goldInBatch = 10;

    [Header("Shake")]
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakePower = 0.5f;

    public BoosterDropSO BoosterDropSO => boosterDropSO;
    public int GoldInBatch => goldInBatch;
    public int GoldAmount => Mathf.RoundToInt(goldRange.GetRandomValue()) / goldInBatch;

    private Vector3 lastPos;
    CoroutineHandle shakeRoutine;
    WaitForSeconds waitForSeconds;

    private void Awake()
    {
        lastPos = transform.position;
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        waitForSeconds = new WaitForSeconds(loopDelay);
    }

    IEnumerator SpawnLoopDelay()
    {
        yield return waitForSeconds;
        AudioManager.PlaySoundEffect("CrystalBreak",SoundEffectCategory.Environment);
        SpawnLoot?.Invoke();
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case idle1HittedAnimation:
                skeletonAnimation.AnimationState.SetAnimation(0, idle2AnimationName, true);
                break;
            case idle2HittedAnimation:
                skeletonAnimation.AnimationState.SetAnimation(0, idle3Animation, true);
                break;
            case idle3HittedAnimation:
                skeletonAnimation.AnimationState.SetAnimation(0, idle4Animation, true);
                break;
            case idle4HittedAnimation:
                skeletonAnimation.AnimationState.SetAnimation(0, idle5Animation, true);
                break;
                case idle5HittedAnimation:
                skeletonAnimation.AnimationState.SetAnimation(0, FinalBreakAnimation, false);
                StartCoroutine(SpawnLoopDelay());
                break;
            case FinalBreakAnimation:
                OnDestroyed?.Invoke(this);
                break;
            default:
                break;
        }
    }

    public void OnHit(int normalisedHealth)
    {
        switch (normalisedHealth)
        {
            case 4:
                skeletonAnimation.AnimationState.SetAnimation(0, idle1HittedAnimation, false);
                break;
            case 3:
                skeletonAnimation.AnimationState.SetAnimation(0, idle2HittedAnimation, false);
                break;
            case 2:
                skeletonAnimation.AnimationState.SetAnimation(0, idle3HittedAnimation, false);
                break;
            case 1:
                skeletonAnimation.AnimationState.SetAnimation(0, idle4HittedAnimation, false);
                break;
            case 0:
                skeletonAnimation.AnimationState.SetAnimation(0, idle5HittedAnimation, false);
                break;
            default: break;
        }

        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Shake(randomDirection);
    }

    private void Shake(Vector3 direction)
    {
        direction.y = 0;

        if (shakeRoutine != null && !shakeRoutine.IsDone)
        {
            transform.position = lastPos;
            Juicer.StopCoroutine(shakeRoutine);
        }
        lastPos = transform.position;
        shakeRoutine = transform.JuicyShakePosition(shakeDuration, direction * shakePower, 1);
    }
}
}

