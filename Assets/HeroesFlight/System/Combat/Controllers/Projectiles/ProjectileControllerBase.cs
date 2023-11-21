using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;
using static HeroesFlightProject.System.Gameplay.Controllers.ProjectileControllerBase;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ProjectileControllerBase : MonoBehaviour, ProjectileControllerInterface
    {
        public enum ProjectileType
        {
            Impact,
            FlyThrough
        }

        public enum DamgeType
        {
            Normal,
            Line
        }

        [SerializeField] float lifeTime;
        [SerializeField] float speed;
        [SerializeField] LayerMask collisionMask;
        [SerializeField] ProjectileType projectileType;
        [SerializeField] DamgeType damageType;
        [SerializeField] int numberOfLines;
        [SerializeField] float lineDamageDelay;
        [SerializeField] ParticleSystem hitEffect;
        Transform view;
        public event Action<ProjectileControllerInterface> OnHit;
        public event Action<ProjectileControllerInterface> OnDeactivate;
        Vector2 currentDirection = default;
        float damage;
        float currentLifetime;
        bool disabled;
        private IHealthController owner;

        void Update()
        {
            if (disabled)
                return;

            currentLifetime += Time.deltaTime;
            if (currentLifetime >= lifeTime)
            {
                switch (projectileType)
                {
                    case ProjectileType.Impact:
                        ProjectileHit();
                        break;
                    case ProjectileType.FlyThrough:
                        OnDeactivate?.Invoke(this);
                        break;
                    default: break;
                }
            }

            transform.Translate(currentDirection.normalized * speed * Time.deltaTime);
        }

        public void SetLine(int numberOfLines, float lineDamageDelay)
        {
            projectileType = ProjectileType.FlyThrough;
            damageType = DamgeType.Line;
            this.numberOfLines = numberOfLines;
            this.lineDamageDelay = lineDamageDelay;
        }

        public void ProjectileHit()
        {
            switch (projectileType)
            {
                case ProjectileType.Impact:
                    disabled = true;
                    break;
                case ProjectileType.FlyThrough:
                    break;
                default: break;
            }

            if (hitEffect != null)
                hitEffect?.Play();

            OnHit?.Invoke(this);
        }

        public void SetupProjectile(float targetDamage, Vector2 targetDirection,IHealthController currentOwner=null)
        {
            owner = currentOwner;
            currentDirection = targetDirection;
            currentLifetime = 0;
            damage = targetDamage;
            view = transform.GetChild(0);
            var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.AngleAxis(angle, view.forward);
            view.rotation = targetRotation;
            disabled = false;
            gameObject.SetActive(true);
        }


        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (collisionMask != (collisionMask | (1 << col.gameObject.layer)))
                return;

            if (col.gameObject.TryGetComponent<IHealthController>(out var healthController))
            {
                switch (damageType)
                {
                    case DamgeType.Normal:
                        healthController.TryDealDamage(new HealthModificationIntentModel(damage,
                            DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat, owner));
                        break;
                    case DamgeType.Line:
                        healthController.TryDealLineDamage(numberOfLines, lineDamageDelay,
                            new HealthModificationIntentModel(damage, DamageCritType.NoneCritical, AttackType.Regular,
                                CalculationType.Flat,owner));
                        break;
                    default:
                        break;
                }


                ProjectileHit();
            }
        }
    }
}