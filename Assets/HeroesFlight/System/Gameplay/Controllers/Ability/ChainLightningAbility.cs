using System;
using HeroesFlight.System.Gameplay.Model;
using StansAssets.Foundation.Patterns;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ChainLightningAbility : MonoBehaviour,CharacterAttackAbilityInterface
    {
        [SerializeField] float jumpRadius=3;
        [SerializeField] int jumpsNumber=10;
        [SerializeField] float timeBetweenJumps = 0.1f;
        [SerializeField] LayerMask targetMask;
        public event Action<Transform> OnDealingDamage;
        DefaultPool<ChainLightning> pool;
        void Awake()
        {
            pool = new DefaultPool<ChainLightning>();
        }

        public void UseAbility( IHealthController targetHealthController,
            DamageModel damageToDeal)
        {
            var lightning = pool.Get();
            lightning.Init(jumpsNumber, jumpRadius, timeBetweenJumps, targetMask);
            lightning.OnComplete += HandleChainComplete;
               // new ChainLightning(jumpsNumber, jumpRadius, timeBetweenJumps, targetMask);
            lightning.OnDealingDamage += HandleDamageDealt;
            lightning.Start(targetHealthController,damageToDeal);
          
        }

        void HandleChainComplete(ChainLightning chain)
        {
            chain.OnComplete -= HandleChainComplete;
            chain.OnDealingDamage -= HandleDamageDealt;
            chain.Reset();
            pool.Release(chain);
        }

        void HandleDamageDealt(Transform target)
        {
            OnDealingDamage?.Invoke(target);
        }
    }
}