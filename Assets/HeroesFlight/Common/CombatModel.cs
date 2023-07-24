using UnityEngine;


namespace HeroesFlight.Common
{
    [CreateAssetMenu(fileName = "CombatModel", menuName = "Model/Combat", order = 0)]
    public class CombatModel : ScriptableObject
    {
        [SerializeField] float agroDistance;
        [SerializeField] float m_AttackRange;
        [SerializeField] float m_Speed;
        [SerializeField] int m_MaxHealth;
        [SerializeField] float m_TimeBetweenAttacks;
        [SerializeField] int damage = 2;
        public int Damage => damage;
        public float AgroDistance => agroDistance;
        public float AttackRange => m_AttackRange;
        public int Health => m_MaxHealth;
        public float Speed => m_Speed;
        public float TimeBetweenAttacks => m_TimeBetweenAttacks;
    }
}