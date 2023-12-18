using HeroesFlight.Common.Enum;


namespace HeroesFlight.Common.Animation
{
    public class AttackAnimationEvent : AnimationEventBase
    {
        public AttackType AttackType { get; }
        public int AttackIndex { get; }

     
        public AttackAnimationEvent(AniamtionEventType eventType,int index,AttackType attackType) :base(eventType,index)
        {
            AttackType = attackType;
            AttackIndex = index;
            Type = eventType;
        }
    }
}