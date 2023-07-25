using UnityEngine;
using UnityEngine.UI;
using System;
using Pelumi.Juicer;

public class SliderHelper : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private AdvanceButton _button;
    [SerializeField] private float _durationAnimation = 0.3f;

    public void SetValue(bool value)
    {
        UpdateValue(value);
    }

    private void Awake()
    {
        _button.OnToggle.AddListener(UpdateValue);
        _button.SetIsOn(true);
    }


    private void UpdateValue(bool notify = true)
    {
        _slider.JuicyValue(notify ==  true? 1 : 0, _durationAnimation).Start();
    }
}
