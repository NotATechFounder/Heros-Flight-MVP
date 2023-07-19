using UnityEngine;
using UnityEngine.UI;
using System;
using Pelumi.Juicer;

public class SliderHelper : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Button _button;
    [SerializeField] private float _durationAnimation = 0.3f;

    public event Action<bool> OnValueChanged;


    private bool _isOn = true;

    public void SetValue(bool value)
    {
        _isOn = value;
        UpdateValue(false);
    }

    private void Awake()
    {
        _button.onClick.AddListener(ChangeStateButton);
    }

    private void ChangeStateButton()
    {
        _isOn = !_isOn;
        UpdateValue();
    }

    private void UpdateValue(bool notify = true)
    {
        _slider.JuicyValue(_isOn ==  true? 1 : 0, _durationAnimation).Start();
        if(notify == true) OnValueChanged?.Invoke(_isOn);
    }
}
