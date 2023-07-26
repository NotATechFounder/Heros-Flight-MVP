using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class AttackRangeVisualsController : MonoBehaviour
    {
        
        [SerializeField] Transform m_attackVisuals;
       
        public void Init(float attackRange)
        {
            m_attackVisuals.localScale = new Vector3(attackRange*3, attackRange*3, 0);
        }
        public void SetPosition(Vector2 position)
        {
            m_attackVisuals.position = position;
        }
    }
}