using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectUI : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private Transform rootParent;
    [SerializeField] private Transform defaultParent;
    [SerializeField] private bool changeParent;

    RectTransform rectTransform;
    private Vector2 defaultOrigin;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultOrigin = rectTransform.position;
        gameObject.SetActive(false);
    }

    public void MoveTo(Transform target, Action OnReached)
    {
        gameObject.SetActive(true);
        if (changeParent) transform.parent = rootParent;
        rectTransform.JuicyMove(target.position, 1f)
            .SetDelay(delay)
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
        if (changeParent) transform.parent = defaultParent;
        transform.position = defaultOrigin;
        gameObject.SetActive(false);
    }
}
