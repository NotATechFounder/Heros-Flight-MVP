using System;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class BossNodeBoundAbilitiesEntry 
    {
       [SerializeField] List<AbilityBaseNPC> boundAbilities;
       [SerializeField] BossCrystalsHealthController nodeHealth;
        public BossCrystalsHealthController HealthController => nodeHealth;
        public List<AbilityBaseNPC> Abilities => boundAbilities;

    }
}