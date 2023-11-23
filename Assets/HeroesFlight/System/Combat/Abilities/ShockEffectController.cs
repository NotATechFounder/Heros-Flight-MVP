using HeroesFlight.System.Gameplay.Model;
using Pelumi.ObjectPool;
using StansAssets.Foundation.Patterns;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ShockEffectController : MonoBehaviour
    {
        [SerializeField] float jumpRadius = 3;
        [SerializeField] int jumpsNumber = 10;
        [SerializeField] float timeBetweenJumps = 0.1f;
        [SerializeField] private Particle mainHitParticle;
        [SerializeField] LayerMask targetMask;

        public Particle MainParticle => mainHitParticle;
        DefaultPool<ShockEffectRuntime> pool;

        void Awake()
        {
            pool = new DefaultPool<ShockEffectRuntime>();
        }

        public void TriggerEffect(IHealthController targetHealthController,
            HealthModificationIntentModel healthModificationIntentToDeal)
        {
            var shock = pool.Get();
            shock.Init(jumpsNumber, jumpRadius, timeBetweenJumps, targetMask);
            shock.OnComplete += HandleChainComplete;
            shock.Start(targetHealthController, healthModificationIntentToDeal);
        }

        void HandleChainComplete(ShockEffectRuntime shock)
        {
            shock.OnComplete -= HandleChainComplete;
            pool.Release(shock);
        }
    }
}