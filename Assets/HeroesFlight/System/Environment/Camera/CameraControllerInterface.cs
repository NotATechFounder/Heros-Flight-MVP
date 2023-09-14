using System;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CameraControllerInterface
    {
        CameraShakerInterface CameraShaker { get; }
        void SetTarget(Transform target);
        void SetCameraState(GameCameraType newState);
        void SetConfiner(PolygonCollider2D collider2D);

        void UpdateCharacterCameraFrustrum(float modifier, bool increase);
        void InitLevelOverview(float duration,Action onComplete);
    }
}