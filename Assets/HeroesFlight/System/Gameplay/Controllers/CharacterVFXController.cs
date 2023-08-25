using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterVFXController : MonoBehaviour
    {
        [Header("Level Up")]
        [SerializeField] private ParticleSystem levelUpBuildEffect;
        [SerializeField] private ParticleSystem levelUpEffect;
        [SerializeField] private ParticleSystem levelUpAfterEffect;

        [Header("Miss")]
        [SerializeField] private ParticleSystem missEffect;


        private CameraShakerInterface cameraShakerInterface;

        public void Initialize(CameraShakerInterface cameraShakerInterface)
        {
            this.cameraShakerInterface = cameraShakerInterface;
        }

        public void TriggerLevelUpEffect()
        {
            StartCoroutine(LevelUpEffect());
        }

        IEnumerator LevelUpEffect()
        {
            cameraShakerInterface.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble, .9f, 0.10f);
            levelUpBuildEffect.Play();
            yield return new WaitForSeconds(1f);
            cameraShakerInterface.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion, .1f, 0.20f);
            levelUpBuildEffect.Stop();
            levelUpEffect.Play();
        }

        public void TriggerLevelUpAfterEffect()
        {
            cameraShakerInterface.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble, .9f, 0.10f);
            levelUpAfterEffect.Play();
        }   

        public void TriggerMissEffect()
        {
            missEffect.Play();
        }
    }
}

