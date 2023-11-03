using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class MeleeEnemyAttackController : EnemyAttackControllerBase
    {
       
        // protected override void Update()
        // {
        //     if (health.IsDead())
        //         return;
        //
        //     if (target.IsDead())
        //     {
        //         aiController.SetAttackState(false);
        //         return;
        //     }
        //
        //
        //     timeSinceLastAttack += Time.deltaTime;
        //     var distanceToPlayer = Vector2.Distance(transform.position, aiController.CurrentTarget.position);
        //     if (distanceToPlayer <= attackRange && timeSinceLastAttack >= timeBetweenAttacks)
        //     {
        //         aiController.SetAttackState(true);
        //         InitAttack(null);
        //     }
        // }
    }
}