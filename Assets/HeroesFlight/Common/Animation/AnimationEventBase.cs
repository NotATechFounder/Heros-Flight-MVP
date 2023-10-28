

using HeroesFlight.Common.Enum;

namespace HeroesFlight.Common.Animation
{
    public class AnimationEventBase : AnimationEventInterface
    {
        public AnimationEventBase()
        {
            
        }
        public AnimationEventBase(AniamtionEventType type,int eventIndex) 
        {
            Type = type;
            EventIndex = eventIndex;
        }
        public AniamtionEventType Type { get; protected set; }
        public int EventIndex { get; }
    }
}