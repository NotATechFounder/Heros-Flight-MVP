using System;
using System.Collections.Generic;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.Stats.Traits.Model;

namespace HeroesFlight.System.Stats.Handlers
{
    public interface TraitSystemInterface : SystemInterface
    {
        event Action<Dictionary<StatAttributeType, int>> OnTraitsStateChange;
        bool HasTraitOfType(TraitType targetType, out List<TraitStateModel> model);
        TraitEffect GetTraitEffect(string id);
        Dictionary<StatAttributeType, int> GetUnlockedEffects();

        void UnlockAllTraits();
    }
}