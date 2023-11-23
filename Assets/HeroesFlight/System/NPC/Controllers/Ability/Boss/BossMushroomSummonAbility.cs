using System;
using Cinemachine;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossMushroomSummonAbility : BossAttackAbilityBase
    {
        [SerializeField] AiControllerBase[] mushrooms;
        [SerializeField] float preDamageDelay = 2f;

        [Header("Mushrooms stat params")]
        [SerializeField] int health=250;
       
        public event Action<IHealthController> OnEnemySpawned;
        Transform player;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            player = GameObject.FindWithTag("Player").transform;
           
        }
     


        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            CoroutineUtility.WaitForSeconds(preDamageDelay,() =>
            {
                abilityParticle.Play();
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                foreach (var mushroom in mushrooms)
                {
                   mushroom.Init(player,health,damage,new MonsterStatModifier(),null);
                   OnEnemySpawned?.Invoke( mushroom.GetComponent<IHealthController>());
                   mushroom.gameObject.SetActive(true);
                }
            });
            
        }
        
        
        public override void StopAbility()
        {
           
            foreach (var mushroom in mushrooms)
            {
                mushroom.gameObject.SetActive(false);
            }
        }
    }
}