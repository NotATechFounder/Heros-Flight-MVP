using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CameraUiHook : MonoBehaviour
    {
        [SerializeField] Slider characterCameraSlider;
        [SerializeField] TextMeshProUGUI characterText;
        [SerializeField] Slider skillCameraSlider;
        [SerializeField] TextMeshProUGUI skillText;

        public Slider CharacterSlider => characterCameraSlider;
        public Slider SkillSLider => skillCameraSlider;

        void Awake()
        {
            characterCameraSlider.onValueChanged.AddListener(HandleCharacterSliderChanged); 
            skillCameraSlider.onValueChanged.AddListener(HandleSkillSliderValueChanged); 
        }

        void HandleSkillSliderValueChanged(float value)
        {
            skillText.text = value.ToString();
        }

        void HandleCharacterSliderChanged(float value)
        {
            characterText.text = value.ToString();
        }

        public void SetCharacterSliderValue(float value)
        {
            characterCameraSlider.value = value;
        }

        public void SetSkillSliderValue(float value)
        {
            skillCameraSlider.value = value;
        }
    }
}