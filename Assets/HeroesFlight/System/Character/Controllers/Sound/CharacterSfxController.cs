using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using Plugins.Audio_System;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Character.Controllers.Sound
{
    public class CharacterSfxController : MonoBehaviour,AnimationEventHandlerInterface

    {
        CharacterAnimationControllerInterface animationController;
        Dictionary<string, Dictionary<int, AudioClip>> characterAnimationSfxCache = new();
        public void Initialize(CharacterAnimations characterSoCharacterAnimations)
        {
            animationController = GetComponent<CharacterAnimationController>();
            animationController.OnAnimationEvent += HandleAnimationEvent;
          
            GenerateCache(characterSoCharacterAnimations);
        }

      

        void GenerateCache(CharacterAnimations characterSoCharacterAnimations)
        {
            foreach (var data in characterSoCharacterAnimations.AttackAnimationsData)
            {
                characterAnimationSfxCache.Add(data.Aniamtion.Animation.Name, new Dictionary<int, AudioClip>());
                foreach (var entry in data.SoundEntries)
                {
                  
                    characterAnimationSfxCache[data.Aniamtion.Animation.Name].Add(entry.Index, entry.Audio);
                }
            }

            foreach (var data in characterSoCharacterAnimations.UltAnimationsData)
            {
                characterAnimationSfxCache.Add(data.Aniamtion.Animation.Name, new Dictionary<int, AudioClip>());
                foreach (var entry in data.SoundEntries)
                {
                    characterAnimationSfxCache[data.Aniamtion.Animation.Name].Add(entry.Index,  entry.Audio);
                }
            }
        }

        void HandleAnimationEvent(AnimationEventInterface animationEvent)
        {
            if (animationEvent.Type != AniamtionEventType.Sfx)
                return;

            var targetEvent = animationEvent as SFXAnimationEvent;
            if(characterAnimationSfxCache.TryGetValue(targetEvent.AnimationName,out var data))
            {
                if (data.TryGetValue(targetEvent.EventIndex, out var sound))
                {
                    AudioManager.PlaySoundEffect(sound,SoundEffectCategory.Hero,true);
                }
            }
        }
        
        
    }
}