using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DObserver : MonoBehaviour
{
    public event Action<Collider2D> OnEnter;

    [SerializeField] private LayerMask _layerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_layerMask == (_layerMask | (1 << collision.gameObject.layer)))
        {
            OnEnter?.Invoke(collision);
        }
    }
}
