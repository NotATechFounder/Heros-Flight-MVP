using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CameraControllerInterface
    {
        void SetCameraShakeState(bool shouldShake);
        void SetTarget(Transform target);

        void SetCameraState(GameCameraType newState);
    }
}