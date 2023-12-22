using System;
using System.Collections.Generic;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossControllerBase : MonoBehaviour, BossControllerInterface
    {
        public event Action<Transform> OnCrystalDestroyed;
        public BossState State { get; protected set; }
        public List<IHealthController> CrystalNodes { get; protected set; }
        public event Action<float> OnHealthPercentageChange;
        public event Action<HealthModificationIntentModel> OnBeingDamaged;
        public event Action<BossState> OnBossStateChange;

        protected CameraShakerInterface cameraShaker;
        protected AiAnimatorInterface animator;
        protected float maxHealth;
        protected float baseDamage;
        protected bool initied;

        public virtual void Init(float maxHealth,float damage)
        {
            Debug.Log($"{maxHealth} nad {damage}");
            cameraShaker = FindObjectOfType<CameraController>().CameraShaker;
            animator = GetComponent<AiAnimationController>();
            this.maxHealth = maxHealth;
            baseDamage = damage;
        }

        protected void InvokeHealthPercChangeEvent(float value)
        {
            OnHealthPercentageChange?.Invoke(value);
        }

        protected void InvokeOnBeingDamagedEvent(HealthModificationIntentModel model)
        {
            OnBeingDamaged?.Invoke(model);
        }

        protected void InvokeCrystalDestroyedEvent(Transform target)
        {
            OnCrystalDestroyed?.Invoke(target);
        }

        protected void ChangeState(BossState newState)
        {
            if (State == newState)
                return;

            State = newState;
            Debug.Log(newState);
            OnBossStateChange?.Invoke(State);
        }
    }
}