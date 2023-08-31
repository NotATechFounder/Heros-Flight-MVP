using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Gameplay.Model;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ChainLightning
    {
        public ChainLightning() { }
        public ChainLightning(int jumpsLeft, float maxRange, float timeBetweenJumps,
            LayerMask targetMask)
        {
            maxJumps = jumpsLeft-1;
            range = maxRange;
            timeBetweenBounces = timeBetweenJumps;
            mask = targetMask;
            colliders = new Collider2D[10];
        }

        int maxJumps;
        float range;
        float timeBetweenBounces ;
        int jumpsLeft;
        DamageModel damageModel;
        IHealthController currentTarget;
        List<IHealthController> hitedTargets = new();
        Collider2D[] colliders;
        LayerMask mask;

        public void Start(IHealthController targetHealthController, DamageModel damage)
        {
            damageModel = damage;
            jumpsLeft = maxJumps;
            currentTarget = targetHealthController;
            CoroutineUtility.Start(StartBounce());
        }

        IEnumerator StartBounce()
        {
            currentTarget.DealDamage(damageModel);
            hitedTargets.Add(currentTarget);
            if (jumpsLeft <= 0)
                yield break;
            var hitCount =
                Physics2D.OverlapCircleNonAlloc(currentTarget.HealthTransform.position, range, colliders, mask);
            if (hitCount <= 1)
                yield break;
          
            var targets = new List<IHealthController>();
            for (int i = 0; i < hitCount; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health) && health != currentTarget)
                {
                    targets.Add(health);
                }
            }

            targets = targets.OrderBy((d) =>
                (d.HealthTransform.position - currentTarget.HealthTransform.position).sqrMagnitude).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                if (!hitedTargets.Contains(targets[i]))
                {
                    currentTarget = targets[i];
                    jumpsLeft--;
                    yield return new WaitForSeconds(timeBetweenBounces);
                    CoroutineUtility.Start(StartBounce());
                    break;
                }
            }
        }

        public void Reset()
        {
            jumpsLeft = 0;
            hitedTargets.Clear();
        }
    }
}