using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;


using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers.Hazard
{
    public class MushroomHazardSubskill : EnironmentHazard
    {
    [Header("Farting Mushroom Settings")]
    [SerializeField] private MushroomHazard.FartingMushroomType fartingMushroomType;
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
    [SerializeField] private ParticleSystem subfartingParticle;

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
        SetAllParticleColor();
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

    public void SetAllParticleColor()
    {
        switch (fartingMushroomType)
        {
            case MushroomHazard.FartingMushroomType.Damage:
                SetParticleColor(mainfartingParticle, damageEffectColor);
                SetParticleColor(subfartingParticle, damageEffectColor);
                break;
            case MushroomHazard.FartingMushroomType.Slow:
                SetParticleColor(mainfartingParticle, SlowEffectColor);
                SetParticleColor(subfartingParticle, SlowEffectColor);
                break;
            case MushroomHazard.FartingMushroomType.Both:
                SetParticleColor(mainfartingParticle, damageEffectColor);
                SetParticleColor(subfartingParticle, SlowEffectColor);
                break;
            default: break;
        }
    }

    public void SetParticleColor(ParticleSystem particle, Color startColor)
    {
        var main = particle.main;
        main.startColor = startColor;
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
        switch (fartingMushroomType)
        {
            case MushroomHazard.FartingMushroomType.Damage:
                poisonDamageTimer = poisonDamageInterval;
                break;
            case MushroomHazard.FartingMushroomType.Slow:

                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, false);
                }

                break;
            case MushroomHazard.FartingMushroomType.Both:
                poisonDamageTimer = poisonDamageInterval;
                if (collider2D.TryGetComponent(out characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, false);
                }
                break;
            default: break;
        }
    }

    private void InDamageZone(Collider2D collider2D)
    {
        switch (fartingMushroomType)
        {
            case MushroomHazard.FartingMushroomType.Damage:
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
            case MushroomHazard.FartingMushroomType.Slow:

                break;
            case MushroomHazard.FartingMushroomType.Both:
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
        switch (fartingMushroomType)
        {
            case MushroomHazard.FartingMushroomType.Damage:

                break;
            case MushroomHazard.FartingMushroomType.Slow:
                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, true);
                }
                break;
            case MushroomHazard.FartingMushroomType.Both:
                if (collider2D.TryGetComponent(out characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(slowPercentageDecrease, true);
                }
                break;
            default: break;
        }
    }

   
    }
}