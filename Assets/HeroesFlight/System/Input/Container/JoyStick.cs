using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Joystick : OnScreenControl
{
    public static Joystick Instance { get; private set; }

    public bool IsTouching { get; private set; }

    public Vector2 MovementAmount => _movementAmount;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public Action OnTap;

    public Action OnDrag;

    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPath;

    [SerializeField] private bool fixedJoystick;

    [SerializeField] private bool hideJoystick;

    [SerializeField] private float dragThereshold = 0.4f;

    [SerializeField] private RectTransform joyStick;

    [SerializeField] private RectTransform knob;

    [SerializeField] private Transform[] positionFocus;

    [SerializeField] private Event touchZoneOutputEvent;

    private Vector2 _movementAmount;
    private Finger _movementFinger;
    private Vector2 _joystickSize;
    private Vector2 _knobPosition;
    private float _maxMovement;
    private float _releaseTime;
    private ETouch.Touch _currentTouch;
    private List<RaycastResult> uiHit;

    private void Awake()
    {
        Instance = this;
        uiHit = new List<RaycastResult>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += ETouch_onFingerDown;
        ETouch.Touch.onFingerUp += ETouch_onFingerUp;
        ETouch.Touch.onFingerMove += ETouch_onFingerMove;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EnhancedTouchSupport.Disable();
        ETouch.Touch.onFingerDown -= ETouch_onFingerDown;
        ETouch.Touch.onFingerUp -= ETouch_onFingerUp;
        ETouch.Touch.onFingerMove -= ETouch_onFingerMove;
    }

    private void ETouch_onFingerDown(Finger touchedFinger)
    {
        if (ClickOnUI(touchedFinger) || (_movementFinger == null && touchedFinger.screenPosition.y >= Screen.height / 1.5f))
        {
            return;
        }

        IsTouching = true;
        _movementFinger = touchedFinger;
        _movementAmount = Vector2.zero;

        joyStick.gameObject.SetActive(true);

        _joystickSize = joyStick.sizeDelta;
        if (!fixedJoystick) joyStick.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);

        SetFocus();
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if(startPosition.x < _joystickSize.x / 2f)
        {
            startPosition.x = _joystickSize.x / 2f;
        }

        if (startPosition.y < _joystickSize.y /2f)
        {
            startPosition.y = _joystickSize.y / 2f;
        }
        else if (startPosition.y > Screen.height - _joystickSize.y / 2f)
        {
            startPosition.y = Screen.height - _joystickSize.y / 2f;
        }

        return startPosition;
    }

    private void ETouch_onFingerMove(Finger onFingerMove)
    {
        if (_movementFinger == onFingerMove)
        {
            _releaseTime += Time.deltaTime;

            _knobPosition = Vector2.zero;
            _maxMovement = _joystickSize.x / 2f;
            _currentTouch = _movementFinger.currentTouch;

            if(Vector2.Distance(_currentTouch.screenPosition, joyStick.anchoredPosition) > _maxMovement)
            {
                _knobPosition = (_currentTouch.screenPosition - joyStick.anchoredPosition).normalized * _maxMovement;
            }
            else
            {
                _knobPosition = _currentTouch.screenPosition - joyStick.anchoredPosition;
            }

            knob.anchoredPosition = _knobPosition;
            _movementAmount = _knobPosition / _maxMovement;
            SendValueToControl(_movementAmount);
            SetFocus();
        }
    }

    private void ETouch_onFingerUp(Finger lostFinger)
    {
        if (_movementFinger == lostFinger)
        {
            IsTouching = false;
            if (_movementFinger.currentTouch.isTap)
            {
                if (OnTap != null) OnTap.Invoke();
            }
            else if (OnDrag != null && _releaseTime < 0.2f && knob.localPosition.magnitude >= joyStick.rect.width * dragThereshold)
            {
                if (OnDrag != null) OnDrag.Invoke();
            }

            _releaseTime = 0;

            _movementFinger = null;
            _movementAmount = Vector2.zero;
            knob.anchoredPosition = Vector2.zero;
            joyStick.gameObject.SetActive(!hideJoystick);
            SendValueToControl(Vector2.zero);
            SetFocus();
        }
    }

    private bool ClickOnUI(Finger touchedFinger)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touchedFinger.screenPosition;
        EventSystem.current.RaycastAll(pointerEventData, uiHit);

        foreach (RaycastResult item in uiHit)
        {
           if(item.gameObject.layer == LayerMask.NameToLayer("UI_IgnoreTouch"))
            return true;
        }
        return false;
    }

    void SetFocus()
    {
        positionFocus[0].gameObject.SetActive(_movementAmount.x < 0 && _movementAmount.y > 0);
        positionFocus[1].gameObject.SetActive(_movementAmount.x > 0 && _movementAmount.y > 0);
        positionFocus[2].gameObject.SetActive(_movementAmount.x > 0 && _movementAmount.y < 0);
        positionFocus[3].gameObject.SetActive(_movementAmount.x < 0 && _movementAmount.y < 0);
    }
}
