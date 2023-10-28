using System;
using System.Collections;
using Cinemachine;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using StansAssets.Foundation.Async;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossBuffAbility : BossAbilityBase
    {
        [Header("Data")]
        [Range(0,100)]
        [SerializeField] float buffStrenght;
        [SerializeField] float buffDuration;
        [SerializeField] BossAttackAbilityBase[] attackAbilities;
        [SerializeField] BossCrystalsHealthController[] healthControllers;
        [Header("Visuals")]
        [SerializeField] GameObject attackIcon;
        [SerializeField] GameObject deffenceIcon;
        Coroutine buffRoutine;
        float defaultDefence = 30;
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            currentCooldown = 0;
           
        }
        public override void UseAbility(Action onComplete = null)
        {
            var  type= (BossBuffType)Random.Range(0, Enum.GetValues(typeof(BossBuffType)).Length);
            CoroutineUtility.WaitForSeconds(2f, () =>
            {
                if (buffRoutine != null)
                    StopCoroutine(buffRoutine);
                StartCoroutine(ProcessBuff(type));
                abilityParticle.Play();
                cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
            });
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation,onComplete);
            }
           
        }


        IEnumerator ProcessBuff(BossBuffType buffType)
        {
            attackIcon.SetActive(false);
            deffenceIcon.SetActive(false);
            if (buffType == BossBuffType.Attack)
            {
                
                foreach (var ability in attackAbilities)
                {
                    ability.SetModifierValue(buffStrenght);
                }
                attackIcon.SetActive(true);
            }
            else
            {
                deffenceIcon.SetActive(true);
                foreach (var health in healthControllers)
                {
                    health.SetDefence(60f);
                }
            }
            yield return new WaitForSeconds(buffDuration);
            foreach (var ability in attackAbilities)
            {
                ability.SetModifierValue(0);
            }
            foreach (var health in healthControllers)
            {
                health.SetDefence(defaultDefence);
            }
            attackIcon.SetActive(false);
            deffenceIcon.SetActive(false);
          
        }
    }
}