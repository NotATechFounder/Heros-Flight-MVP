using System;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class MobNodeBoundAbilitiesEntry
    {
        [SerializeField] BossCrystalsHealthController nodeHealth;
        [SerializeField] List<AbilityBaseNPC> boundAbilities;
        
        public BossCrystalsHealthController HealthController => nodeHealth;
        public List<AbilityBaseNPC> Abilities => boundAbilities;
    }
}