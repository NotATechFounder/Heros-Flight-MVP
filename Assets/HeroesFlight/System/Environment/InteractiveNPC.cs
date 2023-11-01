using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public Action OnInteract;
    [SerializeField] protected Trigger2DObserver trigger2DObserver;
    [SerializeField] protected bool hasInteracted = false;

    private void Start()
    {
        trigger2DObserver.OnEnter += OnEnter2D;
    }

    protected void OnEnter2D(Collider2D d)
    {
        if (hasInteracted) return;
        hasInteracted = true;
        Interact();
    }

    protected virtual void Interact()
    {
        OnInteract?.Invoke();
    }
}
