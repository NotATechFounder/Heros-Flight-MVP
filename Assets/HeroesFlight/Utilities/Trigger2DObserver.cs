using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DObserver : MonoBehaviour
{
    public event Action<Collider2D> OnEnter;
    public event Action<Collider2D> OnStay;
    public event Action<Collider2D> OnExit;

    [SerializeField] private LayerMask layerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer))
        {
            OnEnter?.Invoke(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer))
        {
            OnStay?.Invoke(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer))
        {
            OnExit?.Invoke(collision);
        }
    }

    public bool IsInLayerMask(int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
