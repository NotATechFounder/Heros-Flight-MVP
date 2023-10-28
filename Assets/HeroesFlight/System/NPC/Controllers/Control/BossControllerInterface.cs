
using System;
using System.Collections.Generic;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public interface BossControllerInterface
    {
        public event Action<Transform> OnCrystalDestroyed; 
        public BossState State { get; }
        public List<IHealthController> CrystalNodes { get; }
        public event Action<float> OnHealthPercentageChange;
        public event Action<HealthModificationIntentModel> OnBeingDamaged;
        public event Action<BossState> OnBossStateChange;
        void Init();
        
    }
}