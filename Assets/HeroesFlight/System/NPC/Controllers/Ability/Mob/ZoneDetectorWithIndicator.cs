using System;
using System.Collections;
using Pelumi.Juicer;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class ZoneDetectorWithIndicator : AreaDamageEntity
    {
        [SerializeField] GameObject staticIndicator;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private ParticleSystem particle;
        private JuicerRuntime juicerRuntime;

        private void Awake()
        {
            juicerRuntime=   staticIndicator.transform.JuicyScale(endScale, duration);
        }

        public override void Init()
        {
            tickWait = new WaitForSeconds(tick);
        }


        public override void StartDetection(Action onComplete = null)
        {
            Debug.Log("Starting to detect ");
            overlap.OnDetect += NotifyTargetDetected;
            StartDetectionRoutine(() =>
            {
                overlap.OnDetect -= NotifyTargetDetected;
                onComplete?.Invoke();
                Debug.Log("FInished checking");
            });
        }


        public override void ToggleIndicator(bool isEnabled)
        {
            staticIndicator.SetActive(isEnabled);
        }

        protected override IEnumerator DetectionRoutine(Action onComplete)
        {
            staticIndicator.transform.localScale = Vector3.zero;
            juicerRuntime.Start();
            ToggleIndicator(true);
            yield return new WaitForSeconds(duration);
            overlap.DetectOverlap();
            ToggleIndicator(false);
            if (particle != null) particle.Play();
            onComplete?.Invoke();
        }
    }
}