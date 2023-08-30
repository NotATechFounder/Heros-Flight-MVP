using UnityEngine;

namespace Plugins._2D_OverlapChecker
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class CapsuleOverlap : OverlapChecker
    {
        [Header("Capsule")]
        [SerializeField] Vector2 size;
        [SerializeField] CapsuleDirection2D direction;
        [SerializeField] float angle;
        [SerializeField] CapsuleCollider2D visualCollider;


        public override bool HasDetected()
        {
            hitCount = Physics2D.OverlapCapsuleNonAlloc(pos, size, direction, angle, colliders, layerMask);
            return hitCount != 0;
        }

        public override float GetSizeX()
        {
            return size.x;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            visualCollider.direction = direction;
            visualCollider.size = size;
            visualCollider.offset = localOffset * facingVector;
        }
    }
}