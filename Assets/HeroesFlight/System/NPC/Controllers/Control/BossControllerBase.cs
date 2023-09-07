using System;
using System.Collections.Generic;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossControllerBase : MonoBehaviour,BossControllerInterface
    {
        [SerializeField] List<AbilityBaseNPC> abilities;
        [SerializeField] float defaultAbilitiesCooldown;
        [SerializeField] float currentCooldown;
       
        bool isDead;
        AiAnimatorInterface animator;

        void Awake()
        {
            Init();
        }


    
        public void Init()
        {
            animator = GetComponent<AiAnimationController>();
            currentCooldown = defaultAbilitiesCooldown;
        }

        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;
      

        public bool IsDead()
        {
            throw new NotImplementedException();
        }

     
        public void SetInvulnerableState(bool isImmortal)
        {
            throw new NotImplementedException();
        }

        void Update()
        {
            if (isDead)
                return;


            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                UseRandomAbility();
            }
        }


        void UseRandomAbility()
        {
            var rng = Random.Range(0, abilities.Count);
            var targetAbility = abilities[rng];
            Debug.Log($"using ability {targetAbility.gameObject.name}");
            var abilityCooldown = targetAbility.CoolDown;
            targetAbility.UseAbility();
            currentCooldown = abilityCooldown;
        }
    }
}