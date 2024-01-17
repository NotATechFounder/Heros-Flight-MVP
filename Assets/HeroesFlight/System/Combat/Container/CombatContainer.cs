using UnityEngine;

namespace HeroesFlight.System.Combat.Container
{
    public class CombatContainer : MonoBehaviour
    {
        [SerializeField] private int ultCHargesPerHit = 2;
        public int UltChargePerHit => ultCHargesPerHit;
    }
}