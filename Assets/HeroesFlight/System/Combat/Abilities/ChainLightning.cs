using System;
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
        public ChainLightning()
        {
            colliders = new Collider2D[10];
        }
        public ChainLightning(int jumpsLeft, float maxRange, float timeBetweenJumps,
            LayerMask targetMask)
        {
            Init(jumpsLeft,maxRange,timeBetweenJumps,targetMask);
            colliders = new Collider2D[10];
        }

        int maxJumps;
        float range;
        float timeBetweenBounces ;
        int jumpsLeft;
        HealthModificationIntentModel healthModificationIntentModel;
        IHealthController currentTarget;
        List<IHealthController> hitedTargets = new();
        Collider2D[] colliders;
        LayerMask mask;
        public event Action<Transform> OnDealingDamage;
        public event Action<ChainLightning> OnComplete;

        public void Start(IHealthController targetHealthController, HealthModificationIntentModel healthModificationIntent)
        {
            healthModificationIntentModel = healthModificationIntent;
            jumpsLeft = maxJumps;
            currentTarget = targetHealthController;
            CoroutineUtility.Start(StartBounce());
        }

        IEnumerator StartBounce()
        {
            var particle=  ParticleManager.instance.Spawn("Chain_Lightning", currentTarget.HealthTransform);
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = currentTarget.HealthTransform.position;
            particle.GetParticleSystem.Emit(emitParams,1);
            currentTarget.TryDealDamage(healthModificationIntentModel);
            hitedTargets.Add(currentTarget);
            OnDealingDamage?.Invoke(currentTarget.HealthTransform);
            if (jumpsLeft <= 0)
            {
                OnComplete?.Invoke(this);
                yield break;
            }
               
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
            var foundedNext = false;
            for (int i = 0; i < targets.Count; i++)
            {
                if (!hitedTargets.Contains(targets[i]))
                {
                    foundedNext = true;
                    currentTarget = targets[i];
                    emitParams.position = currentTarget.HealthTransform.position;
                    particle.GetParticleSystem.Emit(emitParams,1);
                    jumpsLeft--;
                    yield return new WaitForSeconds(timeBetweenBounces);
                    CoroutineUtility.Start(StartBounce());
                    break;
                }
            }
            
            if(!foundedNext)  
            {
                OnComplete?.Invoke(this);
               
            }
        }

        public void Reset()
        {
            jumpsLeft = 0;
            hitedTargets.Clear();
        }

        public void Init(int jumpsLeft, float maxRange, float timeBetweenJumps, LayerMask targetMask)
        {
            maxJumps = jumpsLeft-1;
            range = maxRange;
            timeBetweenBounces = timeBetweenJumps;
            mask = targetMask;
           
        }
    }
}