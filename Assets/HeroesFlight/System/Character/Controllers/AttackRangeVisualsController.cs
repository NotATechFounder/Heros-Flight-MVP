using System;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class AttackRangeVisualsController : MonoBehaviour
    {
        [SerializeField] float m_AttackRange;
        [SerializeField] Transform m_attackVisuals;
        void Awake()
        {
            m_attackVisuals.localScale = new Vector3(m_AttackRange*2, m_AttackRange*2, 0);
        }
    }
}