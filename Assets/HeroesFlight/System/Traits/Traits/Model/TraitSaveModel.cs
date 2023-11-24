using System;

namespace HeroesFlight.System.Stats.Traits.Model
{
    [Serializable]
    public class TraitSaveModel
    {
        public TraitSaveModel( string id,int value)
        {
            ID = id;
            Value = value;
        }

        public string ID;
        public int Value;
    }
}