using System;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiAnimationController : MonoBehaviour, AiAnimatorInterface
    {
        [SerializeField] AnimationReferenceAsset attackAnimation;
        [SerializeField] AnimationReferenceAsset deathAnimation;
        SkeletonAnimation skeletonAnimation;
        AiControllerInterface aiController;
        int movementTrackIndex = 0;
        int dynamicTrackIndex = 1;

        void Awake()
        {
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            aiController = GetComponent<AiControllerInterface>();
        }


        void Update()
        {
            UpdateSkeletonScale();
        }

        void UpdateSkeletonScale()
        {
            var velocity = aiController.GetVelocity();
            skeletonAnimation.Skeleton.ScaleX = velocity.x >= 0 ? 1f : -1f;
        }

    

        public void StartAttackAnimation(Action onCompleteAction)
        {
            var turnTrack = skeletonAnimation.AnimationState.SetAnimation(dynamicTrackIndex, attackAnimation, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(dynamicTrackIndex, .5f, 0);
            turnTrack.AttachmentThreshold = 1f;
            turnTrack.MixDuration = .5f;
            CoroutineUtility.WaitForSeconds(deathAnimation.Animation.Duration, () =>
            {
                onCompleteAction?.Invoke();
            });
        }

        public void StopAttackAnimation()
        {
            var track = skeletonAnimation.AnimationState.SetEmptyAnimation(dynamicTrackIndex, .5f);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        public void PlayDeathAnimation(Action onCompleteAction)
        {
            skeletonAnimation.AnimationState.SetAnimation(movementTrackIndex, deathAnimation, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(dynamicTrackIndex, 0, 0);
            skeletonAnimation.AnimationState.AddEmptyAnimation(3, 0, 0);
            CoroutineUtility.WaitForSeconds(deathAnimation.Animation.Duration, () =>
            {
                onCompleteAction?.Invoke();
            });
        }

        public void PlayAnimation(AnimationReferenceAsset animationReference,Action onCompleteAction=null)
        {
            skeletonAnimation.AnimationState.SetAnimation(3, animationReference, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(3, 0, 0);
            CoroutineUtility.WaitForSeconds(animationReference.Animation.Duration, () =>
            {
                onCompleteAction?.Invoke();
            });
        }

       
    }
}