using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class AttackData
    {
        [SerializeField] int enemiesPerAttack=4;
        [SerializeField] float attackPositionOffset=1;

        public int EnemiesPerAttack => enemiesPerAttack;
        public float AttackPositionOffset => attackPositionOffset;
    }
}