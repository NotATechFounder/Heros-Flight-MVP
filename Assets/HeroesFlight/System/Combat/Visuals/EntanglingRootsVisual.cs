using Spine.Unity;
using UnityEngine;

namespace HeroesFlight.System.Combat.Visuals
{
    public class EntanglingRootsVisual : MonoBehaviour
    {
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset startAnimation;
        [SerializeField] AnimationReferenceAsset endAnimation;
        [SerializeField] SkeletonAnimation skeletonAnimation;



        public void ShowRootsVisual()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, startAnimation, false);
            skeletonAnimation.AnimationState.AddAnimation(0, endAnimation, false, 0.2f);
            skeletonAnimation.gameObject.SetActive(true);
        }
    }
}