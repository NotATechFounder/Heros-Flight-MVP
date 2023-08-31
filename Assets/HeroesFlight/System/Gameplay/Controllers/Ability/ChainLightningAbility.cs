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
        public event Action OnDealingDamage;
        ObjectPool<ChainLightning> pool;
        void Awake()
        {
           // pool= new ObjectPool<ChainLightning>()
        }

        public void UseAbility( IHealthController targetHealthController,
            DamageModel damageToDeal)
        {
            Debug.Log("USING ABILITY");
           
            var lightning =
                new ChainLightning(jumpsNumber, jumpRadius, timeBetweenJumps, targetMask);
            lightning.Start(targetHealthController,damageToDeal);
          
        }
      
    }
}