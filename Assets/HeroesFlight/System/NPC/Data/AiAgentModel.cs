using System;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Data
{
    [Serializable]
    public class AiAgentModel
    {
        [SerializeField] EnemyType m_EnemyType;
        [SerializeField] float agroDistance;
        [SerializeField] float m_AttackRange;
        [SerializeField] float m_Speed;
        [SerializeField] int m_MaxHealth;
        [SerializeField] float m_TimeBetweenAttacks;
        [SerializeField] int damage=2;
        public int Damage => damage;
        public EnemyType EnemyType => m_EnemyType;
        public float AgroDistance => agroDistance;
        public float AttackRange => m_AttackRange;
        public int Health => m_MaxHealth;
        public float Speed => m_Speed;
        public float TimeBetweenAttacks => m_TimeBetweenAttacks;
    }
}