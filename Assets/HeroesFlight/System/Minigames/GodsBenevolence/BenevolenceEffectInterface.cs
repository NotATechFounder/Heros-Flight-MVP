using System;

namespace HeroesFlight.System.Minigames.GodsBenevolence
{
    public interface BenevolenceEffectInterface
    {
        void SetUp(float damage, Action OnHitEvent = null);
    }
}