using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlightProject.System.Combat.Controllers
{
    public interface CharacterAbilityInterface : AbilityInterface
    {
        float CurrentCharge { get; }
        void Init(List<AnimationData> animations, int charges);
        void UpdateAbilityCharges(float value);
    }
}