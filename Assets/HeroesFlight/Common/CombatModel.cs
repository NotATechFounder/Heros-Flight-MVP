using UnityEngine;


namespace HeroesFlight.Common
{
    public abstract class CombatModel : ScriptableObject
    {
        [SerializeField] int health;
        [SerializeField] float timeBetweenAttacks;
        [SerializeField] float moveSpeed;
        [SerializeField] float attackRange;

        public int Health => health;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        public float MoveSpeed => moveSpeed;
        public float AttackRange => attackRange;
    }
}