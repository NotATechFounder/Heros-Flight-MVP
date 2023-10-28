using HeroesFlight.Common.Enum;

namespace HeroesFlight.Common.Animation
{
    public class SFXAnimationEvent : AnimationEventBase
    {
        public SFXAnimationEvent(AniamtionEventType type, int eventIndex, string animationName) : base(type,eventIndex)
        {
            AnimationName = animationName;
           
        }
        public string AnimationName { get; }
       
    }
}