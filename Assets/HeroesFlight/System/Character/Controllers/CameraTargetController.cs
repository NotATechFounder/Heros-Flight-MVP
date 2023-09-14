using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] Vector2 offsetBounds = new Vector2(2, 2);
        [SerializeField] float speed = 5f;
        CharacterControllerInterface controller;
        Transform parent;
        float sinTime;

        void Awake()
        {
            parent = transform.parent;
            controller = GetComponentInParent<CharacterControllerInterface>();
        }

        void Update()
        {
            FollowTarget();
        }

        public void FollowTarget()
        {
            var velocity = controller.GetVelocity();
            if (velocity.x == 0)
                return;
            // var resultVector = new Vector3(Mathf.Clamp(velocity.x, -offsetBounds.x, offsetBounds.x),
            //     Mathf.Clamp(velocity.y, -offsetBounds.y, offsetBounds.y));
            var resultVector = Vector3.zero;
            if (velocity.x > 0)
            {
                resultVector = new Vector3(Mathf.Clamp(velocity.x, -offsetBounds.x, offsetBounds.x),
                    0);
            }
            else if(velocity.x < 0)
            {
                resultVector = new Vector3(0 -offsetBounds.x, 0, 0);
            }
           
            var desiredPosition = parent.position + resultVector;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        }
    }
}