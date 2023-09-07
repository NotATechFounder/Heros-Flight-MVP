using System;
using System.Collections;
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
        [Header("Visuals")]
        [SerializeField] GameObject attackIcon;
        [SerializeField] GameObject deffenceIcon;
        Coroutine buffRoutine;
        
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            timeSincelastUse = 0;
           
        }
        public override void UseAbility(Action onComplete = null)
        {
          var  type= (BossBuffType)Random.Range(0, Enum.GetValues(typeof(BossBuffType)).Length);
            CoroutineUtility.WaitForSeconds(2f, () =>
            {
                if (buffRoutine != null)
                    StopCoroutine(buffRoutine);
                StartCoroutine(ProcessBuff(type));
            });
            if (targetAnimation != null)
            {
                animator.PlayAnimation(targetAnimation,onComplete);
            }

            timeSincelastUse = coolDown;
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
            }
            yield return new WaitForSeconds(buffDuration);
            foreach (var ability in attackAbilities)
            {
                ability.SetModifierValue(1);
            }
            attackIcon.SetActive(false);
            deffenceIcon.SetActive(false);
          
        }
    }
}