using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ProjectileControllerBase : MonoBehaviour, ProjecttileControllerInterface
    {
        [SerializeField] float lifeTime;
        [SerializeField] float speed;
        Transform view;
        public event Action OnEnded;
        Vector2 currentDirection = default;
        float damage;
        float currentLifetime;


        void Update()
        {
            currentLifetime += Time.deltaTime;
            if (currentLifetime >= lifeTime)
            {
                DisableProjectile();
            }

            transform.Translate(currentDirection.normalized * speed * Time.deltaTime);
        }

        void DisableProjectile()
        {
            OnEnded?.Invoke();
            Destroy(this.gameObject);
        }

        public void SetupProjectile(float targetDamage, Transform currentTarget, Vector2 targetDirection)
        {
            currentDirection = targetDirection;
            damage = targetDamage;
            view = transform.GetChild(0);
            var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.AngleAxis(angle, view.forward);
            view.rotation = targetRotation;
            gameObject.SetActive(true);
        }


        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent<IHealthController>(out var healthController))
            {
                healthController.DealDamage(new DamageModel(damage,DamageType.NoneCritical,AttackType.Regular));
                OnEnded?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}