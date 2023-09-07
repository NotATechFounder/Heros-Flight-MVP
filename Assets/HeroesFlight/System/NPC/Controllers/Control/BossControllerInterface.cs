
using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public interface BossControllerInterface
    {
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;
        void Init();
        
    }
}