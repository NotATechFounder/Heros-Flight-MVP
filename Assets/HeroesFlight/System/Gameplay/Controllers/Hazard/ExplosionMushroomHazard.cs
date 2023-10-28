using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using Plugins.Audio_System;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionMushroomHazard : MonoBehaviour
{
    public const string idleHidingAnimationName = "1_idle_hiding";
    public const string idleToShowAnimationName = "2_idle_to_show";

    [SerializeField] private float damagePercentage = 20;

    [Header("Animation and Viusal Settings")]
    [SerializeField] private GameObject visual;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] private Trigger2DObserver detectorObserver;
    [SerializeField] private CircleOverlap damageOverlap;
    [SerializeField] private ParticleCallbackTrigger particleCallbackTrigger;
    [SerializeField] private GameObject warningEffect;

    private bool isExploded;
    private JuicerRuntime juicerRuntime;

    private void Awake()
    {
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        detectorObserver.OnEnter += OnEnterZone;
        damageOverlap.OnDetect += OnDetect;
        particleCallbackTrigger.OnEnd += ParticleCallbackTrigger_OnEnd;
    }

    private void Start()
    {
        juicerRuntime = meshRenderer.JuicyFloatProperty("_FillPhase", 1,.05f).SetLoop(-1);
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == idleToShowAnimationName)
        {
            Explode();
        }
    }

    private void OnEnterZone(Collider2D collider2D)
    {
        if (isExploded)
        {
            return;
        }
        isExploded = true;

        skeletonAnimation.AnimationState.SetAnimation(0, idleToShowAnimationName, false);
        juicerRuntime.Start();
        warningEffect.SetActive(true);
    }

    private void Explode()
    {
        AudioManager.PlaySoundEffect("Explosion", SoundEffectCategory.Environment);
        visual.SetActive(false);
        particleCallbackTrigger.Play();
        damageOverlap.Detect();
    }

    private void OnDetect(int arg1, Collider2D[] collider2D)
    {
        for (int i = 0; i < arg1; i++)
        {
            if (collider2D[i].TryGetComponent(out IHealthController healthController))
            {
                healthController.DealHealthPercentageDamage(damagePercentage,DamageType.Critical, AttackType.Regular);
            }

            if (collider2D[i].TryGetComponent(out Trigger2DObserver trigger2DObserver))
            {
                if (!trigger2DObserver.transform.parent.TryGetComponent(out ExplosionMushroomHazard explosionMushroomHazard))
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

    private void ParticleCallbackTrigger_OnEnd()
    {
        Destroy(gameObject);
    }
}
