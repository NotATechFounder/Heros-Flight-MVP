using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class AttackData
    {
        [SerializeField] int enemiesPerAttack=4;
        public int EnemiesPerAttack => enemiesPerAttack;
       
    }
}