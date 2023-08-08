using HeroesFlight.System.Gameplay.Enum;

namespace HeroesFlight.System.Gameplay.Data.Animation
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