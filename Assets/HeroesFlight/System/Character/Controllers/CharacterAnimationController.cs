using System;
using System.Collections.Generic;
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
        SkeletonAnimation m_SkeletonAnimation;
        ICharacterController m_CharacterController;
        bool m_WasFacingLeft;
        Dictionary<CharacterState, AnimationReferenceAsset> m_AnimationsCache = new();

        void Awake()
        {
            m_CharacterController = GetComponent<CharacterSimpleController>();
            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
            m_CharacterController.OnCharacterMoveStateChanged += AnimateCharacterMovement;
            CreateAnimationCache();
            m_WasFacingLeft = false;
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
           Debug.Log(newState);
            if (m_AnimationsCache.TryGetValue(newState, out var stateAnimation))
            {
                if (TurnCharacterVisuals(m_CharacterController.IsFacingLeft))
                {
                    m_SkeletonAnimation.Skeleton.ScaleX = m_CharacterController.IsFacingLeft ? 1f : -1f;
                    // var targetAnimation =
                    //     m_CharacterController.IsFacingLeft ? m_TurnRightAnimation : m_TurnLeftAnimation;
                    // Debug.Log(targetAnimation);
                    // var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(0, targetAnimation, false);
                    // turnTrack.AttachmentThreshold = 1f;
                    // turnTrack.MixDuration = 0f;


                   var track= m_SkeletonAnimation.AnimationState.SetAnimation(0, stateAnimation, true);
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

        bool TurnCharacterVisuals(bool facingLeft)
        {
            if (m_WasFacingLeft != facingLeft)
            {
                m_WasFacingLeft = facingLeft;
                return true;
            }

            return false;


            // Maybe play a transient turning animation too, then call ChangeStableAnimation.
        }
    }
}