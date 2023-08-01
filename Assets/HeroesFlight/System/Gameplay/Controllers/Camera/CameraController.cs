using Cinemachine;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour,CameraControllerInterface
    {
        [SerializeField] float shakeIntensity=1f;
        CinemachineVirtualCamera camera;
        CinemachineBasicMultiChannelPerlin noise;
        bool shouldShake;

        void Awake()
        {
            camera = GetComponent<CinemachineVirtualCamera>();
            noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

       

        public void SetCameraShakeState(bool shouldShake)
        {
            if (shouldShake)
            {
                StartCameraShake();
            }
            else
            {
                StopCameraShake();
            }
        }

        public void SetTarget(Transform target)
        {
            camera.Follow = target;
            camera.LookAt = target;
        }

        void StopCameraShake()
        {
            noise.m_AmplitudeGain = 0;
        }

        void StartCameraShake()
        {
            noise.m_AmplitudeGain = shakeIntensity;
        }
    }
}