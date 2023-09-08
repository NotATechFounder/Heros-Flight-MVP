using Spine.Unity;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AreaDamageMushroom : AreaDamageEntity
    {
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset activeAniamtion;
        [SerializeField] ParticleSystem particleObject;
        [SerializeField] SkeletonAnimation skeletonAnimation;

        public override void Init()
        {
            base.Init();
            skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
        }


        public override void StartDetection(Action onComplete)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, activeAniamtion, true);
            particleObject.Play(true);
            StartDetectionRoutine(() =>
            {
                skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
                particleObject.Stop(true);
                onComplete?.Invoke();
            });
        }
    }
}