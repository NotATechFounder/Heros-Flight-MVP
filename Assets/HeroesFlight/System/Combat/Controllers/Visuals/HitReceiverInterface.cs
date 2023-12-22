using HeroesFlight.Common.Enum;

namespace HeroesFlight.System.Combat.Controllers.Visuals
{
    public interface HitReceiverInterface
    {
        void ReactToDamage(AttackType attackType);
    }
}