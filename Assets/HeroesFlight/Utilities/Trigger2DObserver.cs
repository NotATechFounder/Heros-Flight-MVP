using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DObserver : MonoBehaviour
{
    public event Action<Collider2D> OnEnter;

    [SerializeField] private LayerMask layerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (layerMask == (layerMask | (1 << collision.gameObject.layer)))
        {
            OnEnter?.Invoke(collision);
        }
    }
}
