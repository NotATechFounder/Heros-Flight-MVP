using System;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EntanglingRoot : AreaDamageEntity
    {
        [SerializeField] GameObject staticIndicator;

        public override void Init()
        {
            tickWait = new WaitForSeconds(tick);
        }


        public override void StartDetection(Action onComplete)
        {
            if (overlap.DetectOverlap())
            {
                overlap.OnDetect += NotifyTargetDetected;


                Debug.Log("Starting to detect ");
                StartDetectionRoutine(() =>
                {
                    overlap.OnDetect -= NotifyTargetDetected;
                    onComplete?.Invoke();
                    Debug.Log("FInished checking");
                });
            }
            else
            {
                ToggleIndicator(false);
            }
        }


        public override void ToggleIndicator(bool isEnabled)
        {
            staticIndicator.SetActive(isEnabled);
        }
    }
}