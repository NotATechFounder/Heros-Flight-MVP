using System;
using Cinemachine;
using HeroesFlight.System.Gameplay.Enum;
using StansAssets.Foundation.Async;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour,CameraControllerInterface
    {
        [Header("General references")]
        [SerializeField] PolygonCollider2D boundsCollider;
        [SerializeField] CinemachineVirtualCamera characterCamera;
        [SerializeField] CinemachineVirtualCamera skillCamera;
        [SerializeField] CinemachineVirtualCamera environmentCamera;

        [Header("Impulse sources")]
        [SerializeField] CinemachineImpulseSource bumpSource;
        [SerializeField] CinemachineImpulseSource explosionSource;
        [SerializeField] CinemachineImpulseSource rumbleSource;
        [SerializeField] CinemachineImpulseSource recoilSource;
        [SerializeField] CinemachineImpulseSource genericSource;

        CinemachineImpulseListener listener;
        CameraUiHook hook;
        CinemachineConfiner2D characterCameraconfiner;
        CinemachineConfiner2D skillCameraconfiner;


        void Awake()
        {
            characterCameraconfiner = characterCamera.GetComponent<CinemachineConfiner2D>();
            skillCameraconfiner = skillCamera.GetComponent<CinemachineConfiner2D>();
            listener = characterCamera.GetComponent<CinemachineImpulseListener>();
            hook = FindObjectOfType<CameraUiHook>();
            CameraShaker = new CinemachineCameraShaker(bumpSource,explosionSource,rumbleSource,recoilSource,genericSource,listener);
        }

      
        void Start()
        {
            hook.SetCharacterSliderValue(characterCamera.m_Lens.OrthographicSize);
            hook.SetSkillSliderValue(skillCamera.m_Lens.OrthographicSize);
            hook.CharacterSlider.onValueChanged.AddListener(UpdateCharacterCameraFOW);
            hook.SkillSLider.onValueChanged.AddListener(UpdateSkillCameraFOW);
        }


        public CameraShakerInterface CameraShaker { get; private set; }

        public void SetTarget(Transform target)
        {
            characterCamera.Follow = target;
            characterCamera.LookAt = target;
            skillCamera.Follow = target;
            skillCamera.LookAt = target;
        }

        public void SetConfiner(PolygonCollider2D collider2D)
        {
            characterCameraconfiner.m_BoundingShape2D = collider2D;
            skillCameraconfiner.m_BoundingShape2D = collider2D;
        }

        public void UpdateCharacterCameraFrustrum(float modifier, bool increase)
        {
            if (increase)
            {
                characterCamera.m_Lens.OrthographicSize *= modifier;
            }
            else
            {
                characterCamera.m_Lens.OrthographicSize /= modifier;
            }
        }

        public void InitLevelOverview(float duration,Action onComplete)
        {
            environmentCamera.enabled = true;
            CoroutineUtility.WaitForSeconds(duration, () =>
            {
                environmentCamera.enabled = false;
                onComplete?.Invoke();
            });
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
         
        public void UpdateCharacterCameraFOW(float newValue)
        {
            characterCamera.m_Lens.OrthographicSize = newValue;
        }

        public void UpdateSkillCameraFOW(float newValue)
        {
            skillCamera.m_Lens.OrthographicSize = newValue;
        }    
    }
}