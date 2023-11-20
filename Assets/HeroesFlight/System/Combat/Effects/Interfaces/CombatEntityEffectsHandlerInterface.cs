using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Combat.StatusEffects;

namespace HeroesFlight.System.Combat.Effects
{
    public interface CombatEntityEffectsHandlerInterface : StatusEffectsHandlerInterface
    {
        void AddCombatEffect(CombatEffect effect,int lvl);
        void RemoveEffect(CombatEffect data);

        void TriggerCombatEffect(CombatEffectApplicationType onDealDamage, HealthModificationRequestModel requestModel);
    }
}