using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class UltimateData
    {
        [SerializeField] int enemiesPerAttack;
        [SerializeField] float damageMultiplier=1.2f;
        [SerializeField] float rangeMultiplier = 2f;
        [SerializeField] float offsetMultiplier = 4f;
        [SerializeField] int charges = 100;
 
        public int EnemiesPerAttack => enemiesPerAttack;
        public float DamageMultiplier => damageMultiplier;
        public float RangeMultiplier => rangeMultiplier;
        public float OffsetMultiplier => offsetMultiplier;
        public int Charges => charges;
    }
}