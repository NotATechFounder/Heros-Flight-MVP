using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOverlap : OverlapChecker
{
    [Header("Sphere")]
    [SerializeField] private float size;

    public override bool HasDetected()
    {
        hitCount = Physics2D.OverlapCircleNonAlloc(pos, size, colliders, layerMask);
        return hitCount != 0;
    }

    public override float GetSizeX()
    {
        return size;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(localOffset * facingVector, size);
    }
}
