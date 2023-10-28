using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] Vector2 offsetBounds = new Vector2(2, 2);
        [SerializeField] float speed = 5f;
        [SerializeField] float cameraLagDelay = 0.4f;
        [SerializeField] float cameraLagSpeed = 12f;
        [SerializeField] float cameraLagLowerThreshold = 4f;
        CharacterControllerInterface controller;
        Transform parent;

        private float cosTime;
        private Vector2 activeOffsetBounds = new Vector2(0, 0);

        private float activeSpeed = 0f;
        float sinTime;

        void Awake()
        {
            parent = transform.parent;
            controller = GetComponentInParent<CharacterControllerInterface>();
        }

        void Update()
        {
            FollowTargetDelayed();
        }

        public void FollowTarget()
        {
            var velocity = controller.GetVelocity();
            if (velocity.x == 0) {
                return;
            }
            // var resultVector = new Vector3(Mathf.Clamp(velocity.x, -offsetBounds.x, offsetBounds.x),
            //     Mathf.Clamp(velocity.y, -offsetBounds.y, offsetBounds.y));
            var resultVector = Vector3.zero;
            if (velocity.x > 0)
            {
                resultVector = new Vector3(Mathf.Clamp(velocity.x, -activeOffsetBounds.x, activeOffsetBounds.x),
                    0);
            }
            else if(velocity.x < 0)
            {
                resultVector = new Vector3(0 -activeOffsetBounds.x, 0, 0);
            }

            var desiredPosition = parent.position + resultVector;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        }

        private void FollowTargetDelayed()
        {
            // Follows the target but after a slight delay defined as cameraLag in seconds.
            var velocity = controller.GetVelocity();
            if (velocity.x == 0) {
                cosTime = 0; // Reset.
                return;
            }

            var resultVector = Vector3.zero;
            doCameraLag();
            if (velocity.x > 0)
            {
                resultVector = new Vector3(Mathf.Clamp(velocity.x, activeOffsetBounds.x-cameraLagLowerThreshold, activeOffsetBounds.x),
                    0);
            }
            else if(velocity.x < 0)
            {
                resultVector = new Vector3(0 -activeOffsetBounds.x, 0, 0);
            }

            var desiredPosition = parent.position + resultVector;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, activeSpeed * Time.deltaTime);
        }

        private void doCameraLag()
        {
            if (cosTime > cameraLagDelay)
            {
                Debug.Log("Camera snap to target.");
                activeSpeed = speed;
                activeOffsetBounds = activeOffsetBounds - (activeOffsetBounds * activeSpeed * Time.deltaTime);
                if (activeOffsetBounds.x < 0.1f)
                {
                    activeOffsetBounds = Vector2.zero;
                }
            }
            else
            {
                activeOffsetBounds = offsetBounds;
                activeSpeed = cameraLagSpeed;
                cosTime += Time.deltaTime;
            }
        }
    }
}