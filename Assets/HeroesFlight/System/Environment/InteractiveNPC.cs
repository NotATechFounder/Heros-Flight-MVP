using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public Action OnInteract;
    [SerializeField] Trigger2DObserver trigger2DObserver;
    [SerializeField] private bool hasInteracted = false;

    private void Start()
    {
        trigger2DObserver.OnEnter += OnEnter2D;
    }

    private void OnEnter2D(Collider2D d)
    {
        if (hasInteracted) return;
        hasInteracted = true;
        OnInteract?.Invoke();
    }
}
