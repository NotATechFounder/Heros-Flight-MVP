using System;
using System.Collections;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AbilityEnableObjectTrigger : AbilityBaseNPC
    {
        [SerializeField] GameObject targetObject;
        [SerializeField] float duration;
        [SerializeField] Trigger2DObserver observer;
        Coroutine routine;
        WaitForSeconds waiter;
        bool isRunning;
        
        protected override void Awake()
        {
            base.Awake();
            waiter = new WaitForSeconds(duration);
            observer.OnEnter += TriggerAbility;
            observer.OnStay += TriggerAbility;
        }

        void TriggerAbility(Collider2D collider2D1)
        {
            if (isRunning || !ReadyToUse)
                return;
          
            isRunning = true;
            UseAbility();
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
            
            Debug.Log("Using ability");
            StartCoroutine(ObjectEnableRoutine(onComplete));

        }


        IEnumerator ObjectEnableRoutine(Action OnComplete)
        {
            
            targetObject.SetActive(true);
            yield return waiter;
            targetObject.SetActive(false);
            OnComplete?.Invoke();
            currentCooldown = CoolDown;
            isRunning = false;
        }
    }
}