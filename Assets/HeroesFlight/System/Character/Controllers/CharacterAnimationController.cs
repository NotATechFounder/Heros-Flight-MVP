using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Character.Enum;
using Spine;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;
using Event = Spine.Event;

namespace HeroesFlight.System.Character
{
    public class CharacterAnimationController : MonoBehaviour, CharacterAnimationControllerInterface
    {
        [SerializeField] AnimationReferenceAsset m_IdleAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingUpAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingDownAnimation;
        [SerializeField] AnimationReferenceAsset m_FlyingForwardAnimation;
        [SerializeField] AnimationReferenceAsset m_TurnLeftAnimation;
        [SerializeField] AnimationReferenceAsset m_TurnRightAnimation;
        [SerializeField] AnimationReferenceAsset m_AttackAnimation;
        [SerializeField] AnimationReferenceAsset deathAnimation;
        SkeletonAnimation m_SkeletonAnimation;
        CharacterControllerInterface m_CharacterController;

        bool m_WasFacingLeft;
        Dictionary<CharacterState, AnimationReferenceAsset> m_AnimationsCache = new();

        public event Action<string> OnDealDamageRequest;

        void Awake()
        {
            m_CharacterController = GetComponent<CharacterSimpleController>();
            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
            m_CharacterController.OnCharacterMoveStateChanged += AnimateCharacterMovement;
            m_SkeletonAnimation.AnimationState.Event += HandleTrackEvent;
            CreateAnimationCache();
            m_WasFacingLeft = true;
            m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, 0f);
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

                    var track = m_SkeletonAnimation.AnimationState.AddAnimation(0, stateAnimation, true, 0);
                    track.AttachmentThreshold = 1f;
                    track.MixDuration = .5f;
                }
                else
                {
                    var track = m_SkeletonAnimation.AnimationState.SetAnimation(0, stateAnimation, true);
                    track.AttachmentThreshold = 1f;
                    track.MixDuration = .5f;
                }
            }
        }

        void PlayAttackAnimation()
        {
           
            var track = m_SkeletonAnimation.AnimationState.GetCurrent(1);
            if (track == null || track.Animation.Name.Equals("<empty>"))
            {
                
                var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(1, m_AttackAnimation, false);
                m_SkeletonAnimation.AnimationState.AddEmptyAnimation(1, .5f, 0);
            }
          
        }

        void StopAttackAnimation()
        {
            var track = m_SkeletonAnimation.AnimationState.GetCurrent(1);
          
            if(track!=null && !track.Animation.Name.Equals("<empty>"))
                        m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, .5f);
          
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

        public void PlayDeathAnimation(Action onComplete=null)
        {
            m_SkeletonAnimation.AnimationState.ClearTrack(1);
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0, deathAnimation, false);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
            CoroutineUtility.WaitForSeconds(deathAnimation.Animation.Duration, () =>
            {
                onComplete?.Invoke();
            });
        }

        public void PlayIdleAnimation()
        {
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0, m_IdleAnimation, true);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        public void PlayAttackSequence()
        {
            PlayAttackAnimation();
        }

        public void StopAttackSequence()
        {
            StopAttackAnimation();
        }

        void HandleTrackEvent(TrackEntry trackentry, Event e)
        {
            switch (e.Data.Name)
            {
                case "Dealing damg":
                    OnDealDamageRequest?.Invoke(e.Data.Name);
                    break;
                case "start_sound":
                    AudioManager.PlaySoundEffect("Attack Sound");
                    break;
            }
        }
    }
}