using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ProjectileControllerBase : MonoBehaviour, ProjecttileControllerInterface
    {
        [SerializeField] float lifeTime;
        [SerializeField] float speed;
        [SerializeField] LayerMask collisionLayers;
        public event Action OnEnded;
        Vector2 currentDirection = default;
        int damage;
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

        public void SetupProjectile(int targetDamage, Vector2 targetDirection)
        {
            currentDirection = targetDirection;
            damage = targetDamage;
            gameObject.SetActive(true);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log(col.gameObject.name);
            if (col.gameObject.TryGetComponent<IHealthController>(out var healthController))
            {
                healthController.DealDamage(damage);
                OnEnded?.Invoke();
                gameObject.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log(col.name);
            if (col.gameObject.TryGetComponent<IHealthController>(out var healthController))
            {
                healthController.DealDamage(damage);
                DisableProjectile();
            }
        }
    }
}