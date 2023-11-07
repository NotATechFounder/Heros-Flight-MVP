using System;

namespace HeroesFlight.System.Stats.Traits.Model
{
    public class TraitStateModel
    {
        public TraitStateModel(Trait trait, IntValue value)
        {
            TargetTrait = trait;
            Value = value;
        }
        public Trait TargetTrait { get; }
        public IntValue Value { get; private set; }

        public void ModifyValue(IntValue newValue)
        {
            Value = newValue;
        }
    }
}