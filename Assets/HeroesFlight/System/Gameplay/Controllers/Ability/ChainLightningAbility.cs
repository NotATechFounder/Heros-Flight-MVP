using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ChainLightningAbility : MonoBehaviour,CharacterAttackAbilityInterface
    {
        [SerializeField] float jumpRadius=3;
        [SerializeField] int jumpsNumber=10;
        [SerializeField] float timeBetweenJumps = 0.1f;
        [SerializeField] LayerMask targetMask;
        
        IHealthController currentTarget;
        DamageModel damageModel;
        Collider2D[] colliders;
      

        void Awake()
        {
            colliders = new Collider2D[10];
        }

        public event Action OnDealingDamage;

        public void UseAbility( IHealthController targetHealthController,
            DamageModel damageToDeal)
        {
            Debug.Log("USING ABILITY");
            currentTarget = targetHealthController;
            damageModel = damageToDeal;
            var lightning =
                new ChainLightning(jumpsNumber, jumpRadius, targetHealthController, timeBetweenJumps, targetMask);
            lightning.Start(damageToDeal);
          
        }



        IEnumerator ProcessBounce()
        {
            currentTarget.DealDamage(damageModel);
            var hitCount = Physics2D.OverlapCircleNonAlloc(currentTarget.HealthTransform.position, jumpRadius, colliders, targetMask);
            if (hitCount == 0)
                yield return null;
            Debug.Log($"founded {hitCount}");
            if (hitCount == 1)
            {
                if (colliders[0].TryGetComponent<IHealthController>(out var health) && health!=currentTarget)
                {
                    currentTarget = health;
                    StartCoroutine(ProcessBounce());
                }
            }
            else
            {
                var targets = new List<IHealthController>();
                for (int i = 0; i < hitCount; i++)
                {
                    if (colliders[i].TryGetComponent<IHealthController>(out var health) && health!=currentTarget)
                    {
                        targets.Add(health);
                    }
                }
                targets = targets.OrderBy((d) => (d.HealthTransform.position - transform.position).sqrMagnitude).ToList();
                currentTarget = targets[0];
                StartCoroutine(ProcessBounce());
                
            }

            void NotifyDamageDealt()
            {
                OnDealingDamage?.Invoke();
            }
            
        }
    }
}