using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class UltimateData
    {
        [SerializeField] int enemiesPerAttack;
        [SerializeField] float damageMultiplier=1.2f;
        [SerializeField] int charges = 100;
 
        public int EnemiesPerAttack => enemiesPerAttack;
        public float DamageMultiplier => damageMultiplier;
     
        public int Charges => charges;
    }
}