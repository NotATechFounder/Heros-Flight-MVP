using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "StatModificationEffect", menuName = "Combat/Effects/StatModificationEffect", order = 100)]
    public class StatModificationEffect : Effect
    {
        [SerializeField] private string targetAttribute;

        public string TargetAttribute => targetAttribute;
    }
}