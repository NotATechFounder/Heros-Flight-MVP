using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.Common
{
    [Serializable]
    public class CharacterAnimations
    {
        [Header("Movement Animations")]
        [SerializeField] AnimationReferenceAsset idleAnimation;
        [SerializeField] AnimationReferenceAsset flyingUpAnimation;
        [SerializeField] AnimationReferenceAsset flyingDownAnimation;
        [SerializeField] AnimationReferenceAsset flyingForwardAnimation;
        [SerializeField] AnimationReferenceAsset turnLeftAnimation;
        [SerializeField] AnimationReferenceAsset turnRightAnimation;
        [Header("Combat animations")]
        [SerializeField] List<AnimationData> ultAnimations = new();
        [SerializeField] List<AnimationData> attackAniamtions = new();
        [Header("Death Animation")]
        [SerializeField] AnimationReferenceAsset deathAnimation;

      

        public AnimationReferenceAsset IdleAniamtion => idleAnimation;
        public AnimationReferenceAsset FlyingUpAnimation => flyingUpAnimation;
        public AnimationReferenceAsset FlyingDownAnimation => flyingDownAnimation;
        public AnimationReferenceAsset FlyingForwardAnimation => flyingForwardAnimation;
        public AnimationReferenceAsset TurnLeftAnimation => turnLeftAnimation;
        public AnimationReferenceAsset TurnRightAnimation => turnRightAnimation;
        public AnimationReferenceAsset DeathAnimation => deathAnimation;
        public List<AnimationData> AttackAnimationsData => attackAniamtions;
        public List<AnimationData> UltAnimationsData => ultAnimations;

    }
}