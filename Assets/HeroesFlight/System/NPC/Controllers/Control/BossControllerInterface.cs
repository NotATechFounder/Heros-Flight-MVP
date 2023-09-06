using System;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public interface BossControllerInterface
    {
        event Action<float> OnHpChange;
        void Init();
        
    }
}