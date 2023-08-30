using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverlapChecker : MonoBehaviour
{
    public Action<int, Collider2D[]> OnDetect;

    public enum Direction { Left, Right }

    [SerializeField] protected Color color;
    [SerializeField] protected Vector2 localOffset;
    [SerializeField] protected int maxHit = 10;
    [SerializeField] protected LayerMask layerMask;

    protected Direction _direction = Direction.Right;
    protected Collider2D[] colliders;
    protected int hitCount;
    protected Vector2 facingVector = Vector2.one;
    protected Vector2 pos;

    public Vector2 Offset => localOffset;
    


    private void Start()
    {
        colliders = new Collider2D[maxHit];
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public virtual void SetDirection(Direction direction)
    {
        _direction = direction;
        facingVector = GetFacingVector(_direction);
    }

    public abstract bool HasDetected();

    public bool TargetInRange()
    {
        pos = transform.position + transform.rotation * (localOffset * facingVector);
        return HasDetected();
    }

    public bool Detect()
    {
        pos = transform.position + transform.rotation * (localOffset * facingVector);
        if (HasDetected())
        {
            OnDetect?.Invoke(hitCount, colliders);
            return true;
        }
        return false;
    }

    public virtual float GetSizeX()
    {
        return 0;
    }

    private Vector2 GetFacingVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return new Vector2(-1, 1);
            case Direction.Right:
                return new Vector2(1, 1);
            default:
                return Vector2.zero;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        facingVector = GetFacingVector(_direction);
    }
    
}
