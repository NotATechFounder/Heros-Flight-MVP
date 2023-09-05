using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MushroomEffect : MonoBehaviour
{
    public enum MushroomType
    {
        Speed,
        Health,
        Damage,
    }

    public enum MushroomTrigger
    {
        Instant,
        Continuous,
    }

    public enum Effect
    {
        Increase,
        Decrease
    }

    [SerializeField] private MushroomType mushroomType;
    [SerializeField] private MushroomTrigger mushroomTrigger;
    [SerializeField] private Effect effect;
    [SerializeField] private float effectPercentage;
    [SerializeField] private float effectInterval;


    private void OnEnterZone(Collider2D collider2D)
    {
        switch (mushroomType)
        {
            case MushroomType.Speed:
                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(effectPercentage, effect == Effect.Increase);
                }
                break;
            case MushroomType.Health:
                if (collider2D.TryGetComponent(out IHealthController healthController))
                {
                    float damage = StatCalc.GetValueOfPercentage(effectPercentage, healthController.CurrentHealth);
                 //   healthController.DealDamage(new DamageModel(damage, DamageType.NoneCritical, AttackType.Regular));
                }
                break;
            case MushroomType.Damage:

                break;
            default:
                break;
        }
    }

    private void InZone(Collider2D collider2D)
    {
        switch (mushroomType)
        {
            case MushroomType.Speed:
                if (collider2D.TryGetComponent(out CharacterStatController characterStatController))
                {
                    characterStatController.ModifyMoveSpeed(effectPercentage, effect != Effect.Increase);
                }
                break;
            case MushroomType.Health:

                break;
            case MushroomType.Damage:

                break;
            default:
                break;
        }
    }

    private void OnExitZone(Collider2D d)
    {
        switch (mushroomType)
        {
            case MushroomType.Speed:

                break;
            case MushroomType.Health:

                break;
            case MushroomType.Damage:

                break;
            default:
                break;
        }
    }
}
