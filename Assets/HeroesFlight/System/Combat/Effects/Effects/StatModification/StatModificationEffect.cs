using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "StatModificationEffect", menuName = "Combat/Effects/StatsModification/StatModificationEffect", order = 100)]
    public class StatModificationEffect : Effect
    {
      [SerializeField] private StatModificationData StatData;

     
        public override T GetData<T>()
        {
            return StatData as T;
        }
    }
}