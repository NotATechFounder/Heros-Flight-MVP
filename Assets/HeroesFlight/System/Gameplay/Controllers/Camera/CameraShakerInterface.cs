using Cinemachine;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CameraShakerInterface
    {
        void ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes shape, float duration, float force = 1f, Vector2 velocity = default);

        void ShakeCamera(ScreenShakeProfile profile);
    }
}