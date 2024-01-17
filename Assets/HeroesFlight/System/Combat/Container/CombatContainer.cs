using UnityEngine;

namespace HeroesFlight.System.Combat.Container
{
    public class CombatContainer : MonoBehaviour
    {
        [SerializeField] private float ultChargesPerHit = 2;
        public float UltChargePerHit => ultChargesPerHit;
    }
}