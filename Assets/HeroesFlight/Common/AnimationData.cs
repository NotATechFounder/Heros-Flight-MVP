using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.Common
{
    [Serializable]
    public class AnimationData
    {
        [SerializeField] AnimationReferenceAsset idleAnimation;
        [SerializeField] AnimationReferenceAsset flyingUpAnimation;
        [SerializeField] AnimationReferenceAsset flyingDownAnimation;
        [SerializeField] AnimationReferenceAsset flyingForwardAnimation;
        [SerializeField] AnimationReferenceAsset turnLeftAnimation;
        [SerializeField] AnimationReferenceAsset turnRightAnimation;
        [SerializeField] AnimationReferenceAsset attackAnimation;
        [SerializeField] AnimationReferenceAsset deathAnimation;
        [SerializeField] AnimationReferenceAsset[] ultimateAnimations;

        public AnimationReferenceAsset IdleAniamtion => idleAnimation;
        public AnimationReferenceAsset FlyingUpAnimation => flyingUpAnimation;
        public AnimationReferenceAsset FlyingDownAnimation => flyingDownAnimation;
        public AnimationReferenceAsset FlyingForwardAnimation => flyingForwardAnimation;
        public AnimationReferenceAsset TurnLeftAnimation => turnLeftAnimation;
        public AnimationReferenceAsset TurnRightAnimation => turnRightAnimation;
        public AnimationReferenceAsset AttackAnimation => attackAnimation;
        public AnimationReferenceAsset DeathAnimation => deathAnimation;
        public AnimationReferenceAsset[] UltimateAnimations => ultimateAnimations;
        
    }
}