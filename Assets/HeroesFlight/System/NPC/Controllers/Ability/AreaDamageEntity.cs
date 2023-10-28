using System;
using System.Collections;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AreaDamageEntity : MonoBehaviour
    {
        [SerializeField] protected OverlapChecker overlap;
        [SerializeField] protected float duration;
        [SerializeField] protected float tick = 0.5f;
        public event Action<int, Collider2D[]> OnTargetsDetected;
        protected WaitForSeconds tickWait;
        protected Coroutine detectionRoutine;

        public virtual void Init()
        {
            overlap.OnDetect += NotifyTargetDetected;
            tickWait = new WaitForSeconds(tick);
        }

        protected virtual void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            OnTargetsDetected?.Invoke(count, targets);
        }

        public virtual void StartDetection(Action onComplete = null)
        {
            StartDetectionRoutine(onComplete);
        }

        protected void StartDetectionRoutine(Action onComplete)
        {
            ToggleIndicator(false);
            if (detectionRoutine != null)
                StopCoroutine(detectionRoutine);
            detectionRoutine = StartCoroutine(DetectionRoutine(onComplete));
        }

        public virtual void ToggleIndicator(bool isEnabled)
        {
        }


        IEnumerator DetectionRoutine(Action onComplete)
        {
            float currentDuration = 0;
            while (currentDuration < duration)
            {
                overlap.Detect();
                yield return tickWait;
                currentDuration += tick;
            }

            onComplete?.Invoke();
        }

        void OnDestroy()
        {
            if (detectionRoutine != null)
                StopCoroutine(detectionRoutine);
        }
    }
}