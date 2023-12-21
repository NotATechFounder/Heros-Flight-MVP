using System;
using Cinemachine;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;


namespace HeroesFlight.System.NPC.Controllers.Ability
{
    /// <summary>
    /// Represents the ability of a boss to entangle its targets with roots.
    /// </summary>
    public class EntanglingRootsAbility : BossAbilityBase
    {
        [SerializeField] GameObject entityHolder;
        [SerializeField] float logicTriggerDelay = 2f;

        [Header("Parameters for optional visual zone usage")] [SerializeField]
        WarningLine zone;

        [SerializeField] float zoneWidth;

        [Header("Effect To Apply on Target")] [SerializeField]
        RootStatusEffect rootEffect;

        AreaDamageEntity[] detectors;
        GameObject runtimeEntityHolder;

        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            runtimeEntityHolder = Instantiate(entityHolder, transform.position, Quaternion.identity);
            detectors = runtimeEntityHolder.GetComponentsInChildren<AreaDamageEntity>();
            foreach (var mushroom in detectors)
            {
                mushroom.Init();
                mushroom.OnTargetsDetected += HandleTargetsDetected;
            }
        }

        /// <summary>
        /// Handles the detection of targets and applies the root effect to each target. </summary> <param name="count">The number of targets detected.</param> <param name="targets">An array of Collider2D objects representing the targets.</param>
        /// /
        void HandleTargetsDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                TryApplyRootEffect(targets[i]);
            }
        }

        /// <summary>
        /// Tries to apply a root effect to the specified target collider.
        /// </summary>
        /// <param name="target">The collider to apply the effect to.</param>
        void TryApplyRootEffect(Collider2D target)
        {
            if (target.TryGetComponent<CombatEffectsController>(out var controller))
            {
                controller.ApplyStatusEffect(rootEffect, 1);
            }
        }


        /// <summary>
        /// Uses the ability of the entity.
        /// </summary>
        /// <param name="onComplete">The action to be executed when the ability is complete. Default is null.</param>
        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            runtimeEntityHolder.transform.position = transform.position;
            if (zone == null)
            {
                CoroutineUtility.WaitForSeconds(logicTriggerDelay, TriggerMushroomDetectionAndCameraShake);
            }
            else
            {
                zone.Trigger(TriggerMushroomDetectionAndCameraShake, logicTriggerDelay, zoneWidth);
            }
        }

        private void ToggleAllMushroomIndicators(bool state)
        {
            foreach (var mushroom in detectors)
            {
                mushroom.ToggleIndicator(state);
            }
        }

        /// <summary>
        /// Triggers the mushroom detection event and shakes the camera.
        /// </summary>
        private void TriggerMushroomDetectionAndCameraShake()
        {
            cameraShaker?.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion, .5f);

            foreach (var mushroom in detectors)
            {
                mushroom.StartDetection();
            }
        }


        /// <summary>
        /// Stops the ability of the character.
        /// </summary>
        public override void StopAbility()
        {
            foreach (var mushroom in detectors)
            {
                mushroom.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// This method is called when the component or object is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Destroy(runtimeEntityHolder);
        }
    }
}