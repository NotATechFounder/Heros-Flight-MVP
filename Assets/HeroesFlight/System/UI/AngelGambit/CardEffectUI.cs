using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CardEffectUI : MonoBehaviour
{
    [SerializeField] private Transform rootParent;
    [SerializeField] private Transform defaultParent;
    RectTransform rectTransform;
    private Vector2 defaultOrigin;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultOrigin = rectTransform.position;
    }

    public void MoveTo(Transform target, Action OnReached)
    {
        transform.parent = rootParent;
        rectTransform.JuicyMove(target.position, 1f)
            .SetEase(Ease.EaseInOutSine)
            .SetOnComplected(() =>
            {
                OnReached?.Invoke();
                ResetParent();
            })
            .Start();
    }

    public void ResetParent()
    {
        transform.parent = defaultParent;
        transform.position = defaultOrigin;
    }
}
