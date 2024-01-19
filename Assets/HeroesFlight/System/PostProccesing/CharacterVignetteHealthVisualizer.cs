using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HeroesFlight.System.PostProccesing
{
    public class CharacterVignetteHealthVisualizer : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        [Header("threshholds")] [SerializeField]
        private List<ThresholdEntry> healthThresholds;

        [Header("Flashing parameter")] [SerializeField]
        private float flashDuration;

        [SerializeField] private float flashIntensityModifier = 0.15f;

        private ThresholdEntry currentThreshold;
        private Vignette vignette;

        private void Awake()
        {
            volume.profile.TryGet(out vignette);
            currentThreshold = healthThresholds[0];
            StartCoroutine(FlashingRoutine());
        }

        public void UpdateVignetteIntensity(float characterHpPerc)
        {
            Debug.Log(characterHpPerc);
            foreach (var threshold in healthThresholds)
            {
                if (characterHpPerc >= threshold.HealthValue)
                {
                    SetCurrentThreshold(threshold);
                   return;
                }
            }
        }

        private void SetCurrentThreshold(ThresholdEntry threshold)
        {
            currentThreshold = threshold;
        }

        private void SetVolumeWeight(float intensity)
        {
            Debug.Log($"setting intensity to {intensity}");
            vignette.intensity.Override(intensity);
        }

        IEnumerator FlashingRoutine()
        {
            while (true)
            {
                if (Mathf.Approximately(currentThreshold.IntensityValue, 0f))
                {
                    yield return null; //Skip execution and wait for the next frame
                    continue;
                }

                float elaspedTime = 0f;
                // First half of the flashDuration spent going from currentIntensity to currentIntensity - 0.05f
                while (elaspedTime < flashDuration / 2)
                {
                    elaspedTime += Time.deltaTime;
                    float intensity = Mathf.Lerp(currentThreshold.IntensityValue,
                        currentThreshold.IntensityValue + flashIntensityModifier,
                        elaspedTime / (flashDuration / 2));
                    SetVolumeWeight(intensity);
                    yield return null;
                }

                // Reset elapsed time
                elaspedTime = 0f;

                // Second half spent going from currentIntensity - 0.05f to currentIntensity
                while (elaspedTime < flashDuration / 2)
                {
                    elaspedTime += Time.deltaTime;
                    float intensity = Mathf.Lerp(currentThreshold.IntensityValue + flashIntensityModifier,
                        currentThreshold.IntensityValue,
                        elaspedTime / (flashDuration / 2));
                    SetVolumeWeight(intensity);
                    yield return null;
                }

                yield return new WaitForSeconds(flashDuration);
            }
        }
    }
}