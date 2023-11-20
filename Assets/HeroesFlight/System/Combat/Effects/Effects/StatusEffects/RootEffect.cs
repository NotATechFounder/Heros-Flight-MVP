using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "RootEffect", menuName = "Combat/Effects/StatusEffects/Root", order = 100)]
    public class RootEffect : StatusEffect
    {
        public override T GetData<T>()
        {
            throw new NotImplementedException();
        }
    }
}