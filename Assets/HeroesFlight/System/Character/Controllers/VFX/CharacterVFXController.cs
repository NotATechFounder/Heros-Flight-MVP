using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Character.Controllers;
using Plugins.Audio_System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterVFXController : MonoBehaviour,AnimationEventHandlerInterface
    {
        [Header("Bones for VFX")]
        [SerializeField] Transform weaponVfxBone;
        [SerializeField] Transform ultVfxBone;
        
        
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
        CharacterAnimationControllerInterface animationController;
        Dictionary<string, Dictionary<int, ParticleSystem>> characterAnimationVfxCache = new();

        public void Initialize(CharacterAnimations characterSoCharacterAnimations)
        {
            animationController = GetComponent<CharacterAnimationController>();
            animationController.OnAnimationEvent += HandleAnimationEvent;
            animationController.OnAttackAnimationStateChange += HandleAttackAnimationStateChange;
            GenerateCache(characterSoCharacterAnimations);
        }

        void GenerateCache(CharacterAnimations characterSoCharacterAnimations)
        {
            foreach (var data in characterSoCharacterAnimations.AttackAnimationsData)
            {
                characterAnimationVfxCache.Add(data.Aniamtion.Animation.Name, new Dictionary<int, ParticleSystem>());
                foreach (var entry in data.VfxEntries)
                {
                    var particle = Instantiate(entry.Particle, weaponVfxBone);
                    characterAnimationVfxCache[data.Aniamtion.Animation.Name].Add(entry.Index, particle);
                }
            }

            foreach (var data in characterSoCharacterAnimations.UltAnimationsData)
            {
                characterAnimationVfxCache.Add(data.Aniamtion.Animation.Name, new Dictionary<int, ParticleSystem>());
                foreach (var entry in data.VfxEntries)
                {
                    var particle = Instantiate(entry.Particle, ultVfxBone);
                    characterAnimationVfxCache[data.Aniamtion.Animation.Name].Add(entry.Index, particle);
                }
            }
        }

        void HandleAttackAnimationStateChange(bool isActive)
        {
            weaponVfxBone.gameObject.SetActive(isActive);
        }

        public void InjectShaker(CameraShakerInterface cameraShakerInterface)
        {
            this.cameraShakerInterface = cameraShakerInterface;
           
        }

        void HandleAnimationEvent(AnimationEventInterface animationEvent)
        {
            if (animationEvent.Type != AniamtionEventType.Vfx)
                return;

            var vfxEvent = animationEvent as VFXAnimationEvent;
            if (characterAnimationVfxCache.TryGetValue(vfxEvent.AnimationName, out var cache))
            {
              
                if (cache.TryGetValue(vfxEvent.EventIndex, out var particle))
                {
                    var main = particle.main;
                    main.simulationSpeed = vfxEvent.SpeedMultiplier;
                    particle.Play();
                }
            }
            
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

            AudioManager.PlaySoundEffect(audioID,SoundEffectCategory.Items);
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
                   // expEffect.Play();
                    break;
            }

            AudioManager.PlaySoundEffect(audioID,SoundEffectCategory.Items, true);
        }
    }
}

