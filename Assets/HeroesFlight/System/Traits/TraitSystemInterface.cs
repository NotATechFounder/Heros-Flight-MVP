using System.Collections.Generic;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.Stats.Traits.Model;

namespace HeroesFlight.System.Stats.Handlers
{
    public interface TraitSystemInterface : SystemInterface
    {
        bool HasTraitOfType(TraitType targetType, out List<TraitStateModel> model);
        TraitEffect GetTraitEffect(string id);
        List<TraitStateModel> GetUnlockedEffects();
    }
}