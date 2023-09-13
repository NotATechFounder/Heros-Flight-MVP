using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;


using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers.Hazard
{
    public class MushroomHazardSubskill : EnironmentHazard
    {
        [Header("Farting Mushroom Settings")]
        [SerializeField] private Trigger2DObserver effectAreaObserver;
        [SerializeField] private GameObject effectArea;

        [Header("Damage Settings")]
        [SerializeField] private float healthPercentageDecrease;
        [SerializeField] private float poisonDamageInterval;
        [Header("Damage Effect Settings")]
        [SerializeField] private Color damageEffectColor;

        [Header("Slow Effect Settings")]
        [SerializeField] private Color SlowEffectColor;

        [Header("Particle Effect Settings")]
        [SerializeField] private ParticleSystem mainfartingParticle;

        [Header("Slow Settings")]
        [SerializeField] private float slowPercentageDecrease;

        [Header("To Hide")]
        private float poisonDamageTimer;
        private bool isEffectActive;

        private void Awake()
        {
            effectAreaObserver.OnEnter += OnEnterDamageZone;
            effectAreaObserver.OnStay += InDamageZone;
            effectAreaObserver.OnExit += OnExitDamageZone;
            ToggleMushroomEffect(false);
        }

        void OnEnable()
        {
            ToggleMushroomEffect(true);
        }

        void OnDisable()
        {
            ToggleMushroomEffect(false);
        }


        private void Trigger(Collider2D collider2D)
        {
            if (collider2D.TryGetComponent(out IHealthController healthController))
            {
                float damage = StatCalc.GetValueOfPercentage(healthPercentageDecrease, healthController.CurrentHealth);
                healthController.DealDamage(new DamageModel(damage, DamageType.NoneCritical, AttackType.Regular));
            }
        }

        public void ToggleMushroomEffect(bool state)
        {
            isEffectActive = state;
            effectArea.SetActive(state);
            if (state)
            {
                mainfartingParticle.Play();
            }
            else
            {
                mainfartingParticle.Stop();
            }

        }

        private void OnEnterDamageZone(Collider2D collider2D)
        {
            poisonDamageTimer = poisonDamageInterval;
            if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
            {
                characterStatController.ModifyMoveSpeed(slowPercentageDecrease, false);
            }
        }

        private void InDamageZone(Collider2D collider2D)
        {
            if (poisonDamageTimer >= poisonDamageInterval)
            {
                Trigger(collider2D);
                poisonDamageTimer = 0;
            }
            else
            {
                poisonDamageTimer += Time.deltaTime;
            }
        }

        private void OnExitDamageZone(Collider2D collider2D)
        {
            if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
            {
                characterStatController.ModifyMoveSpeed(slowPercentageDecrease, true);
            }
        }
    }
}