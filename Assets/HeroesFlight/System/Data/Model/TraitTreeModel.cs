using System.Collections.Generic;

namespace HeroesFlight.System.FileManager.Model
{
    public class TraitTreeModel
    {
        public TraitTreeModel(int maxTier, int rowsPerTier, Dictionary<string, TraitModel> data)
        {
            MaxTier = maxTier;
            RowsPerTier = rowsPerTier;
            Data = data;
        }


        public int MaxTier { get; }
        public int RowsPerTier { get; }
        public Dictionary<string, TraitModel> Data { get; }
    }
}