using HeroesFlight.Common.Enum;


namespace HeroesFlight.Common.Animation
{
    public class AttackAnimationEvent : AnimationEventBase
    {
        public AttackType AttackType { get; }
        public int AttackIndex { get; }

        public AttackAnimationEvent(AttackType attackType,int index) 
        {
            AttackType = attackType;
            AttackIndex = index;
            Type = AniamtionEventType.Attack;
        }
    }
}