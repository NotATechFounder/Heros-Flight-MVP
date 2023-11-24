using System;
using System.Collections.Generic;

namespace HeroesFlight.System.Stats.Traits.Model
{
    [Serializable]
    public class TraitsMapSaveModel
    {
        public List<TraitSaveModel> savedModels = new();
    }
}