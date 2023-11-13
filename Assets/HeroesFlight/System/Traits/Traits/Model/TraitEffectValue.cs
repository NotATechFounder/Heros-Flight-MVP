namespace HeroesFlight.System.Stats.Traits.Model
{
    public class TraitEffectValue<T>
    {
        public T Value { get; protected set; }

        public  T GetValue()
        {
            return Value;
        }
    }
}