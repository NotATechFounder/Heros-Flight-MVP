using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class LightningStrikeAbility : AttackAbilityBaseNPC
    {
        [SerializeField] private ZoneDetectorWithIndicator detector;
    }
}