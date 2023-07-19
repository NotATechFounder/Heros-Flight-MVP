using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiControllerInterface
    {
        void Init(Transform player);
        void Enable();
        void Disable();

    }
}