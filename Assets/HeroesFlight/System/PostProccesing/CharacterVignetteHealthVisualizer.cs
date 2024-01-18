using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HeroesFlight.System.PostProccesing
{
    public class CharacterVignetteHealthVisualizer : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        
        [Header("threshholds")] 
        [SerializeField]
        private List<ThresholdEntry> healthThresholds;

        private Vignette vignette; 
        private void Awake()
        {
            volume.profile.TryGet(out vignette);
        }

        public void UpdateVignetteIntensity(float characterHpPerc)
        {
            Debug.Log(characterHpPerc);
            foreach (var threshold in healthThresholds)
            {
                if (characterHpPerc >= threshold.HealthValue)
                {
                    SetVolumeWeight(threshold.IntensityValue);
                    return;
                }
            }
        }
        
        private void SetVolumeWeight(float intensity)
        {
            Debug.Log($"setting intensity to {intensity}");
            vignette.intensity.Override(intensity);
        }
    }
}