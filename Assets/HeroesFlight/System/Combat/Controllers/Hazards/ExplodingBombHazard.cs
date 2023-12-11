using System.Collections;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using Plugins.Audio_System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlight.System.Combat.Controllers.Hazards
{
    public class ExplodingBombHazard : MonoBehaviour
    {
        [SerializeField] private float damagePercentage = 20;

        [Header("Animation and Viusal Settings")] [SerializeField]
        private GameObject visual;

        [SerializeField] private AllIn1EffectTrigger effectTrigger;
        [SerializeField] private float explosionDelay = 1f;
        [SerializeField] private Trigger2DObserver detectorObserver;
        [SerializeField] private CircleOverlap damageOverlap;
        [SerializeField] private ParticleCallbackTrigger particleCallbackTrigger;
        [SerializeField] private GameObject warningEffect;

        private bool isExploded;


        private void Awake()
        {
            detectorObserver.OnEnter += OnEnterZone;
            damageOverlap.OnDetect += OnDetect;
            particleCallbackTrigger.OnEnd += ParticleCallbackTrigger_OnEnd;
        }


        private void OnEnterZone(Collider2D collider2D)
        {
            if (isExploded)
            {
                return;
            }

            isExploded = true;
            warningEffect.SetActive(true);
            effectTrigger.StartEffect();
            StartCoroutine(ExplodeRoutine());
        }

        private void Explode()
        {
            AudioManager.PlaySoundEffect("Explosion", SoundEffectCategory.Environment);
            visual.SetActive(false);
            particleCallbackTrigger.Play();
            damageOverlap.DetectOverlap();
        }

        private void OnDetect(int arg1, Collider2D[] collider2D)
        {
            for (int i = 0; i < arg1; i++)
            {
                if (collider2D[i].TryGetComponent(out IHealthController healthController))
                {
                    healthController.DealHealthPercentageDamage(damagePercentage, DamageCritType.Critical,
                        AttackType.Regular);
                }

                if (collider2D[i].TryGetComponent(out Trigger2DObserver trigger2DObserver))
                {
                    if (!trigger2DObserver.transform.parent.TryGetComponent(
                            out ExplodingBombHazard explosionMushroomHazard))
                    {
                        continue;
                    }

                    if (explosionMushroomHazard == this || explosionMushroomHazard.isExploded)
                    {
                        continue;
                    }

                    explosionMushroomHazard.OnEnterZone(null);
                }
            }
        }


        IEnumerator ExplodeRoutine()
        {
            yield return new WaitForSeconds(explosionDelay);
            Explode();
        }

        private void ParticleCallbackTrigger_OnEnd()
        {
            Destroy(gameObject);
        }
    }
}