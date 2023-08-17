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
            var velocity = controller.GetVelocity();
            var resultVector = new Vector3(Mathf.Clamp(velocity.x, -offsetBounds.x, offsetBounds.x),
                Mathf.Clamp(velocity.y, -offsetBounds.y, offsetBounds.y));

            var desiredPosition = parent.position + resultVector;

            
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        }


       
    }
}