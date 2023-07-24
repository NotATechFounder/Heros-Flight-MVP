using Spine.Unity;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiAnimationController : MonoBehaviour, AiAnimatorInterface
    {
        [SerializeField] AnimationReferenceAsset attackAnimation;
        SkeletonAnimation skeletonAnimation;
        AiControllerInterface aiController;

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

        public void StartAttackAnimation()
        {
            var turnTrack = skeletonAnimation.AnimationState.SetAnimation(1, attackAnimation, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(1, .5f, 0);
            turnTrack.AttachmentThreshold = 1f;
            turnTrack.MixDuration = .5f;
        }

        public void StopAttackAnimation()
        {
            var track = skeletonAnimation.AnimationState.SetEmptyAnimation(1, .5f);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }
    }
}