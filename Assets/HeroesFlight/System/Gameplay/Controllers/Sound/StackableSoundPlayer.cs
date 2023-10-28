using System.Collections;
using Plugins.Audio_System;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Controllers.Sound
{
    public class StackableSoundPlayer : MonoBehaviour, HitEffectPlayerInterface
    {
        [SerializeField] int maxHitSounds = 5;
        [SerializeField] float resetThreshhold = 0.5f;
        [SerializeField] int currentSoundsPlaying;
        WaitForSeconds resetTime;

        void Awake()
        {
            resetTime = new WaitForSeconds(resetThreshhold);
        }

        public void PlayHitEffect(string soundName, bool randomPitch = false)
        {
            if (currentSoundsPlaying >= maxHitSounds)
                return;
            StartCoroutine(PlaySound(soundName, randomPitch));
        }

        public void PlayHitEffect(AudioClip sound, bool randomPitch = false)
        {
            if (currentSoundsPlaying >= maxHitSounds)
                return;
            StartCoroutine(PlaySound(sound, randomPitch));
        }

        IEnumerator PlaySound(string soundName, bool randomPitch = false)
        {
            AudioManager.PlaySoundEffect(soundName,SoundEffectCategory.Combat, randomPitch);
            currentSoundsPlaying++;
            yield return resetTime;
            currentSoundsPlaying--;
        }

        IEnumerator PlaySound(AudioClip sound, bool randomPitch = false)
        {
            AudioManager.PlaySoundEffect(sound,SoundEffectCategory.Combat, randomPitch);
            currentSoundsPlaying++;
            yield return resetTime;
            currentSoundsPlaying--;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}