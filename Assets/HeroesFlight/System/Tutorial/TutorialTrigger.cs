using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger2DObserver))]
public class TutorialTrigger : MonoBehaviour
{
    private Trigger2DObserver trigger2DObserver;
    private Action onTriggered;

    private void Awake()
    {
        trigger2DObserver = GetComponent<Trigger2DObserver>();
        trigger2DObserver.OnEnter += OnEnter;
    }

    public void Activate(Vector2 position, Action action)
    {
        transform.position = position;
        onTriggered = action;
        gameObject.SetActive(true);
    }

    public void Trigger()
    {
        onTriggered?.Invoke();
        Disable();
    }

    public void Disable()
    {
        onTriggered = null;
        gameObject.SetActive(false);
    }

    private void OnEnter(Collider2D collider2D)
    {
        Trigger();
    }
}
