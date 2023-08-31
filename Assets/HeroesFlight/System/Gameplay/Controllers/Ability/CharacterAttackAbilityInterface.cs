using System;
using HeroesFlight.System.Gameplay.Model;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CharacterAttackAbilityInterface
    {
        event Action OnDealingDamage;
        void UseAbility( IHealthController targetHealthController, DamageModel damageToDeal);
    }
}