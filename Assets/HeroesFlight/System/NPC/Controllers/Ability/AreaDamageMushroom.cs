using System;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AreaDamageMushroom : AreaDamageEntity
    {
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset activeAniamtion;
        [SerializeField] ParticleSystem[] particleObject;
        [SerializeField] SkeletonAnimation skeletonAnimation;

        public override void Init()
        {
            base.Init();
            skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
        }


        public override void StartDetection(Action onComplete)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, activeAniamtion, true);
            foreach (var particle in particleObject)
            {
                particle.Play(true);
            }
           
            StartDetectionRoutine(() =>
            {
                skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
                foreach (var particle in particleObject)
                {
                    particle.Stop(true);
                }
                onComplete?.Invoke();
            });
        }
    }
}