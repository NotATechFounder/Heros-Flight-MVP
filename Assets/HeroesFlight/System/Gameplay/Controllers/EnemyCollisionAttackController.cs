using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyCollisionAttackController : EnemyAttackControllerBase
    {
        void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log("Attacking player");
            InitAttack();
        }
    }
}