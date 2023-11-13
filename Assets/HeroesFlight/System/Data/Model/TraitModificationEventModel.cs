using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Model;

namespace HeroesFlight.Common.Feat
{
    public class TraitModificationEventModel
    {
        public TraitModificationEventModel(TraitModel target, TraitModificationType type)
        {
            Model = target;
            ModificationType = type;
        }

        public TraitModel Model { get; }
        public TraitModificationType ModificationType { get; }
    }
}