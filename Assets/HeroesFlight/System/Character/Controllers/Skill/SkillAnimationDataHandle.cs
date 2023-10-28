
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using Plugins.Audio_System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HeroesFlight.System.Character.Controllers.Skill
{
    public class SkillAnimationDataHandle
    {
        public SkillAnimationDataHandle(List<SkillAnimationData> data)
        {
            GenerateCache(data);
        }


        Dictionary<string, Dictionary<int, AudioClip>> characterAnimationSfxCache = new();

        Dictionary<string, Dictionary<int, ParticleSystem>> characterAnimationVfxCache = new();

        public void HandleAnimationEvent(AnimationEventInterface animationEvent)
        {
            switch (animationEvent.Type)
            {
               
                case AniamtionEventType.Vfx:
                    InvokeVFX(animationEvent as VFXAnimationEvent);
                    break;
                case AniamtionEventType.Sfx:
                    InvokeSFX(animationEvent as SFXAnimationEvent);
                    
                    break;
              
            }
        }

        void GenerateCache(List<SkillAnimationData> data)
        {
            foreach (var dataEntry in data)
            {
                characterAnimationVfxCache.Add(dataEntry.Aniamtion.Animation.Name,
                    new Dictionary<int, ParticleSystem>());
                characterAnimationSfxCache.Add(dataEntry.Aniamtion.Animation.Name, new Dictionary<int, AudioClip>());
                foreach (var entry in dataEntry.SoundEntries)
                {
                    characterAnimationSfxCache[dataEntry.Aniamtion.Animation.Name].Add(entry.Index, entry.Audio);
                }

                foreach (var entry in dataEntry.VfxEntries)
                {
                    var particle = Object.Instantiate(entry.Particle, dataEntry.TargetTransform);
                    characterAnimationVfxCache[dataEntry.Aniamtion.Animation.Name].Add(entry.Index, particle);
                }
            }
        }

        void InvokeVFX(VFXAnimationEvent vfxAnimationEvent)
        {
            if(characterAnimationVfxCache.TryGetValue(vfxAnimationEvent.AnimationName,out var data))
            {
                if (data.TryGetValue(vfxAnimationEvent.EventIndex, out var particleSystem))
                {
                    particleSystem.Play();
                }
            }
        }

        void InvokeSFX(SFXAnimationEvent sfxAnimationEvent)
        {
            if(characterAnimationSfxCache.TryGetValue(sfxAnimationEvent.AnimationName,out var data))
            {
                if (data.TryGetValue(sfxAnimationEvent.EventIndex, out var sound))
                {
                    AudioManager.PlaySoundEffect(sound,SoundEffectCategory.Hero,true);
                }
            }
        }
    }
}