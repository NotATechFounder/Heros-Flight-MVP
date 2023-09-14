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

        [Header("Booster")]
        [SerializeField] private ParticleSystem healingBoosterEffect;
        [SerializeField] private ParticleSystem attackBoosterEffect;
        [SerializeField] private ParticleSystem defBoosterEffect;
        [SerializeField] private ParticleSystem speedBoosterEffect;

        [Header("Currency")]
        [SerializeField] private ParticleSystem goldEffect;
        [SerializeField] private ParticleSystem expEffect;


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

        public void TriggerBoosterEffect(BoosterEffectType boosterEffectType)
        {
            string audioID = "";

            switch (boosterEffectType)
            {
                case BoosterEffectType.Attack:
                    audioID = "AttackBooster";
                    attackBoosterEffect.Play();         
                    break;
                case BoosterEffectType.Defense:
                    audioID = "DefenseBooster";
                    defBoosterEffect.Play();
                    break;
                case BoosterEffectType.Health:
                    audioID = "HealthBooster";
                    healingBoosterEffect.Play();
                    break;
                case BoosterEffectType.MoveSpeed:
                    audioID = "SpeedBooster";
                    speedBoosterEffect.Play();
                    break;
            }

            AudioManager.PlaySoundEffect(audioID);
        }

        public void TriggerCurrencyEffect(string currencyID)
        {
            string audioID = "";

            switch (currencyID)
            {
                case CurrencyKeys.Gold:
                    audioID = "Gold";
                    goldEffect.Play();
                    break;
                case CurrencyKeys.Experience:
                    audioID = "Experience";
                    expEffect.Play();
                    break;
            }

            AudioManager.PlaySoundEffect(audioID, true);
        }
    }
}

