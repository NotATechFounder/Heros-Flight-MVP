using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Gameplay.Model;
using Pelumi.ObjectPool;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ShockEffectRuntime
    {
        public ShockEffectRuntime()
        {
            colliders = new Collider2D[10];
        }
      
        int maxJumps;
        float range;
        float timeBetweenBounces;
      
        HealthModificationIntentModel healthModificationIntentModel;
        IHealthController currentTarget;
        private Particle mainHitParticle;
        Collider2D[] colliders;
        LayerMask mask;
        public event Action<Transform> OnDealingDamage;
        public event Action<ShockEffectRuntime> OnComplete;

        public void Start(IHealthController targetHealthController,
            HealthModificationIntentModel healthModificationIntent)
        {
            healthModificationIntentModel = healthModificationIntent;
           
            currentTarget = targetHealthController;
            CoroutineUtility.Start(StartBounce());
        }

        IEnumerator StartBounce()
        {
            
            yield return new WaitForSeconds(timeBetweenBounces);
            var hitCount =
                Physics2D.OverlapCircleNonAlloc(currentTarget.HealthTransform.position, range, colliders, mask);
            if (hitCount <= 1)
            {
                OnComplete?.Invoke(this);
                yield break;
            }

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
            var currentlyHited = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                if (currentlyHited >= maxJumps)
                    break;
              
                currentlyHited++;
                var particle = ParticleManager.instance.Spawn("Chain_Lightning", currentTarget.HealthTransform);
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = currentTarget.HealthTransform.position;
                particle.GetParticleSystem.Emit(emitParams, 1);
                emitParams.position = targets[i].HealthTransform.position;
                particle.GetParticleSystem.Emit(emitParams, 1);
                targets[i].TryDealDamage(healthModificationIntentModel);
            }


            OnComplete?.Invoke(this);
        }

      

        public void Init(int jumpsLeft, float maxRange, float timeBetweenJumps, LayerMask targetMask)
        {
            maxJumps = jumpsLeft;
            range = maxRange;
            timeBetweenBounces = timeBetweenJumps;
            mask = targetMask;
        }
    }
}