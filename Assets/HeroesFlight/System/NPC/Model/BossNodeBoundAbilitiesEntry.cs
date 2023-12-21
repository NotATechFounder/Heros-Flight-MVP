using System;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Model
{
    [Serializable]
    public class BossNodeBoundAbilitiesEntry
    {
        [SerializeField] BossCrystalsHealthController nodeHealth;
        [SerializeField] List<BossAbilityBase> boundAbilities;
        
        public BossCrystalsHealthController HealthController => nodeHealth;
        public List<BossAbilityBase> Abilities => boundAbilities;

    }
}