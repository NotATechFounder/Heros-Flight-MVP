using UnityEngine;

namespace HeroesFlight.System.Gameplay.Controllers.Sound
{
    public interface HitEffectPlayerInterface
    {
        void PlayHitEffect(string soundName, bool randomPitch = false);
        void PlayHitEffect(AudioClip sound, bool randomPitch = false);
    }
}