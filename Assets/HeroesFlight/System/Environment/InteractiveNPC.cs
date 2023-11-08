using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public event Action OnInteract;
    [SerializeField] protected Trigger2DObserver trigger2DObserver;
    [SerializeField] protected bool hasInteractedSuccesfully = false;

    private void Start()
    {
        trigger2DObserver.OnEnter += OnEnter2D;
    }

    protected void OnEnter2D(Collider2D d)
    {
        if (hasInteractedSuccesfully) return;
        Interact();
    }

    protected virtual void Interact()
    {
        OnInteract?.Invoke();
    }

    public void InteractionComplected()
    {
        hasInteractedSuccesfully = true;
    }

    private void OnDestroy()
    {
        trigger2DObserver.OnEnter -= OnEnter2D;
        OnInteract = null;
    }
}
