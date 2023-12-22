using System;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using Spine;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;
using Event = Spine.Event;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiAnimationController : MonoBehaviour, AiAnimatorInterface
    {
        [SerializeField] AnimationReferenceAsset idleAnimation;
        [SerializeField] AnimationReferenceAsset moveAniamtion;
        [SerializeField] AnimationReferenceAsset attackAnimation;
        [SerializeField] AnimationReferenceAsset deathAnimation;
        [SerializeField] AnimationReferenceAsset hitAnimation;
        [SerializeField] SkeletonAnimation skeletonAnimation;
        AiControllerInterface aiController;
        int movementTrackIndex = 0;
        int hitTrackIndex = 1;
        int attackTrackIndex = 2;
        int dynamicTrackIndex = 3;
        public event Action<AttackAnimationEvent> OnAnimationEvent;


        void Awake()
        {
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            aiController = GetComponent<AiControllerInterface>();
            skeletonAnimation.AnimationState.Event += HandleTrackEvent;
        }


      


        public void SetMovementAnimation(bool isMoving)
        {
            var targetTrack = isMoving ? moveAniamtion : idleAnimation;
            var currentTrack = skeletonAnimation.AnimationState.GetCurrent(movementTrackIndex);
            if (currentTrack == null)
                return;

            if (currentTrack.Animation.Name.Equals(targetTrack.Animation.Name) ||
                currentTrack.Animation.Name.Equals(deathAnimation.Animation.Name))
                return;
            //skeletonAnimation.AnimationState.ClearTrack(movementTrackIndex);
            skeletonAnimation.AnimationState.SetAnimation(movementTrackIndex, targetTrack, true);
        }

        public void StartAttackAnimation(AnimationReferenceAsset animationReference, Action onComplete = null)
        {
            var turnTrack = skeletonAnimation.AnimationState.SetAnimation(hitTrackIndex, animationReference, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(hitTrackIndex, 0, 0);
            // turnTrack.AttachmentThreshold = 0f;
            // turnTrack.MixDuration = 0f;
            CoroutineUtility.WaitForSeconds(animationReference.Animation.Duration, () => { onComplete?.Invoke(); });
        }

        public void StartAttackAnimation(Action onComplete = null)
        {
            var turnTrack = skeletonAnimation.AnimationState.SetAnimation(hitTrackIndex, attackAnimation, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(hitTrackIndex, 0, 0);
            // turnTrack.AttachmentThreshold = 1f;
            // turnTrack.MixDuration = .5f;
            CoroutineUtility.WaitForSeconds(attackAnimation.Animation.Duration, () => { onComplete?.Invoke(); });
        }

        public void StopAttackAnimation()
        {
            var track = skeletonAnimation.AnimationState.SetEmptyAnimation(attackTrackIndex, .5f);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        public void PlayDeathAnimation(Action onCompleteAction)
        {
            skeletonAnimation.AnimationState.SetAnimation(movementTrackIndex, deathAnimation, false);
            skeletonAnimation.AnimationState.SetEmptyAnimation(hitTrackIndex, 0);
            skeletonAnimation.AnimationState.SetEmptyAnimation(attackTrackIndex, 0);
            skeletonAnimation.AnimationState.SetEmptyAnimation(dynamicTrackIndex, 0);
            CoroutineUtility.WaitForSeconds(deathAnimation.Animation.Duration, () => { onCompleteAction?.Invoke(); });
        }

        public void PlayHitAnimation(bool interruptAttack, Action onCompleteAction = null)
        {
            var track = skeletonAnimation.AnimationState.GetCurrent(hitTrackIndex);
            if (track != null)
            {
                var playingAttackAnimation = track.Animation.Name.Equals(attackAnimation.Animation.Name);
                if (playingAttackAnimation && interruptAttack || !playingAttackAnimation)
                {
                    var hitTrack = skeletonAnimation.AnimationState.SetAnimation(hitTrackIndex, hitAnimation, false);
                  //  hitTrack.TimeScale = 2f;
                    skeletonAnimation.AnimationState.AddEmptyAnimation(hitTrackIndex, 0, 0);
                    CoroutineUtility.WaitForSeconds(hitAnimation.Animation.Duration / 2f,
                        () => { onCompleteAction?.Invoke(); });
                }
            }
            else
            {
               var hitTrack = skeletonAnimation.AnimationState.SetAnimation(hitTrackIndex, hitAnimation, false);
              //  hitTrack.TimeScale = 2f;
                skeletonAnimation.AnimationState.AddEmptyAnimation(hitTrackIndex, 0, 0);
                CoroutineUtility.WaitForSeconds(hitAnimation.Animation.Duration / 2f,
                    () => { onCompleteAction?.Invoke(); });
            }
        }

        public void PlayDynamicAnimation(AnimationReferenceAsset animationReference, Action onCompleteAction = null)
        {
            skeletonAnimation.AnimationState.SetAnimation(dynamicTrackIndex, animationReference, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(dynamicTrackIndex, 0, 0);
            CoroutineUtility.WaitForSeconds(animationReference.Animation.Duration,
                () => { onCompleteAction?.Invoke(); });
        }

        public void StopDynamicAnimation()
        {
            skeletonAnimation.AnimationState.SetEmptyAnimation(dynamicTrackIndex, 0.1f);
        }

        

        void HandleTrackEvent(TrackEntry trackentry, Event e)
        {
            Debug.Log(e.Data.Name);
            switch (e.Data.Name)
            {
                case AnimationEventNames.AiDamage:
                    OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Attack, 0,
                        AttackType.Regular));
                    break;
                case AnimationEventNames.AiDamageV2:
                    OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Attack, 0,
                        AttackType.Regular));
                    break;

                case AnimationEventNames.Sounds:
                    Debug.Log(e.String);
                    break;
                case AnimationEventNames.VFX:
                    Debug.Log(e.String);
                    break;
                case AnimationEventNames.Shoot:
                    OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Shoot, 0, AttackType.Regular));
                    break;
                case AnimationEventNames.AiDamageV3:
                    OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Shoot, 0, AttackType.Regular));
                    break;
                case AnimationEventNames.AiDamageV4:
                    OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Shoot, 0, AttackType.Regular));
                    break;
            }
        }
    }
}