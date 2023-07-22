using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiMoverInterface
    {
        void Move(Vector2 target);
    }
}