using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOverlap : OverlapChecker
{
    [Header("Box")]
    [SerializeField] private Vector2 size;

    public override bool HasDetected()
    {
        hitCount = Physics2D.OverlapBoxNonAlloc(pos, size, transform.eulerAngles.z, colliders, layerMask);
        return hitCount != 0;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(localOffset * facingVector, size);
    }
}
