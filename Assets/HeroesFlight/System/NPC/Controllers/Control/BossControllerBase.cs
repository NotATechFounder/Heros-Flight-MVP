using System;
using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossControllerBase : MonoBehaviour, BossControllerInterface
    {
        [SerializeField] List<BossAbilityBase> abilities;
        [SerializeField] List<BossCrystalsHealthController> bossHealth;
        [SerializeField] float defaultAbilitiesCooldown;
        [SerializeField] float currentCooldown;

        CameraShakerInterface cameraShaker;
        AiAnimatorInterface animator;

        public BossState State { get; private set; }
        public event Action<float> OnHealthPercentageChange;
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<BossState> OnBossStateChange;

        float maxHealth;
        bool initied;

        


        public void Init()
        {
            cameraShaker = FindObjectOfType<CameraController>().CameraShaker;
            animator = GetComponent<AiAnimationController>();
            currentCooldown = defaultAbilitiesCooldown;
            foreach (var ability in abilities)
            {
                ability.InjectShaker(cameraShaker);
            }
            foreach (var health in bossHealth)
            {
                health.OnDeath += HandleCrystalDeath;
                health.OnBeingDamaged += HandleCrystalDamaged;
                maxHealth += health.MaxHealth;
            }

            initied = true;
            animator.PlayHitAnimation(false);
        }

        void HandleCrystalDamaged(DamageModel obj)
        {
            OnBeingDamaged?.Invoke(obj);
            var currentHealth = CalculateCurrentHealth();
            OnHealthPercentageChange?.Invoke(currentHealth); 
        }

        void HandleCrystalDeath(IHealthController obj)
        {
            var currentHealth = CalculateCurrentHealth();
            if (currentHealth > 0)
            {
                ChangeState(BossState.Damaged);
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,1f);
                animator.PlayHitAnimation(false, () =>
                {
                    ChangeState(BossState.Idle);
                });
            }
            else
            {
                foreach (var ability in abilities)
                {
                    ability.StopAbility();
                }
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,2f);
                ChangeState(BossState.Dead);
                animator.PlayDeathAnimation( () =>
                {
                  gameObject.SetActive(false);
                });
            }
           
        }


        void Update()
        {
            if (!initied)
                return;
            if (State == BossState.Dead)
                return;


            currentCooldown -= Time.deltaTime;
            
            if(State==BossState.Damaged)
                return;
            
            if (currentCooldown <= 0 && State == BossState.Idle)
            {
                UseRandomAbility();
            }
        }


        void UseRandomAbility()
        {
            BossAbilityBase targetAbility =null;
            float totalChance = 0;
            foreach (var t in abilities)
            {
                totalChance += t.UseChance;
            }

            float currentChance = 0;
            var rng = Random.Range(0, totalChance);
            foreach (var t in abilities)
            {
                currentChance += t.UseChance;
                if (rng <= currentChance)
                {
                    targetAbility = t;
                    break;
                    ;
                }
            }
          
           
           
            Debug.Log($"using ability {targetAbility.gameObject.name}");
            var abilityCooldown = targetAbility.CoolDown;
            foreach (var health in bossHealth)
            {
                health.SetInvulnerableState(true);
            }

            targetAbility.UseAbility(() =>
            {
                foreach (var health in bossHealth)
                {
                    health.SetInvulnerableState(false);
                }

                ChangeState(BossState.Idle);
            });
            currentCooldown = abilityCooldown;
            ChangeState(BossState.UsingAbility);
        }

        void ChangeState(BossState newState)
        {
            if (State == newState)
                return;

            State = newState;
            Debug.Log(newState);
            OnBossStateChange?.Invoke(State);
        }

        float CalculateCurrentHealth()
        {
            float currentHealth = 0;
            foreach (var health in bossHealth)
            {
                if(health.CurrentHealth>=0)
                 currentHealth += health.CurrentHealth;
            }
            
            return currentHealth / maxHealth;
        }
    }
}