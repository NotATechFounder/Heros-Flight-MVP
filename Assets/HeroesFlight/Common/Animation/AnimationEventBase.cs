using HeroesFlight.System.Gameplay.Enum;

namespace HeroesFlight.System.Gameplay.Data.Animation
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