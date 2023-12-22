using System;
using System.Collections.Generic;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;


namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class SpawnEnemyAbilityHealthThreshold : SpawnEnemyAbility
    {
        [Header("Spawn data")] [SerializeField]
        private float minimalHealthThreshhold = 75;

        [SerializeField] List<ThresholdSpawnModel> targetsToSpawn = new();
        private BossController controller;

        public override bool ReadyToUse =>
            currentCooldown <= 0 && controller.CurrentHealthPercentage <= minimalHealthThreshhold;

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponentInParent<BossController>();
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (controller.CurrentHealthPercentage > minimalHealthThreshhold)
            {
                onComplete?.Invoke();
                return;
            }


            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, onComplete);
            }

            currentCooldown = CoolDown;
            CoroutineUtility.WaitForSeconds(1.5f, () => { SpawnRandomEnemies(PickTargetEnemies()); });
        }

        private List<AiControllerBase> PickTargetEnemies()
        {
            var currentHealth = controller.CurrentHealthPercentage;
            List<ThresholdSpawnModel> models = new();

            foreach (var data in targetsToSpawn)
            {
                if (currentHealth <= data.HealthThreshhold)
                {
                    models.Add(data);
                }
            }

            ThresholdSpawnModel modelWithMinThreshHold = GetModelWithMinHealthThreshold(models);

            return modelWithMinThreshHold.TargetsToSpawn;
        }

        private ThresholdSpawnModel GetModelWithMinHealthThreshold(List<ThresholdSpawnModel> models)
        {
            ThresholdSpawnModel modelWithMinThreshHold = models[0];
            foreach (var newData in models)
            {
                if (newData.HealthThreshhold < modelWithMinThreshHold.HealthThreshhold)
                {
                    modelWithMinThreshHold = newData;
                }
            }

            return modelWithMinThreshHold;
        }

        public override void StopAbility()
        {
            base.StopAbility();
            foreach (var mob in spawnedMobs)
            {
                if (mob != null && mob.gameObject.activeSelf)
                    mob.Disable();
            }

            spawnedMobs.Clear();
        }

        void OnDisable()
        {
            StopAbility();
        }
    }
}