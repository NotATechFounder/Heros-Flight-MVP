using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common;
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
        SkeletonAnimation m_SkeletonAnimation;
        AnimationData aniamtionData;
        bool m_WasFacingLeft;
        Dictionary<CharacterState, AnimationReferenceAsset> m_AnimationsCache = new();

        public event Action<string> OnDealDamageRequest;


        public void Init(AnimationData data)
        {
            aniamtionData = data;
            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
            m_SkeletonAnimation.AnimationState.Event += HandleTrackEvent;
            CreateAnimationCache();
            m_WasFacingLeft = true;
            m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, 0f);
            AnimateCharacterMovement(CharacterState.Idle, true);
        }

        void CreateAnimationCache()
        {
            m_AnimationsCache.Add(CharacterState.Idle, aniamtionData.IdleAniamtion);
            m_AnimationsCache.Add(CharacterState.FlyingUp, aniamtionData.FlyingUpAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingDown,aniamtionData.FlyingDownAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingLeft,aniamtionData.FlyingForwardAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingRight,aniamtionData.FlyingForwardAnimation );
        }


        public void AnimateCharacterMovement(CharacterState newState,bool isfacingLeft)
        {
            if (m_AnimationsCache.TryGetValue(newState, out var stateAnimation))
            {
                if (TurnCharacterVisuals(isfacingLeft))
                {
                    m_SkeletonAnimation.Skeleton.ScaleX = m_WasFacingLeft ? 1f : -1f;
                    var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(0,  aniamtionData.TurnLeftAnimation, false);
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

        void PlayAttackAnimation(float speedMultiplier)
        {
           
            var track = m_SkeletonAnimation.AnimationState.GetCurrent(1);
            if (track == null || track.Animation.Name.Equals("<empty>"))
            {
                
                var turnTrack = m_SkeletonAnimation.AnimationState.SetAnimation(1,aniamtionData.AttackAnimation , false);
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
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0,aniamtionData.DeathAnimation , false);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
            CoroutineUtility.WaitForSeconds(aniamtionData.DeathAnimation.Animation.Duration, () =>
            {
                onComplete?.Invoke();
            });
        }

        public void PlayIdleAnimation()
        {
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0,aniamtionData.IdleAniamtion , true);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        public void PlayAttackSequence(float speedMultiplier)
        {
            PlayAttackAnimation(speedMultiplier);
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