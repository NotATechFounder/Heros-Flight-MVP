using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossController : BossControllerBase
    {
        [SerializeField] List<BossNodeBoundAbilitiesEntry> abilityNodes= new ();
        [SerializeField] float defaultAbilitiesCooldown;
        [SerializeField] float currentCooldown;
        [SerializeField] private bool boundAbilitiesToCrystals = true;
      
        Queue<AbilityBaseNPC> abilityQue = new ();
        Dictionary<BossCrystalsHealthController, List<AbilityBaseNPC>> abilityNodesCache = new();


        public override void Init(float maxHealth,float damage)
        {
            base.Init(maxHealth,damage);
            currentCooldown = defaultAbilitiesCooldown;
            CrystalNodes = new List<IHealthController>();
            foreach (var node in abilityNodes)
            {
                abilityNodesCache.Add(node.HealthController,node.Abilities);
                foreach (var ability in node.Abilities)
                {
                    ability.InjectShaker(cameraShaker);
                    if (ability.GetType() == typeof(AttackAbilityBaseNPC))
                    {
                        var attackAbility = ability as AttackAbilityBaseNPC;
                        attackAbility.SetStats(damage,0);
                    }
                }
                
                node.HealthController.OnDeath += HandleCrystalDeath;
                node.HealthController.OnDamageReceiveRequest += HandleCrystalDamaged;
                CrystalNodes.Add(node.HealthController);
                node.HealthController.SetMaxHealth(maxHealth/abilityNodes.Count);
            }
            
            initied = true;
            animator.PlayHitAnimation(false);
        }

        void HandleCrystalDamaged(HealthModificationRequestModel healthModificationRequestModel)
        {
            InvokeOnBeingDamagedEvent(healthModificationRequestModel.IntentModel);
            var damagedHealth = healthModificationRequestModel.RequestOwner.HealthTransform.GetComponent<BossCrystalsHealthController>();
            PickAffectedAbility(damagedHealth);
            var currentHealth = CalculateCurrentHealth();
            InvokeHealthPercChangeEvent(currentHealth); 
        }

        private void PickAffectedAbility(BossCrystalsHealthController damagedHealth)
        {
            if (!boundAbilitiesToCrystals)
                return;
            
            if (abilityNodesCache.TryGetValue(damagedHealth, out var abilities))
            {
               
                AbilityBaseNPC targetAbility =null;
                float totalChance = 0;
                foreach (var ability in abilities)
                {
                    totalChance += ability.UseChance;
                   
                }
           

                float currentChance = 0;
                var rng = Random.Range(0, totalChance);
                
                foreach (var ability in abilities)
                {
                    currentChance += ability.UseChance;
                    if (rng <= currentChance)
                    {
                        targetAbility = ability;
                        break;
                        ;
                    }
                }

                if (targetAbility != null)
                {
                    if (abilityQue.Count > 0)
                        abilityQue.Dequeue();
                    abilityQue.Enqueue(targetAbility);
                }
            }
        }

        void HandleCrystalDeath(IHealthController obj)
        {
            var currentHealth = CalculateCurrentHealth();
           
            if (currentHealth > 0)
            {
                ChangeState(BossState.Damaged);
                InvokeCrystalDestroyedEvent(obj.HealthTransform);
                foreach (var abilityList in abilityNodesCache.Values)
                {
                    foreach (var ability in abilityList)
                    {
                        ability.ResetAbility();
                    }
                  
                }
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,1f);
                var healthToRemove = obj as BossCrystalsHealthController;
               
                if (abilityNodesCache.TryGetValue(healthToRemove,out var abilities))
                {
                    foreach (var ability in abilities)
                    {
                        ability.StopAbility();
                    }
                    abilityNodesCache.Remove(healthToRemove);
                    abilityQue.Dequeue();
                }
                animator.PlayHitAnimation(false, () =>
                {
                    ChangeState(BossState.Idle);
                });
            }
            else
            {
                foreach (var abilityList in abilityNodesCache.Values)
                {
                    foreach (var ability in abilityList)
                    {
                        ability.StopAbility();
                    }
                  
                }
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,2f);
                ChangeState(BossState.Dead);
                animator.PlayDeathAnimation( () =>
                {
                  //gameObject.SetActive(false);
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
                UseAbility();
            }
        }

        void UseAbility()
        {
            if (abilityQue.Count > 0)
            {
                Debug.Log("Gona use qued ability");
                UseQueuedAbility();
            }
            else
            {
                UseRandomAbility();
            }
        }


        void UseRandomAbility()
        {
            AbilityBaseNPC targetAbility =null;
            float totalChance = 0;
            foreach (var abilityList in abilityNodesCache.Values)
            {
                foreach (var ability in abilityList)
                {
                    totalChance += ability.UseChance;
                }
               
            }
           

            float currentChance = 0;
            var rng = Random.Range(0, totalChance);
            foreach (var abilityList in abilityNodesCache.Values)
            {
                foreach (var ability in abilityList)
                {
                    currentChance += ability.UseChance;
                    if (rng <= currentChance)
                    {
                        targetAbility = ability;
                        break;
                        ;
                    }
                }
               
            }
          
           
           
            Debug.Log($"using ability {targetAbility.gameObject.name}");
            var abilityCooldown = targetAbility.CoolDown;
           

            targetAbility.UseAbility(() =>
            {
                ChangeState(BossState.Idle);
            });
            currentCooldown = abilityCooldown;
            targetAbility.SetCoolDown(abilityCooldown);
            ChangeState(BossState.UsingAbility);
        }
        
        void UseQueuedAbility()
        {
            var ability = abilityQue.Dequeue();
            var abilityCooldown = ability.CoolDown;

            ability.UseAbility(() =>
            {
                ChangeState(BossState.Idle);
            });
            currentCooldown = abilityCooldown;
            ability.SetCoolDown(abilityCooldown);
            ChangeState(BossState.UsingAbility);
        }

    

        float CalculateCurrentHealth()
        {
            float currentHealth = 0;
            foreach (var health in abilityNodesCache.Keys)
            {
                if(health.CurrentHealth>=0)
                 currentHealth += health.CurrentHealth;
            }
            
            return currentHealth / maxHealth;
        }

        // void SetHealthCrystalsState(bool isImmortal)
        // {
        //     foreach (var health in bossHealth)
        //     {
        //         health.SetInvulnerableState(isImmortal);
        //     }
        // }
    }
}