

using HeroesFlight.Common.Enum;

namespace HeroesFlight.Common.Animation
{
    public class AnimationEventBase : AnimationEventInterface
    {
        public AnimationEventBase()
        {
            
        }
        public AnimationEventBase(AniamtionEventType type)
        {
            Type = type;
        }
        public AniamtionEventType Type { get; protected set; }
    }
}