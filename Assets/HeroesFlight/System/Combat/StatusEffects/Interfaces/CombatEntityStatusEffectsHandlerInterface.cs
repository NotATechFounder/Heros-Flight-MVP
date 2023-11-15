using HeroesFlight.System.Combat.StatusEffects.Enum;

namespace HeroesFlight.System.Combat.StatusEffects
{
    public interface CombatEntityStatusEffectsHandlerInterface
    {
        void ApplyEffect();
        void RemoveEffect();
        void TryApplyEffects(StatusEffectApplyType applyType);
    }
}