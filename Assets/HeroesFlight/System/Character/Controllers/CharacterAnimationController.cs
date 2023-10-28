using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character.Enum;
using Plugins.Audio_System;
using Spine;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;
using Event = Spine.Event;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Character
{
    public class CharacterAnimationController : MonoBehaviour, CharacterAnimationControllerInterface
    {
        SkeletonAnimation m_SkeletonAnimation;
        CharacterAnimations m_AnimationData;
        bool m_WasFacingLeft;
        Dictionary<CharacterState, AnimationReferenceAsset> m_AnimationsCache = new();

        public event Action<AnimationEventInterface> OnAnimationEvent;
        public event Action<bool> OnAttackAnimationStateChange;
        float speedMultiplier;

        public void Init(CharacterAnimations data)
        {
            m_AnimationData = data;
            m_SkeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            m_SkeletonAnimation.AnimationState.Event += HandleTrackEvent;
            CreateAnimationCache();
            m_WasFacingLeft = true;
            m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, 0f);
            AnimateCharacterMovement(CharacterState.Idle, true);
        }

        void CreateAnimationCache()
        {
            m_AnimationsCache.Add(CharacterState.Idle, m_AnimationData.IdleAniamtion);
            m_AnimationsCache.Add(CharacterState.FlyingUp, m_AnimationData.FlyingUpAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingDown, m_AnimationData.FlyingDownAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingLeft, m_AnimationData.FlyingForwardAnimation);
            m_AnimationsCache.Add(CharacterState.FlyingRight, m_AnimationData.FlyingForwardAnimation);
        }


        public void AnimateCharacterMovement(CharacterState newState, bool isfacingLeft)
        {
            if (m_AnimationsCache.TryGetValue(newState, out var stateAnimation))
            {
                if (TurnCharacterVisuals(isfacingLeft))
                {
                    m_SkeletonAnimation.Skeleton.ScaleX = m_WasFacingLeft ? 1f : -1f;
                    var turnTrack =
                        m_SkeletonAnimation.AnimationState.SetAnimation(0, m_AnimationData.TurnLeftAnimation, false);
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

        float PlayAttackAnimation(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;
            var track = m_SkeletonAnimation.AnimationState.GetCurrent(1);
            if (track == null || track.Animation.Name.Equals("<empty>"))
            {
                OnAttackAnimationStateChange?.Invoke(true);
                var attackAnimation = m_AnimationData.AttackAnimationsData
                    .ElementAt(Random.Range(0, m_AnimationData.AttackAnimationsData.Count)).Aniamtion;
                var turnTrack =
                    m_SkeletonAnimation.AnimationState.SetAnimation(1, attackAnimation, false);
                m_SkeletonAnimation.AnimationState.AddEmptyAnimation(1, .1f, 0);
                turnTrack.TimeScale = speedMultiplier;
                OnAnimationEvent?.Invoke(new VFXAnimationEvent(AniamtionEventType.Vfx,0, attackAnimation.Animation.Name,speedMultiplier));
                return attackAnimation.Animation.Duration;
            }

            return 0;
        }

        void StopAttackAnimation()
        {
            var track = m_SkeletonAnimation.AnimationState.GetCurrent(1);

            if (track != null && !track.Animation.Name.Equals("<empty>"))
                m_SkeletonAnimation.AnimationState.SetEmptyAnimation(1, .2f);
            
            OnAttackAnimationStateChange?.Invoke(false);
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

        public void PlayDeathAnimation(Action onComplete = null)
        {
            m_SkeletonAnimation.AnimationState.ClearTrack(1);
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0, m_AnimationData.DeathAnimation, false);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
            CoroutineUtility.WaitForSeconds(m_AnimationData.DeathAnimation.Animation.Duration, () =>
            {
                onComplete?.Invoke();
            });
        }

        public void PlayIdleAnimation()
        {
            var track = m_SkeletonAnimation.AnimationState.SetAnimation(0, m_AnimationData.IdleAniamtion, true);
            track.AttachmentThreshold = 1f;
            track.MixDuration = .5f;
        }

        public float PlayAttackSequence(float speedMultiplier)
        {
          return   PlayAttackAnimation(speedMultiplier);
        }

        public void StopAttackSequence()
        {
            StopAttackAnimation();
        }

        public void PlayAnimationSequence(List<AnimationReferenceAsset> animations, Action onCompleteAction = null)
        {
            StopAttackAnimation();
            float duration = 0;
            for (var i = 0; i < animations.Count; i++)
            {
                if (i == 0)
                {
                    m_SkeletonAnimation.AnimationState.SetAnimation(2, animations[i], false);
                    OnAnimationEvent?.Invoke(new VFXAnimationEvent(AniamtionEventType.Vfx,0, animations[i].Animation.Name,1));
                }
                else
                {
                    m_SkeletonAnimation.AnimationState.AddAnimation(2, animations[i].Animation, false, 0);
                }

                duration += animations[i].Animation.Duration;
            }
         
            CoroutineUtility.WaitForSeconds(duration, () =>
            {
                m_SkeletonAnimation.AnimationState.SetEmptyAnimation(2, 0f);
                onCompleteAction?.Invoke();
            });
        }

        void HandleTrackEvent(TrackEntry trackentry, Event e)
        {
            switch (e.Data.Name)
            {
                case AnimationEventNames.Damage:
                    switch (e.String)
                    {
                        case AnimationEventValues.NormalAttack:
                            OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Attack,e.Int, AttackType.Regular));
                            break;
                        case AnimationEventValues.UltimateAttack:
                            OnAnimationEvent?.Invoke(new AttackAnimationEvent(AniamtionEventType.Attack, e.Int,AttackType.Ultimate));
                            AudioManager.PlaySoundEffect("Attack Sound",SoundEffectCategory.Hero);
                            break;
                    }

                    break;
                case AnimationEventNames.Sounds:
                    OnAnimationEvent?.Invoke(new SFXAnimationEvent(AniamtionEventType.Sfx,e.Int, trackentry.Animation.Name));
                    break;
                case AnimationEventNames.VFX:
                    //OnAnimationEvent?.Invoke(new VFXAnimationEvent(AniamtionEventType.Vfx,e.Int, trackentry.Animation.Name,speedMultiplier));
                    break;
            }
        }

        public void StopUltSequence()
        {
            m_SkeletonAnimation.AnimationState.SetEmptyAnimation(2, 0f);
        }
    }
}