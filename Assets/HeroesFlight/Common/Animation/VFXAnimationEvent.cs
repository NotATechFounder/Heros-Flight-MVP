using HeroesFlight.Common.Enum;

namespace HeroesFlight.Common.Animation
{
    public class VFXAnimationEvent : AnimationEventBase
    {
        public VFXAnimationEvent(AniamtionEventType type, int eventIndex, string animationName, float speedMultiplier) : base(type,eventIndex)
        {
            AnimationName = animationName;
            SpeedMultiplier = speedMultiplier;
        }
        public string AnimationName { get; }
        public float SpeedMultiplier { get; }
        
    }
}