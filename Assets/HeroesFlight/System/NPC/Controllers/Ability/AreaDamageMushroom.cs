using Spine.Unity;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AreaDamageMushroom : AreaDamageEntity
    {
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset activeAniamtion;
        [SerializeField] GameObject particleObject;
        SkeletonAnimation skeletonAnimation;

        public override void Init()
        {
            base.Init();
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
        }


        public override void StartDetection(Action onComplete)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
            particleObject.SetActive(true);
            base.StartDetection(() =>
            {
                skeletonAnimation.AnimationState.SetAnimation(0, activeAniamtion, true);
                particleObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
    }
}