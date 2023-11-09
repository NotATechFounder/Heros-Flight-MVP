using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ProjectileControllerBase : MonoBehaviour, ProjectileControllerInterface
    {
        [SerializeField] float lifeTime;
        [SerializeField] float speed;
        [SerializeField] LayerMask collisionMask;
        Transform view;
        public event Action<ProjectileControllerInterface> OnEnded;
        Vector2 currentDirection = default;
        float damage;
        float currentLifetime;
        bool disabled;

        void Update()
        {
            if (disabled)
                return;
            
            currentLifetime += Time.deltaTime;
            if (currentLifetime >= lifeTime)
            {
                DisableProjectile();
            }

            transform.Translate(currentDirection.normalized * speed * Time.deltaTime);
        }

        public virtual void DisableProjectile()
        {
            disabled = true;
            OnEnded?.Invoke(this);
        }

        public void SetupProjectile(float targetDamage, Vector2 targetDirection)
        {
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
                healthController.TryDealDamage(new HealthModificationIntentModel(damage,
                    DamageType.NoneCritical,AttackType.Regular,DamageCalculationType.Flat));
                DisableProjectile();
            }
        }
    }
}