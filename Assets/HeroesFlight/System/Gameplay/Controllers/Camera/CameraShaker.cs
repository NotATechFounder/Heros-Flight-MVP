using Cinemachine;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CameraShaker : MonoBehaviour,CameraControllerInterface
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