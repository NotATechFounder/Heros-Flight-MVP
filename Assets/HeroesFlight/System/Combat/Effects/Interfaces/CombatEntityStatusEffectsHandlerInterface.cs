using HeroesFlight.System.Combat.StatusEffects.Effects;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.Combat.StatusEffects
{
    public interface CombatEntityStatusEffectsHandlerInterface
    {
        void AddCombatEffect(CombatEffectData data);
        void RemoveEffect(CombatEffectData data);

        void TryReceiveStatusEffect(StatusEffect effect);
        void TryApplyEffects(StatusEffectApplyType applyType,IHealthController target);
    }
}