using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;


using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers.Hazard
{
    public class MushroomHazardSubskill : EnironmentHazard
    {
        public enum SkillType
        {
            Damage,
            Slow,
            Both,
        }

        [Header("Farting Mushroom Settings")]
        [SerializeField] private SkillType skillType;
        [SerializeField] private Trigger2DObserver effectAreaObserver;
        [SerializeField] private GameObject effectArea;

        [Header("Damage Settings")]
        [SerializeField] private float healthPercentageDecrease;
        [SerializeField] private float poisonDamageInterval;

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
                float damage = StatCalc.GetPercentage(healthPercentageDecrease, healthController.CurrentHealth);
                healthController.TryDealDamage(new HealthModificationIntentModel(healthPercentageDecrease, 
                    DamageCritType.NoneCritical, AttackType.Regular,CalculationType.Percentage,null));
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
            switch (skillType)
            {
                case SkillType.Damage:

                    break;
                case SkillType.Slow:
                    if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                    {
                        characterStatController.GetStatModel.ModifyCurrentStat( StatType.MoveSpeed, slowPercentageDecrease, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    }
                    break;
                case SkillType.Both:
                    if (collider2D.TryGetComponent(out characterStatController))
                    {
                        characterStatController.GetStatModel. ModifyCurrentStat(StatType.MoveSpeed, slowPercentageDecrease, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    }
                    break;
                default: break;
            }
        }

        private void InDamageZone(Collider2D collider2D)
        {
            switch (skillType)
            {
                case SkillType.Damage:
                    if (poisonDamageTimer >= poisonDamageInterval)
                    {
                        Trigger(collider2D);
                        poisonDamageTimer = 0;
                    }
                    else
                    {
                        poisonDamageTimer += Time.deltaTime;
                    }
                    break;
                case SkillType.Slow:

                    break;
                case SkillType.Both:
                    if (poisonDamageTimer >= poisonDamageInterval)
                    {
                        Trigger(collider2D);
                        poisonDamageTimer = 0;
                    }
                    else
                    {
                        poisonDamageTimer += Time.deltaTime;
                    }
                    break;
                default: break;
            }
        }

        private void OnExitDamageZone(Collider2D collider2D)
        {
            switch (skillType)
            {
                case SkillType.Damage:

                    break;
                case SkillType.Slow:
                    if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                    {
                        characterStatController.GetStatModel.ModifyCurrentStat( StatType.MoveSpeed, slowPercentageDecrease, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
                    }
                    break;
                case SkillType.Both:
                    if (collider2D.TryGetComponent(out characterStatController))
                    {
                        characterStatController.GetStatModel.ModifyCurrentStat(StatType.MoveSpeed, slowPercentageDecrease, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
                    }
                    break;
                default: break;
            }
        }
    }
}