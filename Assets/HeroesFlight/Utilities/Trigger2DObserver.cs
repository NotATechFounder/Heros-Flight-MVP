using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger2DObserver : MonoBehaviour
{
    public event Action<Collider2D> OnEnter;
    public event Action<Collider2D> OnStay;
    public event Action<Collider2D> OnExit;

    [SerializeField] private LayerMask layerMask;

    [Header("Debug Option")]
    [SerializeField] private bool showDebug;
    [SerializeField] private Color debugColor = Color.red;

    private Collider2D _collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer))
        {
            Debug.Log("triggering collision message with obj "+collision.name);
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

    private void OnDrawGizmos()
    {
        if (!showDebug)
            return;
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        Gizmos.color = debugColor;
        switch (_collider)
        {
            case BoxCollider2D boxCollider2D:
                Gizmos.DrawWireCube(boxCollider2D.bounds.center, boxCollider2D.size);
                break;
            case CircleCollider2D circleCollider2D:
                Gizmos.DrawWireSphere(circleCollider2D.bounds.center, circleCollider2D.radius);
                break;
            case PolygonCollider2D polygonCollider2D:
                for (int i = 0; i < polygonCollider2D.points.Length; i++)
                {
                    Gizmos.DrawLine(polygonCollider2D.points[i], polygonCollider2D.points[(i + 1) % polygonCollider2D.points.Length]);
                }
                break;
        }
    }
}
