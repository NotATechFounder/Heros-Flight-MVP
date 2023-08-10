using System;
using Cinemachine;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour,CameraControllerInterface
    {
        [SerializeField] float shakeIntensity=1f;
        [SerializeField] CinemachineVirtualCamera characterCamera;
        [SerializeField] CinemachineVirtualCamera skillCamera;
        CinemachineBasicMultiChannelPerlin noise;
        bool shouldShake;
        CameraUiHook hook;
        
        void Awake()
        {
            noise = characterCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = 0;
            hook = FindObjectOfType<CameraUiHook>();
           
        }

        void Start()
        {
            hook.SetCharacterSliderValue(characterCamera.m_Lens.OrthographicSize);
            hook.SetSkillSliderValue(skillCamera.m_Lens.OrthographicSize);
            hook.CharacterSlider.onValueChanged.AddListener(UpdateCharacterCameraFOW);
            hook.SkillSLider.onValueChanged.AddListener(UpdateSkillCameraFOW);
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
            characterCamera.Follow = target;
            characterCamera.LookAt = target;
            skillCamera.Follow = target;
            skillCamera.LookAt = target;
        }

        public void SetCameraState(GameCameraType newType)
        {
            switch (newType)
            {
                case GameCameraType.Character:
                    characterCamera.enabled = true;
                    break;
                case GameCameraType.Skill:
                    characterCamera.enabled = false;
                    break;
               
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

        public void UpdateCharacterCameraFOW(float newValue)
        {
            characterCamera.m_Lens.OrthographicSize = newValue;
        }

        public void UpdateSkillCameraFOW(float newValue)
        {
            skillCamera.m_Lens.OrthographicSize = newValue;
            Debug.Log(skillCamera.m_Lens.OrthographicSize);
        }
        
    }
}