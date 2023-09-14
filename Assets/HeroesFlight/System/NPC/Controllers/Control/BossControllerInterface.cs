
using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Enum;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public interface BossControllerInterface
    {
        public BossState State { get; }
        public event Action<float> OnHealthPercentageChange; 
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<BossState> OnBossStateChange; 
        void Init();
        
    }
}