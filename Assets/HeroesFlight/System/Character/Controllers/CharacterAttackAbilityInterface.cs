using System;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CharacterAttackAbilityInterface
    {
        event Action<Transform> OnDealingDamage;
        void UseAbility( IHealthController targetHealthController, HealthModificationIntentModel healthModificationIntentToDeal);
    }
}