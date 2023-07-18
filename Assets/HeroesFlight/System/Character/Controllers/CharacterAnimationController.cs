using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.System.Character.Enum;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] AnimationReferenceAsset m_IdleAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingUpAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingDownAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingForwardAnimation;
        [SerializeField] AnimationReferenceAsset m_TurnLeftAnimation;
        [SerializeField] AnimationReferenceAsset m_TurnRightAnimation;
        [SerializeField] AnimationReferenceAsset m_AttackAnimation;
        SkeletonAnimation m_SkeletonAnimation;
        ICharacterController m_CharacterController;
        IAttackController m_AttackController;
        bool m_WasFacingLeft;
        Dictionary<CharacterState, AnimationReferenceAsset> m_AnimationsCache = new();

        void Awake()
        {
            m_CharacterController = GetComponent<CharacterSimpleController>();
            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
            m_AttackController = GetComponent<CharacterAttackController>();
            m_CharacterController.OnCharacterMoveStateChanged += AnimateCharacterMovement;
            m_AttackController.OnAttackTarget += PlayAttackAnimation;
            m_AttackController.OnStopAttack += StopAttackAnimation;
            CreateAnimationCache();
            m_WasFacingLeft = true;
        }

        void CreateAnimationCache()
        {
            m_AnimationsCache.Add(CharacterState.Idle, m_IdleAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingUp, m_FlyingUpAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingDown, m_FlyingDownAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingLeft, m_FlyingForwardAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingRight, m_FlyingForwardAnimation);
        }


        void AnimateCharacterMovement(CharacterState newState)
        {
            if (m_AnimationsCache.TryGetValue(newState, out var stateAnimation))
            {
                if (TurnCharacterVisuals(m_CharacterController.IsFacingLeft))
                {
                    m_SkeletonAnimation.Skeleton.ScaleX = m_WasFacingLeft ? 1f : -1f;
                    var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(0, m_TurnLeftAnimation, false);
                    turnTrack.AttachmentThreshold = 1f;
                    turnTrack.MixDuration = 0f;

                    var track= m_SkeletonAnimation.AnimationState.AddAnimation(0, stateAnimation, true,0);
                    track.AttachmentThreshold = 1f;
                    track.MixDuration = .5f;
                }
                else
                {
                    var track= m_SkeletonAnimation.AnimationState.SetAnimation(0, stateAnimation, true);
                    track.AttachmentThreshold = 1f;
                    track.MixDuration = .5f;
                  
                }
            }


           
        }

        void PlayAttackAnimation(IHealthController target)
        {
            var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(1, m_AttackAnimation, false);
            m_SkeletonAnimation.AnimationState.AddEmptyAnimation(1, .5f, 0);
            turnTrack.AttachmentThreshold = 1f;
            turnTrack.MixDuration = .5f;
        }

        void StopAttackAnimation()
        {
            var track= m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, .5f);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        bool TurnCharacterVisuals(bool facingLeft)
        {
           
            if (m_WasFacingLeft != facingLeft)
            {
                m_WasFacingLeft = facingLeft;
                return true;
            }

            return false;
        }
    }
}